using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneEntra : AzioneGOAP
{
    private readonly string _luogo;
    private Guid _idEdificio;
    private bool _emesso = false;

    public AzioneEntra(string luogo)
    {
        _luogo = luogo;
        Nome = $"Entra_{luogo}";
        Costo = 1;

        Precondizioni.Add("Luogo", "Citta");
        Precondizioni.Add($"VicinoA_{luogo}", true);
        Effetti.Add("Luogo", luogo);
        Effetti.Add($"VicinoA_{luogo}", false);
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        if (!_emesso)
        {
            var edificio = LuoghiAgente.GetEdificioVicino(agente, contesto.Citta, _luogo);
            if (edificio == null) { Stato = StatoAzione.Fallita; return; }

            _idEdificio = edificio.ID;
            buffer.Aggiungi(new ComandoEntraEdificio(agente.ID, _idEdificio));
            _emesso = true;
            return;
        }

        Stato = agente.EdificioCorrente == _idEdificio ? StatoAzione.Completata : StatoAzione.Fallita;
    }

    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta) => true;

    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _emesso = false;
    }

}
