using OneBigCity_Model.Classi.Entita;

namespace OneBigCity_Simulation.GestioneSimulazione;

public class ContestoTick
{
    public DateTime OrarioCorrente { get; }
    public Citta Citta { get; }
    public double DeltaTempoSecondi { get; }

    public ContestoTick(DateTime orarioCorrente, Citta citta, double deltaTempoSecondi = 1)
    {
        OrarioCorrente = orarioCorrente;
        Citta = citta;
        DeltaTempoSecondi = deltaTempoSecondi;
    }

}
