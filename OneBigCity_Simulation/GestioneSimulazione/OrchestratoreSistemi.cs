using OneBigCity_Model.Classi.Entita;
using OneBigCity_Simulation.GestioneSimulazione;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.GestioneSimulazione.Sistemi;

namespace OneBigCity_Simulation.Motori;

public class OrchestratoreSistemi : IDisposable
{
    private readonly List<ISistemaSimulazione> _sistemi = new();
    public Citta Citta { get; set; }
    private readonly CommandBuffer _commandBuffer = new();
    private DateTime? _ultimoOrario;

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
        if (Citta == null) return;

        // Secondi passati dall'ultimo tick
        double deltaTempoSecondi = 1.0;
        if (_ultimoOrario.HasValue)
        {
            deltaTempoSecondi = (orarioCorrente - _ultimoOrario.Value).TotalSeconds;
        }
        _ultimoOrario = orarioCorrente;

        var contesto = new ContestoTick(orarioCorrente, Citta, deltaTempoSecondi);

        foreach (var sistema in _sistemi)
        {
            sistema.Aggiorna(contesto, _commandBuffer);
        }

        _commandBuffer.ApplicaTutti(Citta);
        _commandBuffer.Svuota();
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
