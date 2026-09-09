using OneBigCity_Simulation.DTO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneBigCity_Simulation.Stato;

public class CaricatoreEntitaJson : ICaricatoreEntita
{
    private static readonly JsonSerializerOptions OpzioniJson = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() } // Permette di leggere gli Enum come stringhe (es. "Maschio")
    };

    public EntitaIniziali CaricaEntita(string percorsoFile)
    {
        try
        {
            if (!File.Exists(percorsoFile))
            {
                Console.WriteLine($"[ERROR] File entità non trovato: {percorsoFile}");
                return new EntitaIniziali();
            }

            string jsonTesto = File.ReadAllText(percorsoFile);
            var risultato = JsonSerializer.Deserialize<EntitaIniziali>(jsonTesto, OpzioniJson);

            return risultato ?? new EntitaIniziali();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Errore durante il caricamento delle entità JSON: {ex.Message}");
            return new EntitaIniziali();
        }
    }
}
