using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public record ComandoEliminaPersona(Guid idPersona) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        citta.Abitanti.RemoveAll(p => p.ID == idPersona);
    }
}

public record ComandoVariaBisogno(Guid idAgente, Bisogno bisogno, double variazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(p => p.ID == idAgente);
        if (agente == null) return;

        agente.Bisogni.TryGetValue(bisogno, out double attuale);
        agente.Bisogni[bisogno] = Math.Clamp(attuale + variazione, 0, 100);
    }
}

public record ComandoVariaInventario(Guid idAgente, Risorsa risorsa, double variazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.FirstOrDefault(p => p.ID == idAgente);
        if (agente == null) return;

        agente.Inventario.TryGetValue(risorsa, out double attuale);
        agente.Inventario[risorsa] = Math.Max(0, attuale + variazione);
    }
}