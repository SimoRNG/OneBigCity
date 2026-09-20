using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoEntraEdificio(Guid idAgente, Guid idEdificio) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var edificio = citta.Edifici.FirstOrDefault(e => e.ID == idEdificio);
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);

        if (edificio != null && agente != null)
        {
            if (edificio.CapienzaMassima != edificio.OccupantiAttuali.Count()) edificio.OccupantiAttuali.Add(idAgente);

            // Tolgo l'agente dalla mappa esterna (TODO - gli agenti dovrebbero tenere da qualche parte in memoria che
            // sono all'interno di un edificio?)

            agente.Posizione = new Punto3D(0, 0, 0);
        }
    }
}


public record ComandoEsciEdificio(Guid idAgente, Guid idEdificio) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var edificio = citta.Edifici.FirstOrDefault(e => e.ID == idEdificio);
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);

        if (edificio != null && agente != null)
        {
            if (edificio.CapienzaMassima != edificio.OccupantiAttuali.Count()) edificio.OccupantiAttuali.Remove(idAgente);

            // Posiziono su una cella della griglia esterna all'edifico (se esiste) l'agente. Se non ci sono celle disponibili
            // lo metto in una cella valida adiacente all'edificio
            if (edificio.CoordinateAccessiEsterni.Count() > 0)
            {
                Punto3D accesso = edificio.CoordinateAccessiEsterni[0];
                agente.Posizione = new Punto3D(accesso.X, accesso.Y, accesso.Z);
            }
            else
            {
                Punto3D adiacente = Utilita.AlgoritmiPathFinding
                    .CercaPrimaCellaCalpestabileBFS(citta.Mappa, edificio.CoordinateOccupate[0]);

                agente.Posizione = adiacente;
            }
        }
    }
}



public record ComandoVariaCondizioneEdificio(Guid idEdificio, int variazionePercentuale) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var edificio = citta.Edifici.FirstOrDefault(e => e.ID == idEdificio);
        if (edificio != null)
        {
            edificio.PercentualeCondizione += variazionePercentuale;

            if (edificio.PercentualeCondizione > 100) edificio.PercentualeCondizione = 100;
            if (edificio.PercentualeCondizione < 0) edificio.PercentualeCondizione = 0;
        }
    }
}
