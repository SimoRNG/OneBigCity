using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneMangia : AzioneGOAP
{
    private double _secondiTrascorsi = 0;
    private readonly double _incrementoFame;
    public AzioneMangia(double incrementoFame)
    {
        Nome = "MangiaCibo";
        DurataInSecondiSimulazione = 10; // Dura 10 secondi virtuali
        _incrementoFame = incrementoFame;
        Costo = 1;

        // PRECONDIZIONI
        Precondizioni.Add("HaRisorsa_Cibo", true);

        // EFFETTI
        Effetti.Add("FameSoddisfatta", true);
    }

    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta)
    {
        return true;
    }

    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _secondiTrascorsi = 0;
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        _secondiTrascorsi += contesto.DeltaTempoSecondi;
        if (_secondiTrascorsi < DurataInSecondiSimulazione) return;

        // Controllo sull'inventario reale al momento di mangiare
        if (!agente.Inventario.TryGetValue(Risorsa.Cibo, out double cibo) || cibo < 1)
        {
            Stato = StatoAzione.Fallita;
            return;
        }

        buffer.Aggiungi(new ComandoVariaInventario(agente.ID, Risorsa.Cibo, -1));
        buffer.Aggiungi(new ComandoVariaBisogno(agente.ID, Bisogno.Fame, _incrementoFame));
        Stato = StatoAzione.Completata;
    }

}