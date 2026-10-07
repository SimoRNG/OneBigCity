using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoEntraEdificio(Guid idAgente, Guid idEdificio) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);
        var edificio = citta.Edifici.FirstOrDefault(e => e.ID == idEdificio);
        if (agente == null || edificio == null) return;

        if (agente.EdificioCorrente != null) return; // già dentro
        if (!edificio.CoordinateAccessiEsterni.Contains(agente.Posizione)) return; // non è su un accesso
        if (edificio.CoordinateAccessiInterni.Count == 0) return;

        int occupanti = citta.Abitanti.Count(a => a.EdificioCorrente == edificio.ID);
        if (occupanti >= edificio.CapienzaMassima) return; // pieno

        agente.EdificioCorrente = edificio.ID;
        agente.Posizione = edificio.CoordinateAccessiInterni[0];
        agente.Percorso.Clear();
        agente.IndiceStepPercorso = -1;
    }
}

public record ComandoEsciEdificio(Guid idAgente) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);
        if (agente?.EdificioCorrente == null) return;

        var edificio = citta.Edifici.FirstOrDefault(e => e.ID == agente.EdificioCorrente);
        if (edificio == null) return;
        if (!edificio.CoordinateAccessiInterni.Contains(agente.Posizione)) return; // deve essere su un accesso interno

        Punto3D uscita = edificio.CoordinateAccessiEsterni.Count > 0
            ? edificio.CoordinateAccessiEsterni[0]
            : Utilita.AlgoritmiPathFinding.CercaPrimaCellaCalpestabileBFS(citta.Mappa, edificio.CoordinateOccupate[0]);

        agente.EdificioCorrente = null;
        agente.Posizione = uscita;
        agente.Percorso.Clear();
        agente.IndiceStepPercorso = -1;
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
