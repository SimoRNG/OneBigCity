using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Model.Classi.Geografia.EnumGeografia;

namespace OneBigCity_Simulation.Stato;

public class CaricatoreMappaTesto : ICaricatoreMappa
{
    private static readonly Dictionary<string, TipoCella> MappaturaSimboli = new()
    {
        { "T", TipoCella.Terreno },
        { "S", TipoCella.Strada },
        { "E", TipoCella.Edificio },
        { "A", TipoCella.Acqua  },
        { "P", TipoCella.Ponte },
        { "V", TipoCella.Vuoto },
        { "G", TipoCella.Pianta },
        { "U", TipoCella.Sottosuolo },
        { "R", TipoCella.Rampa },
        { "O",TipoCella.Ostacolo }
    };

    public MappaGriglia CaricaMappa(string percorsoFile)
    {
        // Legge tutte le righe del file .txt
        // Parsa ogni elemento separato da un separatore
        // Popola le celle del Layer 0 nella MappaGriglia
        // Restituisce la mappa pronta

        try
        {
            if (!File.Exists(percorsoFile))
            {
                Console.WriteLine($"[ERROR] File mappa non trovato: {percorsoFile}");
                return new MappaGriglia();
            }

            string[] righe = File.ReadAllLines(percorsoFile);

            if (righe.Length == 0)
                return new MappaGriglia();

            int dimY = righe.Length; // Y = Righe del file

            var righeParsate = righe
                .Select(riga => riga.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .ToArray())
                .ToList();

            int dimX = righeParsate.Max(riga => riga.Length); // X = Colonne del file

            // Inizializza MappaGriglia(lunghezza = dimX, larghezza = dimY)
            MappaGriglia risultato = new MappaGriglia(dimX, dimY);

            // Popolamento layer 0 (Z = 0)
            for (int y = 0; y < dimY; y++)
            {
                string[] elementiRiga = righeParsate[y];

                for (int x = 0; x < dimX; x++)
                {
                    TipoCella tipo = TipoCella.Vuoto;

                    if (x < elementiRiga.Length)
                    {
                        string simbolo = elementiRiga[x];
                        if (!MappaturaSimboli.TryGetValue(simbolo, out tipo))
                        {
                            tipo = TipoCella.Vuoto;
                        }
                    }

                    Punto3D posizione = new Punto3D(x, y, 0);
                    bool calpestabile = tipo is TipoCella.Strada or TipoCella.Terreno or TipoCella.Ponte;

                    CellaGriglia nuovaCella = new CellaGriglia(posizione, tipo, calpestabile);
                    risultato.ImpostaCella(nuovaCella, posizione);
                }
            }

            return risultato;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Errore durante il caricamento della mappa: {ex.Message}");
            return new MappaGriglia();
        }
    }
}
