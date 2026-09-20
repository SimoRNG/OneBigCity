using OneBigCity_Model.Classi.Entita;

namespace OneBigCity_Simulation.GestioneSimulazione;

public class ContestoTick
{
    public DateTime OrarioCorrente { get; }
    public Citta Citta { get; }

    public ContestoTick(DateTime orarioCorrente, Citta citta)
    {
        OrarioCorrente = orarioCorrente;
        Citta = citta;
    }

}
