using OneBigCity_Model.Classi.Geografia.EnumGeografia;

namespace OneBigCity_Model.Classi.Geografia;

public class MappaGriglia
{
    public int Larghezza { get; } // n nelle matrici
    public int Lunghezza { get; } // m nelle matrici
    public int LivelloMinimo { get; }
    public int LivelloMassimo { get; }

    // Dizionario che associa a ogni indice di Livello (es. -1, 0, 1) una matrice 2D di Celle [X, Y]
    private readonly Dictionary<int, CellaGriglia[,]> _strati;

    public MappaGriglia(int lunghezza = 10, int larghezza = 10, int livelloMinimo = -3, int livelloMassimo = 3)
    {
        Lunghezza = lunghezza;
        Larghezza = larghezza;

        LivelloMinimo = livelloMinimo;
        LivelloMassimo = livelloMassimo;

        _strati = new Dictionary<int, CellaGriglia[,]>();

        InizializzaMappa();
    }

    private void InizializzaMappa()
    {
        for (int z = LivelloMinimo; z <= LivelloMassimo; z++)
        {
            var matriceLivello = new CellaGriglia[Lunghezza, Larghezza];

            for (int x = 0; x < Lunghezza; x++)
            {
                for (int y = 0; y < Larghezza; y++)
                {
                    Punto3D pos = new Punto3D(x, y, z);

                    // Valore di default in base al livello
                    TipoCella tipoDefault = z switch
                    {
                        < 0 => TipoCella.Sottosuolo,
                        0 => TipoCella.Terreno,
                        _ => TipoCella.Vuoto
                    };

                    bool calpestabileDefault = tipoDefault == TipoCella.Terreno;

                    matriceLivello[x, y] = new CellaGriglia(pos, tipoDefault, calpestabileDefault);
                }
            }

            _strati.Add(z, matriceLivello);
        }
    }

    /// <summary>
    /// Recupera la cella alle coordinate specificate.
    /// </summary>
    public CellaGriglia? OttieniCella(Punto3D posizione)
    {
        if (!IsPosizioneValida(posizione))
            return null;

        return _strati[posizione.Z][posizione.X, posizione.Y];
    }

    /// <summary>
    /// Verifica se una posizione si trova nei limiti della mappa.
    /// </summary>
    public bool IsPosizioneValida(Punto3D posizione)
    {
        return posizione.X >= 0 && posizione.X < Lunghezza &&
               posizione.Y >= 0 && posizione.Y < Larghezza &&
               posizione.Z >= LivelloMinimo && posizione.Z <= LivelloMassimo;
    }

    public bool ImpostaCella(CellaGriglia cella, Punto3D posizione)
    {
        if (!IsPosizioneValida(cella.Posizione)) return false;

        _strati[posizione.Z][posizione.X, posizione.Y] = cella;

        return true;
    }
    public bool ImpostaCella(int x, int y, int z, TipoCella tipo, bool calpestabile = true)
    {
        Punto3D pos = new Punto3D(x, y, z);

        if (!IsPosizioneValida(pos))
            return false;

        _strati[z][x, y] = new CellaGriglia(pos, tipo, calpestabile);
        return true;
    }

    public IEnumerable<CellaGriglia> GetVicini(CellaGriglia cella)
    {
        // Direzioni
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = cella.Posizione.X + dx[i];
            int ny = cella.Posizione.Y + dy[i];
            int z = cella.Posizione.Z; // TODO - per il momento mi tengo sullo stesso piano z, da cambiare poi

            CellaGriglia? vicina = OttieniCella(new Punto3D(nx, ny, z));

            if (vicina != null && vicina.IsCalpestabile)
            {
                yield return vicina;
            }
        }
    }
}
