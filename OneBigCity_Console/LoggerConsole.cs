using OneBigCity_Core.Classi.Eventi;

namespace OneBigCity_Console;

public class LoggerConsole
{
    private readonly EventBus _eventBus;

    public LoggerConsole(EventBus eventBus)
    {
        _eventBus = eventBus;

        // Sottoscrizione agli eventi
        _eventBus.Iscrivi<EventoMeteoCambiato>(OnMeteoCambiato);
        _eventBus.Iscrivi<EventoNuovoGiorno>(OnNuovoGiorno);
        _eventBus.Iscrivi<EventoSistema>(OnLogSistema);

    }

    private void OnLogSistema(EventoSistema e)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[SISTEMA] {e.Log})");
        Console.ResetColor();
    }

    private void OnMeteoCambiato(EventoMeteoCambiato e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[METEO] Il tempo è cambiato. Meteo corrente: {e.Nuova})");
        Console.ResetColor();
    }

    private void OnNuovoGiorno(EventoNuovoGiorno e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[TEMPO] NUOVO GIORNO SIMULAZIONE: {e.DataCorrente:yyyy-MM-dd}");
        Console.ResetColor();
    }
}
