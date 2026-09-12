using OneBigCity_Console;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Meteo.EnumMeteo;
using OneBigCity_Simulation.Motori;
using OneBigCity_Simulation.Stato;


var eventBus = new EventBus();
var logger = new LoggerConsole(eventBus); // Il logger inizia subito ad ascoltare
var meteo = new MotoreMeteo(eventBus, new(TipoMeteo.Sereno, 25));
var motore = new MotoreTemporale(eventBus, new DateTime(2026, 1, 1));

Console.WriteLine("Premere O per caricare i file mappa ed entità - Altrimenti un tasto qualsiasi.");
var tastoCarica = Console.ReadKey(intercept: true).Key;

Citta? cittaCorrente = null;

if (tastoCarica == ConsoleKey.O)
{
    Console.WriteLine("\n--- CARICAMENTO CITTA ---");

    string baseDir = AppContext.BaseDirectory;

    // file Mappa (.txt)
    Console.Write("Inserisci il percorso del file Mappa (.txt). Lasciare vuoto per default: 'PrimaCitta_Mappa.txt' ");
    string? percorsoMappa = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(percorsoMappa))
        percorsoMappa = Path.Combine(baseDir, "PrimaCitta_Mappa.txt");

    // file entità (.json)
    Console.Write("Inserisci il percorso del file Entità (.json). Lasciare vuoto per default: 'PrimaCitta_Entita.json' ");
    string? percorsoEntita = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(percorsoEntita))
        percorsoEntita = Path.Combine(baseDir, "PrimaCitta_Entita.json");

    try
    {
        ICaricatoreMappa caricatoreMappa = new CaricatoreMappaTesto();
        ICaricatoreEntita caricatoreEntita = new CaricatoreEntitaJson();
        InizializzatoreCitta inizializzatore = new InizializzatoreCitta(caricatoreEntita, caricatoreMappa);

        cittaCorrente = inizializzatore.CaricaCitta(percorsoMappa, percorsoEntita, "PrimaCitta");

        Console.WriteLine("Caricamento completato.");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERRORE CARICAMENTO] Impossibile caricare la città: {ex.Message}");
        Console.ResetColor();
    }
}

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