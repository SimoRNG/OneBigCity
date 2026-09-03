using OneBigCity_Core.Classi.Eventi;
using OneBigCity_Core.Classi.Meteo.EnumMeteo;
using OneBigCity_Core.Classi.Static;

namespace OneBigCity_Simulation.Motori;

public class MotoreMeteo
{
    private readonly EventBus _eventBus;
    private Dictionary<DateTime, TipoMeteo> StoricoMeteo = new();

    public TipoMeteo MeteoAttuale { get; private set; }

    public MotoreMeteo(EventBus eventBus, TipoMeteo meteoIniziale = TipoMeteo.Sereno)
    {
        _eventBus = eventBus;

        MeteoAttuale = meteoIniziale;

        _eventBus.Iscrivi<EventoNuovoGiorno>(OnNuovoGiorno);

    }

    private void OnNuovoGiorno(EventoNuovoGiorno e)
    {
        // Calcolo un nuovo meteo (che può essere uguale al precedente)
        // Se ci sono variazioni notifico l'eventbus

        TipoMeteo meteoPrecedente = MeteoAttuale;
        MeteoAttuale = CalcolaNuovoMeteo(e.DataCorrente);

        // Aggiorna storico
        StoricoMeteo[e.DataCorrente] = MeteoAttuale;

        // Notifica solo se c'è stato un cambio rispetto al giorno prima
        if (MeteoAttuale != meteoPrecedente)
        {
            _eventBus.Pubblica(new EventoMeteoCambiato(meteoPrecedente, MeteoAttuale));
        }
    }

    private TipoMeteo CalcolaNuovoMeteo(DateTime data)
    {
        Stagione stagioneCorrente = CalendarioStagioni.OttieniStagioneDaData(data);

        if (StoricoMeteo.Count == 0) return CalendarioStagioni.OttieniMeteoStagionale(stagioneCorrente); // Caso inizio simulazione

        TipoMeteo meteoIeri = StoricoMeteo.TryGetValue(data.AddDays(-1), out var mIeri) ? mIeri : MeteoAttuale;

        int severitaIeri = (int)meteoIeri;

        // Pesi base per tutti i meteo possibili
        var pesiMeteo = new Dictionary<TipoMeteo, float>();
        var valoriMeteo = Enum.GetValues<TipoMeteo>();

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
}
