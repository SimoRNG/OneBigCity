using OneBigCity_Model.Classi.Eventi;

namespace OneBigCity_Simulation.Motori;

public class MotoreTemporale
{
    private readonly EventBus _eventBus;
    private CancellationTokenSource? _cts;

    public DateTime DataOraCorrente { get; private set; }
    public TimeSpan StepTemporale { get; set; }
    public int IntervalloTickMs { get; set; } = 1000; // Intervallo reale tra i tick
    public bool IsInEsecuzione { get; private set; }

    public event Action<DateTime>? OnTick; // Evento scatenato ad ogni tick con la nuova data/ora

    public MotoreTemporale(EventBus eventBus, DateTime dataInizio, TimeSpan? stepTemporale = null)
    {
        _eventBus = eventBus;
        DataOraCorrente = dataInizio;
        StepTemporale = stepTemporale ?? TimeSpan.FromDays(1); 
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
            DataOraCorrente = DataOraCorrente.Add(StepTemporale);

            if (DataOraCorrente.Day == 1 && DataOraCorrente.Month == 1 
                && DataOraCorrente.Hour == 0 && DataOraCorrente.Minute == 0
                && DataOraCorrente.Second == 0) _eventBus.Pubblica(new EventoSistema("E' iniziato un nuovo anno!"));

            if (DataOraCorrente.TimeOfDay == new TimeSpan(0, 0, 0)) _eventBus.Pubblica(new EventoNuovoGiorno(DataOraCorrente));

            //debug
            _eventBus.Pubblica(new EventoSistema("Ora corrente: " + DataOraCorrente.ToShortTimeString()));

            OnTick?.Invoke(DataOraCorrente);

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
