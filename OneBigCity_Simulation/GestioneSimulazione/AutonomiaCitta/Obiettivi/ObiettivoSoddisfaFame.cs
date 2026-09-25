using OneBigCity_Model.AutonomiaCitta.Obiettivi;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;

namespace OneBigCity_Simulation.GestioneSimulazione.Obiettivi;

public class ObiettivoSoddisfaFame : ObiettivoGOAP
{
    public ObiettivoSoddisfaFame()
    {
        Nome = "SoddisfaFame";
        StatoDesiderato.Add("FameSoddisfatta", true);
    }
    public override float CalcolaPriorita(AgenteBase agente, Citta citta)
    {
        if (agente.Bisogni.TryGetValue(Bisogno.Fame, out double livelloFame))
        {
            // Più la fame è bassa più la priorità dell'obiettivo deve essere alta
            return (float)(100 - livelloFame);
        }
        return 0f;
    }

    public override bool IsSoddisfatto(AgenteBase agente, Citta citta)
    {
        // Se la fame è superiore a 80% considero il goal raggiunto
        if (agente.Bisogni.TryGetValue(Bisogno.Fame, out double livelloFame))
        {
            return livelloFame >= 80;
        }
        return true;
    }
}
