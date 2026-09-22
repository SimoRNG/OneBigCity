using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.Utilita;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoMuovi(Guid idAgente, Punto3D destinazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        AgenteBase? agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);

        if (agente == null || !agente.PuoMuoversi) return;
        if (agente.Posizione == destinazione) return;

        List<CellaGriglia>? nuovoPercorso = AlgoritmiPathFinding.A_Star(
            citta.Mappa,
            citta.Mappa.OttieniCella(agente.Posizione),
            citta.Mappa.OttieniCella(destinazione)
        );

        if(nuovoPercorso == null) return;

        if (nuovoPercorso.Count > 0)
        {
            agente.Percorso = nuovoPercorso;
            agente.IndiceStepPercorso = 0; // Primo passo della lista
        }
    }
}


public record ComandoAvanzaAgenteStep(Guid idAgente, Punto3D nuovaPosizione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        AgenteBase? agente = citta.Abitanti.FirstOrDefault(a => a.ID == idAgente);
        if (agente != null)
        {
            agente.Posizione = nuovaPosizione;
            agente.IndiceStepPercorso++;

            // Se l'agente ha completato il percorso resetto la sequenza
            if (agente.IndiceStepPercorso >= agente.Percorso.Count)
            {
                agente.Percorso.Clear();
                agente.IndiceStepPercorso = -1;
            }
        }
    }
}
