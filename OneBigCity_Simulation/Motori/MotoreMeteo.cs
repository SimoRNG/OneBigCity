using OneBigCity_Core.Classi.Eventi;
using OneBigCity_Core.Classi.Meteo;
using OneBigCity_Core.Classi.Meteo.EnumMeteo;
using OneBigCity_Core.Classi.Static;
using OneBigCity_Simulation.Precompilati;

namespace OneBigCity_Simulation.Motori;

public class MotoreMeteo
{
    /*
     * Classe che si occupa della gestione delle condizioni atmosferiche.
     * Per rendere realistico il cambio delle temperature in base alla data applico due strategie:
     * 1. Ipotizzo che la temperatura oscilli come una sinusoidale durante l'anno, quindi per ogni
     *  giorno dell'anno ho una temperatura 'target'
     * 2. Non faccio fare sbalzi di temperatura estremi tra un giorno e l'altro
     */


    private readonly EventBus _eventBus;
    private Dictionary<DateTime, CondizioneAtmosferica> StoricoMeteo = new();

    public CondizioneAtmosferica CondizioneAttuale { get; private set; }

    #region Precompilati
    internal static readonly TipoMeteo[] ValoriMeteo = Enum.GetValues<TipoMeteo>();
    // Tabella pre-calcolata delle probabilità di transizione base tra i vari meteo (MeteoIeri -> MeteoOggi -> PesoBase)
    private static readonly Dictionary<(TipoMeteo Da, TipoMeteo A), float> PesiBaseTransizione = MeteoLUT.InizializzaPesiBase();
    #endregion

    public MotoreMeteo(EventBus eventBus, CondizioneAtmosferica meteoIniziale = new())
    {
        _eventBus = eventBus;

        CondizioneAttuale = meteoIniziale;

        _eventBus.Iscrivi<EventoNuovoGiorno>(OnNuovoGiorno);

    }

    private void OnNuovoGiorno(EventoNuovoGiorno e)
    {
        // Notifico se cambia stagione
        if (StoricoMeteo.Count > 0)
        {
            Stagione stagioneCorrente = CalendarioStagioni.OttieniStagioneDaData(e.DataCorrente);
            Stagione stagioneIeri = CalendarioStagioni.OttieniStagioneDaData(e.DataCorrente.AddDays(-1));
            if (stagioneCorrente != stagioneIeri) _eventBus.Pubblica(new EventoSistema($"Stagione cambiata. Nuova stagione: {stagioneCorrente.ToString()}"));
        }


        // Calcolo un nuovo meteo (che può essere uguale al precedente)
        // Se ci sono variazioni notifico l'eventbus

        CondizioneAtmosferica condizionePrecedente = CondizioneAttuale;

        TipoMeteo nuovoTipo = CalcolaNuovoMeteo(e.DataCorrente);
        float nuovaTemp = CalcolaNuovaTemperatura(e.DataCorrente, nuovoTipo);

        CondizioneAttuale = new CondizioneAtmosferica(nuovoTipo, nuovaTemp);

        StoricoMeteo[e.DataCorrente] = CondizioneAttuale; // AGGIORNAMENTO DELLO STORICO

        // Notifica se la temperatura è cambiata in modo significativo (3 gradi) o se è cambiato il tipo di meteo
        if (CondizioneAttuale.Tipo != condizionePrecedente.Tipo ||
            Math.Abs(CondizioneAttuale.TemperaturaCelsius - condizionePrecedente.TemperaturaCelsius) >= 3f)
        {
            _eventBus.Pubblica(new EventoMeteoCambiato(condizionePrecedente, CondizioneAttuale));
        }
    }

    private TipoMeteo CalcolaNuovoMeteo(DateTime data)
    {
        // Calcola il nuovo meteo a partire dalla data passata

        Stagione stagioneCorrente = CalendarioStagioni.OttieniStagioneDaData(data);

        if (StoricoMeteo.Count == 0) return CalendarioStagioni.OttieniMeteoStagionale(stagioneCorrente); // Caso inizio simulazione

        TipoMeteo meteoIeri = StoricoMeteo.TryGetValue(data.AddDays(-1), out var mIeri) ? mIeri.Tipo : CondizioneAttuale.Tipo;

        int severitaIeri = (int)meteoIeri;

        // Pesi base per tutti i meteo possibili
        var pesiMeteo = new Dictionary<TipoMeteo, float>();

        // Sostituita questa logica con l'uso di tabelle di lookup
        /*var valoriMeteo = Enum.GetValues<TipoMeteo>();

        foreach (TipoMeteo meteo in valoriMeteo)
        {
            int delta = Math.Abs((int)meteo - severitaIeri);
            float pesoBase = delta switch
            {
                0 => 50f, // Alta probabilità di stabilità
                1 => 30f, // Buona probabilità di piccola variazione (+-1)
                2 => 15f, // Piccola probabilità di variazione (+-2)
                _ => 5f   // Piccolissima probabilità di cambiamento repentino (Delta >= 3)
            };
        
            // Modificatore stagionale
            float fattoreStagionale = CalendarioStagioni.OttieniFattoreStagionale(meteo, stagioneCorrente);
            float pesoFinale = pesoBase * fattoreStagionale;

            if (pesoFinale > 0)
            {
                pesiMeteo[meteo] = pesoFinale;
            }
        }
        */

        foreach (TipoMeteo meteoTarget in ValoriMeteo)
        {
            float pesoBase = PesiBaseTransizione[(meteoIeri, meteoTarget)];
            float fattoreStagionale = CalendarioStagioni.OttieniFattoreStagionale(meteoTarget, stagioneCorrente);
            float pesoFinale = pesoBase * fattoreStagionale;

            if (pesoFinale > 0)
            {
                pesiMeteo[meteoTarget] = pesoFinale;
            }
        }

        return EstraiMeteoPonderato(pesiMeteo);
    }

    private TipoMeteo EstraiMeteoPonderato(Dictionary<TipoMeteo, float> pesi)
    {
        float sommaPesi = pesi.Values.Sum();

        // Cast in double prima della divisione per avere la precisione decimale corretta
        double fattoreRandom = (double)Utilita.RNG(0, 100) / 100.0;
        double estrazione = fattoreRandom * sommaPesi;

        float accumulatore = 0;
        foreach (var (meteo, peso) in pesi)
        {
            accumulatore += peso;
            if (estrazione <= accumulatore)
                return meteo;
        }

        return pesi.Keys.First();
    }

    public float CalcolaNuovaTemperatura(DateTime data, TipoMeteo nuovoTipo)
    {
        // Temperature medie in base alla stagione
        float targetStagionale = CalendarioStagioni.CalcolaTemperaturaStagionaleBase(data);

        // Il meteo influisce sulla temperatura
        float modifMeteo = nuovoTipo switch
        {
            TipoMeteo.Temporale => -3.0f,
            TipoMeteo.PioggiaLeggera => -1.5f,
            TipoMeteo.Neve => -4.0f,
            TipoMeteo.Sereno => +2.0f,
            _ => 0.0f
        };

        float targetOggi = targetStagionale + modifMeteo;

        // Se è il primo giorno della simulazione - direttamente target
        if (StoricoMeteo.Count == 0) return targetOggi;

        float tempIeri = CondizioneAttuale.TemperaturaCelsius;

        // La temperatura si sposta dell'80% verso il target di oggi + un piccolo rumore casuale (+-0.5°C)
        float variazioneCasuale = (float)(Utilita.RNG(-5, 5) / 10.0);
        float nuovaTemp = tempIeri + (targetOggi - tempIeri) * 0.8f + variazioneCasuale;

        return (float)Math.Round(nuovaTemp, 1);
    }
}
