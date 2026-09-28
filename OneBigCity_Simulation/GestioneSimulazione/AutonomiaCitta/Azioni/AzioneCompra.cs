using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Interfacce;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneCompra : AzioneGOAP, IAzioneGOAP
{
    private readonly double _costoOggetto;
    private double _secondiTrascorsi = 0;
    private readonly Risorsa _oggettoAcquistato;
    private readonly double _qtaAcquistata;
    public AzioneCompra(Risorsa oggettoAcquisto, double quantitaAcquistata, double costo)
    {
        Nome = $"Acquista_{oggettoAcquisto.ToString()}";
        Costo = 1.0; // Costo di pianificazione, non denaro
        DurataInSecondiSimulazione = 10; // Dura 10 secondi virtuali

        _costoOggetto = costo;
        _oggettoAcquistato = oggettoAcquisto;
        _qtaAcquistata = quantitaAcquistata;

        Effetti.Add($"HaRisorsa_{oggettoAcquisto}", true);
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

        if (!agente.Inventario.TryGetValue(Risorsa.Denaro, out double saldo) || saldo < _costoOggetto)
        {
            Stato = StatoAzione.Fallita;
            return;
        }

        buffer.Aggiungi(new ComandoVariaInventario(agente.ID, Risorsa.Denaro, -_costoOggetto));
        buffer.Aggiungi(new ComandoVariaInventario(agente.ID, _oggettoAcquistato, _qtaAcquistata));
        Stato = StatoAzione.Completata;
    }

    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta)
    {
        // Verifica denaro
        if (agente.Inventario.TryGetValue(Risorsa.Denaro, out double saldoAttuale))
        {
            if (saldoAttuale < _costoOggetto) return false;
        }
        else
        {
            return false;
        }

        // TODO - Verificare se si trova in un negozio

        return true;
    }
}
