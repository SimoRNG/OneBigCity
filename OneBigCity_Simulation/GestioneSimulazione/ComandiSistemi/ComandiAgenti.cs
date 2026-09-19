using OneBigCity_Model.Classi.Entita;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoEliminaPersona(Guid idPersona) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        citta.Abitanti.RemoveAll(p => p.ID == idPersona);
    }
}