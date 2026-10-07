using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneVaiA : AzioneGOAP
{
    private readonly string _luogo;
    private Punto3D _destinazione;
    private int _aggiornamenti = 0;

    public AzioneVaiA(string luogo)
    {
        _luogo = luogo;
        Nome = $"VaiA_{luogo}";
        Costo = 1;

        Precondizioni.Add("Luogo", "Citta"); // si cammina solo all'aperto per ora
        Effetti.Add($"VicinoA_{luogo}", true);
    }

    public override bool VerificaPrecondizioni(AgenteBase agente, Citta citta)
        => agente.PuoMuoversi && TrovaAccessoPiuVicino(agente, citta) != null;

    public override double CalcolaCosto(AgenteBase agente, Citta citta)
    {
        var accesso = TrovaAccessoPiuVicino(agente, citta);
        if (accesso == null) return 1000;

        // Costo = numero di tick necessari (più base fissa)
        double tick = Math.Ceiling((double)Distanza(agente.Posizione, accesso.Value) / Math.Max(1, agente.VelocitaMovimento));
        return 1 + tick;
    }

    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _aggiornamenti = 0;

        var accesso = TrovaAccessoPiuVicino(agente, citta);
        if (accesso == null) { Stato = StatoAzione.Fallita; return; }

        _destinazione = accesso.Value;
        buffer.Aggiungi(new ComandoMuovi(agente.ID, _destinazione)); // una sola volta
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        if (Stato == StatoAzione.Fallita) return;

        if (agente.EdificioCorrente != null) { Stato = StatoAzione.Fallita; return; }

        if (agente.Posizione == _destinazione) { Stato = StatoAzione.Completata; return; }

        // Il percorso viene creato dal comando a fine tick: al primo Aggiorna non c'è ancora
        // Se dal secondo in poi non c'è percorso e non sono arrivato, A* non ha trovato nulla
        _aggiornamenti++;
        if (_aggiornamenti >= 2 && agente.Percorso.Count == 0) Stato = StatoAzione.Fallita;
    }

    private Punto3D? TrovaAccessoPiuVicino(AgenteBase agente, Citta citta)
    {
        Punto3D? migliore = null;
        int distMin = int.MaxValue;

        foreach (var edificio in LuoghiAgente.TrovaEdifici(_luogo, agente, citta))
            foreach (var accesso in edificio.CoordinateAccessiEsterni)
            {
                int d = Distanza(agente.Posizione, accesso);
                if (d < distMin) { distMin = d; migliore = accesso; }
            }

        return migliore;
    }

    private static int Distanza(Punto3D a, Punto3D b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
}
