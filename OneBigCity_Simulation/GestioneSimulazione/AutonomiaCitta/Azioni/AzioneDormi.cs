using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Interfacce;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneDormi : AzioneGOAP, IAzioneGOAP
{
    private double _secondiTrascorsi = 0;
    public AzioneDormi()
    {
        Nome = "Dormi";
        DurataInSecondiSimulazione = 21600; // 6 ore

        // PRECONDIZIONI
        Precondizioni.Add("Luogo", "Casa");

        // EFFETTI FISSI
        Effetti.Add("EnergiaSoddisfatta", true);

    }
    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _secondiTrascorsi = 0;
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        if (!LuoghiAgente.IsInCasa(agente, contesto.Citta)) { Stato = StatoAzione.Fallita; return; }

        _secondiTrascorsi += contesto.DeltaTempoSecondi;
        if (_secondiTrascorsi < DurataInSecondiSimulazione) return;

        // Recupero tutta l'energia solo alla fine
        buffer.Aggiungi(new ComandoVariaBisogno(agente.ID, Bisogno.Energia, 100));
        Stato = StatoAzione.Completata;
    }
    public override double CalcolaCosto(AgenteBase agente, Citta citta)
    {
        return 1;
    }


    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta)
    {
        return true;
    }
}
