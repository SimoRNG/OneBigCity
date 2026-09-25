using OneBigCity_Model.AutonomiaCitta.Obiettivi;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Interfacce;
using OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;

namespace OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;

public class GestoreUtilityAgenti
{
    private readonly List<ObiettivoGOAP> _obiettiviRegistrati = new();
    private readonly PianificatoreGOAP _pianificatore = new();

    public GestoreUtilityAgenti(List<ObiettivoGOAP> obiettiviDisponibili)
    {
        _obiettiviRegistrati = obiettiviDisponibili;
    }

    /// <summary>
    /// Valuta tutti gli obiettivi registrati e restituisce quello con la priorità maggiore per l'agente
    /// </summary>
    public ObiettivoGOAP? SelezionaObiettivoMigliore(AgenteBase agente, Citta citta)
    {
        ObiettivoGOAP? obiettivoMigliore = null;
        double prioritaMassima = -1;

        foreach (var obiettivo in _obiettiviRegistrati)
        {
            if (obiettivo.IsSoddisfatto(agente, citta))
                continue;

            float priorita = obiettivo.CalcolaPriorita(agente, citta);

            if (priorita > prioritaMassima)
            {
                prioritaMassima = priorita;
                obiettivoMigliore = obiettivo;
            }
        }

        return obiettivoMigliore;
    }

    public void AggiornaDecisioneAgente(AgenteBase agente, Citta citta, List<AzioneGOAP> azioniDisponibili)
    {
        // Controlla se l'agente ha bisogno di un nuovo piano GOAP ed eventualmente lo ricalcola

        bool haPianoInCorso = agente.PianoCorrente.Count > 0 || agente.AzioneInEsecuzione != null;

        if (haPianoInCorso)
        {
            // TODO - inserire qui una soglia per l'interruzione se un bisogno diventa gravissimo

            return;
        }

        ObiettivoGOAP? goalMigliore = SelezionaObiettivoMigliore(agente, citta);

        if (goalMigliore == null)
        {
            // Agente completamente soddisfatto
            agente.ObiettivoCorrente = null;
            return;
        }

        var nuovoPiano = _pianificatore.CalcolaPiano(agente, goalMigliore, azioniDisponibili, citta);

        if (nuovoPiano != null && nuovoPiano.Count > 0)
        {
            agente.ObiettivoCorrente = goalMigliore; 
            agente.PianoCorrente = new Queue<IAzioneGOAP>(nuovoPiano);
        }
    }
}
