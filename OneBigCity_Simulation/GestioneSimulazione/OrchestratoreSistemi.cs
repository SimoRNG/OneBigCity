using OneBigCity_Model.Classi.Entita;
using OneBigCity_Simulation.GestioneSimulazione.Sistemi;

namespace OneBigCity_Simulation.Motori;

public class OrchestratoreSistemi: IDisposable
{
    private readonly List<ISistemaSimulazione> _sistemi = new();
    public Citta Citta { get; set; }

    public OrchestratoreSistemi(Citta citta)
    {
        Citta = citta;
    }

    public void RegistraSistema(ISistemaSimulazione sistema)
    {
        // L'ordine di registrazione determina l'ordine di aggiornamento

        if (!_sistemi.Contains(sistema))
        {
            _sistemi.Add(sistema);
        }
    }

    public void EseguiTick(DateTime orarioCorrente)
    {
        // Aggiorno i sistemi

        if (Citta == null) return;

        foreach (var sistema in _sistemi)
        {
            sistema.Aggiorna(orarioCorrente, Citta);
        }
    }

    public void Dispose()
    {
        // Faccio il dispose di ogni sistema 
        // I sistemi implementeranno l'interfaccia per disiscriversi dall'eventbus (qualora stiano ascoltando un qualche evento)
        // Es. il sistema meteo

        foreach (var sistema in _sistemi)
        {
            if (sistema is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _sistemi.Clear();
    }
}
