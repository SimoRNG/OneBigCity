using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.Utilita;

public record ComandoMuovi(Guid idAgente, Punto3D destinazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);
        if (agente == null || !agente.PuoMuoversi) return;

        MappaGriglia? mappa = citta.Mappa;
        if (agente.EdificioCorrente != null)
        {
            mappa = citta.Edifici.FirstOrDefault(e => e.ID == agente.EdificioCorrente)?.PlanimetriaInterna;
            if (mappa == null) return;
        }

        var percorso = AlgoritmiPathFinding.A_Star(
            mappa,
            mappa.OttieniCella(agente.Posizione),
            mappa.OttieniCella(destinazione));

        // Nessun percorso oppure già a destinazione (A* restituisce solo la partenza)
        if (percorso == null || percorso.Count <= 1)
        {
            agente.Percorso.Clear();
            agente.IndiceStepPercorso = -1;
            return;
        }

        agente.Percorso = percorso;
        agente.IndiceStepPercorso = 1; // lo 0 è la cella dove si trova già
    }
}

public record ComandoAvanzaAgenteStep(Guid idAgente, int passi) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);
        if (agente == null) return;
        if (agente.IndiceStepPercorso < 0 || agente.IndiceStepPercorso >= agente.Percorso.Count) return;

        int fine = Math.Min(agente.IndiceStepPercorso + passi, agente.Percorso.Count); // escluso
        agente.Posizione = agente.Percorso[fine - 1].Posizione;
        agente.IndiceStepPercorso = fine;

        if (fine >= agente.Percorso.Count)
        {
            agente.Percorso.Clear();
            agente.IndiceStepPercorso = -1;
        }
    }
}