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

public record ComandoVariaBisogno(Guid idPersona, Bisogno bisogno, double variazione) : IComandoSimulazione
{
    public void Esegui(Citta citta)
    {
        var agente = citta.Abitanti.Where(p => p.ID == idPersona).FirstOrDefault();
        if (agente is Persona persona)
        {
            persona.Bisogni[bisogno] += variazione;
            if (persona.Bisogni[bisogno] < 0) persona.Bisogni[bisogno] = 0;
            if (persona.Bisogni[bisogno] > 100) persona.Bisogni[bisogno] = 100;
        }
    }
}