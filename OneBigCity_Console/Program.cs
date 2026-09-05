using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Console;
using OneBigCity_Simulation.Motori;
using OneBigCity_Model.Classi.Meteo.EnumMeteo;


var eventBus = new EventBus();
var logger = new LoggerConsole(eventBus); // Il logger inizia subito ad ascoltare
var meteo = new MotoreMeteo(eventBus, new(TipoMeteo.Sereno, 25));
var motore = new MotoreTemporale(eventBus, new DateTime(2026, 1, 1));


Console.WriteLine("--- SIMULAZIONE CITTÀ AVVIATA ---");
Console.WriteLine("Premi 'P' per Pausa/Avvio, 'Q' per uscire.\n");


motore.Avvia();

// Gestione input utente
while (true)
{
    var tasto = Console.ReadKey(intercept: true).Key;

    if (tasto == ConsoleKey.P)
    {
        if (motore.IsInEsecuzione)
        {
            motore.Pausa();
            Console.WriteLine("\n[SISTEMA] Simulazione in PAUSA.");
        }
        else
        {
            motore.Avvia();
            Console.WriteLine("\n[SISTEMA] Simulazione RIPRESA.");
        }
    }
    else if (tasto == ConsoleKey.Q)
    {
        motore.Pausa();
        Console.WriteLine("\n[SISTEMA] Chiusura simulazione...");
        break;
    }
}