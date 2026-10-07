using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Interfacce;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.Azioni;


public abstract class AzioneGOAP : IAzioneGOAP, ICloneable
{
    public string Nome { get; protected set; } = "AzioneGenerica";
    public Dictionary<string, object> Precondizioni { get; set; } = new();
    public Dictionary<string, object> Effetti { get; set; } = new();
    public double Costo { get; set; }
    public long DurataInSecondiSimulazione { get; set; }
    public StatoAzione Stato { get; set; }

    public abstract bool VerificaPrecondizioni(AgenteBase agente, Citta citta);
    public abstract void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer);
    public virtual double CalcolaCosto(AgenteBase agente, Citta citta) => Costo;
    public virtual Dictionary<string, object> CalcolaEffetti(AgenteBase agente, Citta citta) => Effetti;


    public virtual void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        Stato = StatoAzione.InCorso;
    }

    public virtual void Termina(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        Stato = StatoAzione.Completata;
    }

    public virtual void Annulla(AgenteBase agente)
    {
        Stato = StatoAzione.Fallita;
    }

    public virtual void Sospendi(AgenteBase agente)
    {
        Stato = StatoAzione.Sospesa;
    }

    public object Clone()
    {
        var copia = (AzioneGOAP)this.MemberwiseClone();
        copia.Stato = StatoAzione.NonIniziata;
        return copia;
    }
}

public enum StatoAzione
{
    NonIniziata,
    InCorso,
    Sospesa,
    Completata,
    Fallita
}


