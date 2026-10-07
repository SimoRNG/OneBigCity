using OneBigCity_Console;
using OneBigCity_Model.AutonomiaCitta.Obiettivi;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Meteo.EnumMeteo;
using OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Obiettivi;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;
using OneBigCity_Simulation.GestioneSimulazione.Obiettivi;
using OneBigCity_Simulation.GestioneSimulazione.Sistemi;
using OneBigCity_Simulation.Motori;
using OneBigCity_Simulation.Stato;

Console.WriteLine("Premere O per caricare i file mappa ed entità - Altrimenti un tasto qualsiasi.");
var tastoCarica = Console.ReadKey(intercept: true).Key;

Citta? cittaCorrente = new();

if (tastoCarica == ConsoleKey.O) cittaCorrente = CaricaCitta();

var eventBus = new EventBus();
var logger = new LoggerConsole(eventBus); // Il logger inizia ad ascoltare subito

// Creazione dell'orchestratore (con la città caricata o vuota)
var orchestratore = new OrchestratoreSistemi(cittaCorrente);

// SISTEMI INSERITI NELLA PIPELINE
RegistraSistemi(eventBus, orchestratore);
// -----

var motoreTemporale = new MotoreTemporale(eventBus, new DateTime(2026, 1, 1), TimeSpan.FromMinutes(60)); // 1 ora per tick
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


return 0;


void RegistraSistemi(EventBus eventBus, OrchestratoreSistemi orchestratore)
{

    var gestoreUtility = new GestoreUtilityAgenti(GetObiettiviDisponibili());
    var azioniDisponibili = GetAzioniDisponibili();

    var sistemaMeteo = new SistemaMeteo(eventBus, new(TipoMeteo.Sereno, 25));
    var sistemaEdifici = new SistemaEdifici(eventBus);
    var sistemaAgenti = new SistemaAgenti(eventBus, gestoreUtility, azioniDisponibili);
    var sistemaMovimento = new SistemaMovimento(eventBus);

    orchestratore.RegistraSistema(sistemaMeteo);
    eventBus.Pubblica(new EventoSistema("Sistema meteo registrato."));
    orchestratore.RegistraSistema(sistemaEdifici);
    eventBus.Pubblica(new EventoSistema("Sistema edifici registrato."));
    orchestratore.RegistraSistema(sistemaAgenti);
    eventBus.Pubblica(new EventoSistema("Sistema agenti registrato."));
    orchestratore.RegistraSistema(sistemaMovimento);
    eventBus.Pubblica(new EventoSistema("Sistema movimento registrato."));

}

List<ObiettivoGOAP> GetObiettiviDisponibili()
{
    return new()
    {
        new ObiettivoSoddisfaFame(),
        new ObiettivoRecuperaEnergia()
    };
}

List<AzioneGOAP> GetAzioniDisponibili()
{
    return new()
    {
        new AzioneMangia(60),
        new AzioneCompra(Risorsa.Cibo, 10, 2),
        new AzioneDormi(),
        new AzioneVaiA("Negozio"),
        new AzioneVaiA("Casa"),
        new AzioneVaiA("Lavoro"),
        new AzioneEntra("Negozio"),
        new AzioneEntra("Casa"),
        new AzioneEntra("Lavoro"),
        new AzioneEsci("Negozio"),
        new AzioneEsci("Lavoro"),
        new AzioneEsci("Casa"),
    };
}


Citta CaricaCitta()
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

        return inizializzatore.CaricaCitta(percorsoMappa, percorsoEntita, "PrimaCitta");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERRORE CARICAMENTO] Impossibile caricare la città: {ex.Message}");
        Console.ResetColor();

        return new();
    }
}