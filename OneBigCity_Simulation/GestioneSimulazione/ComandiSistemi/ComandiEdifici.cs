using OneBigCity_Model.Classi.Entita;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoEntraEdificio(Guid idAgente, Guid idEdificio) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var edificio = citta.Edifici.Where(e=>e.ID == idEdificio).FirstOrDefault();
        if(edificio != null)
        {
            if (edificio.CapienzaMassima != edificio.OccupantiAttuali.Count()) edificio.OccupantiAttuali.Append(idAgente);
        }
    }
}
