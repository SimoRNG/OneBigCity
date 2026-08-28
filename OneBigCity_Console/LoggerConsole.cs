using OneBigCity_Core.Classi.Eventi;

namespace OneBigCity_Console;

public class LoggerConsole
{
    private readonly EventBus _eventBus;

    public LoggerConsole(EventBus eventBus)
    {
        _eventBus = eventBus;

        // Sottoscrizione agli eventi
        _eventBus.Iscrivi<EventoNuovoGiorno>(OnNuovoGiorno);

    }

    private void OnNuovoGiorno(EventoNuovoGiorno e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] NUOVO GIORNO SIMULAZIONE: {e.DataCorrente:yyyy-MM-dd}");
        Console.ResetColor();
    }
}
