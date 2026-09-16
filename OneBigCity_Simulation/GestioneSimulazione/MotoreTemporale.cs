using OneBigCity_Model.Classi.Eventi;

namespace OneBigCity_Simulation.Motori;

public class MotoreTemporale
{
    private readonly EventBus _eventBus;
    private CancellationTokenSource? _cts;

    public DateTime DataCorrente { get; private set; }
    public TimeSpan StepTemporale { get; set; }
    public int IntervalloTickMs { get; set; } = 1000; // Intervallo reale tra i tick (es. 1 secondo reale)
    public bool IsInEsecuzione { get; private set; }

    public event Action<DateTime>? OnTick; // Evento scatenato ad ogni tick con la nuova data/ora

    public MotoreTemporale(EventBus eventBus, DateTime dataInizio, TimeSpan? stepTemporale = null)
    {
        _eventBus = eventBus;
        DataCorrente = dataInizio;
        StepTemporale = stepTemporale ?? TimeSpan.FromDays(1); // Default 1 giorno, modificabile in seguito
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
            DataCorrente = DataCorrente.Add(StepTemporale);

            if (DataCorrente.Day == 1 && DataCorrente.Month == 1) _eventBus.Pubblica(new EventoSistema("E' iniziato un nuovo anno!"));

            _eventBus.Pubblica(new EventoNuovoGiorno(DataCorrente));

            OnTick?.Invoke(DataCorrente);

            // Attesa reale tra un tick e l'altro
            try
            {
                await Task.Delay(IntervalloTickMs, token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
