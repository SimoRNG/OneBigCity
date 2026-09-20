using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public class SistemaMovimento :ISistemaSimulazione
{
    private readonly EventBus _eventBus;
    public SistemaMovimento(EventBus eventBus) { _eventBus = eventBus; }


    public void Aggiorna(ContestoTick contesto, CommandBuffer buffer)
    {
        AggiornaMovimentoAgenti(contesto.Citta, buffer);
    }

    public void Dispose()
    {

    }


    private void AggiornaMovimentoAgenti(Citta citta, CommandBuffer buffer)
    {
        foreach (var agente in citta.Abitanti)
        {
            // Verifio se l'agente può muoversi e se ha un percorso attivo non ancora terminato
            if (agente.PuoMuoversi &&
                agente.Percorso.Count > 0 &&
                agente.IndiceStepPercorso >= 0 &&
                agente.IndiceStepPercorso < agente.Percorso.Count)
            {
                // Prossima posizione lungo il percorso
                Punto3D prossimaPosizione = agente.Percorso[agente.IndiceStepPercorso];

                // Comando per spostare atomicamente l'agente sulla nuova cella
                buffer.Aggiungi(new ComandoAvanzaAgenteStep(agente.ID, prossimaPosizione));
            }
        }
    }
}
