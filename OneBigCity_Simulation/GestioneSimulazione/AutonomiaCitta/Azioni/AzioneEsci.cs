using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneEsci : AzioneGOAP
{
    private bool _emesso = false;

    public AzioneEsci(string luogoAttuale)
    {
        Nome = "Esci";
        Costo = 1;

        Precondizioni.Add("Luogo", luogoAttuale);
        Effetti.Add("Luogo", "Citta");
    }

    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta) => true;

    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _emesso = false;
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        if (!_emesso)
        {
            if (agente.EdificioCorrente == null) { Stato = StatoAzione.Fallita; return; }
            buffer.Aggiungi(new ComandoEsciEdificio(agente.ID));
            _emesso = true;
            return;
        }

        Stato = agente.EdificioCorrente == null ? StatoAzione.Completata : StatoAzione.Fallita;
    }
}
