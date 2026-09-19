using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoRichiediMovimento(Guid idPersona, Punto3D destinazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
         var richiedente = citta.Abitanti.Where(p => p.ID == idPersona).FirstOrDefault();
        // TODO
    }
}
