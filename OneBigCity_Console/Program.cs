using OneBigCity_Console;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Meteo.EnumMeteo;
using OneBigCity_Simulation.Motori;
using OneBigCity_Simulation.Stato;

Console.WriteLine("Premere O per caricare i file mappa ed entità - Altrimenti un tasto qualsiasi.");
var tastoCarica = Console.ReadKey(intercept: true).Key;

Citta? cittaCorrente = new();

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

        cittaCorrente = new();
    }
}

var eventBus = new EventBus();
var logger = new LoggerConsole(eventBus); // Il logger inizia ad ascoltare subito

var motoreMeteo = new SistemaMeteo(eventBus, new(TipoMeteo.Sereno, 25));

// Creazione dell'orchestratore (con la città caricata o vuota)
var orchestratore = new OrchestratoreSistemi(cittaCorrente);

// SISTEMI INSERITI NELLA PIPELINE
orchestratore.RegistraSistema(motoreMeteo);
// -----

var motoreTemporale = new MotoreTemporale(eventBus, new DateTime(2026, 1, 1), TimeSpan.FromDays(1)); // 1 giorno per tick
motoreTemporale.OnTick += orchestratore.EseguiTick;


// Avvio della simulazione
Console.WriteLine("--- SIMULAZIONE CITTÀ AVVIATA ---");
Console.WriteLine("Premi 'P' per Pausa/Avvio, 'Q' per uscire.\n");
motoreTemporale.Avvia();



// Gestione input utente
while (true)
{
    var tasto = Console.ReadKey(intercept: true).Key;

    if (tasto == ConsoleKey.P)
    {
        if (motoreTemporale.IsInEsecuzione)
        {
            motoreTemporale.Pausa();
            Console.WriteLine("\n[SISTEMA] Simulazione in PAUSA.");
        }
        else
        {
            motoreTemporale.Avvia();
            Console.WriteLine("\n[SISTEMA] Simulazione RIPRESA.");
        }
    }
    else if (tasto == ConsoleKey.Q)
    {
        motoreTemporale.Pausa();
        Console.WriteLine("\n[SISTEMA] Chiusura simulazione...");
        break;
    }
}