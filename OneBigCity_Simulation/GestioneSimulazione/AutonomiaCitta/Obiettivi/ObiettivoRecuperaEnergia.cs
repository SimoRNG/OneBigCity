using OneBigCity_Model.AutonomiaCitta.Obiettivi;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Interfacce;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Obiettivi;

public class ObiettivoRecuperaEnergia : ObiettivoGOAP, IObiettivoGOAP
{

    public ObiettivoRecuperaEnergia()
    {
        Nome = "RecuperaEnergia";
        StatoDesiderato.Add("EnergiaSoddisfatta", true);    
    }

    public override double CalcolaPriorita(AgenteBase agente, Citta citta)
    {
        // Più è bassa l'energia dell'agente più è importante
        // Il controllo inizia solo quando l'agente inizia ad avere "sonno" (sotto il 25%)

        agente.Bisogni.TryGetValue(Bisogno.Energia, out double energia);
        if(energia < 25)
        {
            return 100 - energia;
        }
        else return 0;
    }

    public override bool IsSoddisfatto(AgenteBase agente, Citta citta)
    {
        // Se superiore a 80% goal raggiunto
        if (agente.Bisogni.TryGetValue(Bisogno.Energia, out double livello))
        {
            return livello >= 80;
        }
        return true;
    }
}
