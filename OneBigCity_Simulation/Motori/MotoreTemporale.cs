using OneBigCity_Core.Classi.Eventi;

namespace OneBigCity_Simulation.Motori;

public class MotoreTemporale
{
    private readonly EventBus _eventBus;
    private CancellationTokenSource? _cts;

    public DateTime DataCorrente { get; private set; }
    public int INTERVALLO_TICK_MS = 100 * 60; // 1 minuto reale = 1 giorno di gioco
    public bool IsInEsecuzione { get; private set; }

    public MotoreTemporale(EventBus eventBus, DateTime dataInizio)
    {
        _eventBus = eventBus;
        DataCorrente = dataInizio;
        IsInEsecuzione = false;
    }

    public void Avvia()
    {
        if (IsInEsecuzione) return;

        IsInEsecuzione = true;
        _cts = new CancellationTokenSource();
        Task.Run(() => CicloSimulazioneAsync(_cts.Token));
    }

    public void Pausa()
    {
        if (!IsInEsecuzione) return;

        IsInEsecuzione = false;
        _cts?.Cancel();
    }

    private async Task CicloSimulazioneAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            DataCorrente = DataCorrente.AddDays(1);

            _eventBus.Pubblica(new EventoNuovoGiorno(DataCorrente));

            // Attesa tra un tick e l'altro
            try
            {
                await Task.Delay(INTERVALLO_TICK_MS, token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // Gestisce l'interruzione pulita quando viene chiamata la Pausa
                break;
            }
        }
    }
}
