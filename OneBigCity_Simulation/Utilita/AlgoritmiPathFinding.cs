using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.Utilita;

public static class AlgoritmiPathFinding
{
    private static readonly (int dx, int dy)[] DirezioniCardinali = new[]
    {
        (0, -1), // Nord
        (1, 0),  // Est
        (0, 1),  // Sud
        (-1, 0)  // Ovest
    };


    #region BFS
    public static Punto3D CercaPrimaCellaCalpestabileBFS(MappaGriglia mappa, CellaGriglia partenza)
    {
        return CercaPrimaCellaCalpestabileBFS(mappa, partenza.Posizione);
    }

    public static Punto3D CercaPrimaCellaCalpestabileBFS(MappaGriglia mappa, Punto3D partenza)
    {
        // Se la cella di partenza è già valida e calpestabile la restituisce
        CellaGriglia? cellaIniziale = mappa.OttieniCella(partenza);
        if (cellaIniziale != null && cellaIniziale.IsCalpestabile)
        {
            return partenza;
        }

        Queue<Punto3D> coda = new();
        HashSet<Punto3D> visitati = new();

        coda.Enqueue(partenza);
        visitati.Add(partenza);

        while (coda.Count > 0)
        {
            Punto3D corrente = coda.Dequeue();

            // Esploro la i 4 vicini sul medesimo piano (Z)
            foreach (var (dx, dy) in DirezioniCardinali)
            {
                Punto3D vicino = new Punto3D(corrente.X + dx, corrente.Y + dy, corrente.Z);

                if (!mappa.IsPosizioneValida(vicino) || visitati.Contains(vicino))
                    continue;

                CellaGriglia? cellaVicino = mappa.OttieniCella(vicino);
                if (cellaVicino != null && cellaVicino.IsCalpestabile)
                {
                    return vicino; // Trovata
                }

                visitati.Add(vicino);
                coda.Enqueue(vicino);
            }
        }

        // Fallback
        return new Punto3D(0, 0, 0);
    }

    #endregion


    #region A*
    public class NodoAStar
    {
        public CellaGriglia Cella { get; }
        public NodoAStar? Padre { get; set; }
        public double G { get; set; } // Costo dal punto di partenza al nodo corrente
        public double H { get; set; } // Stima del costo dal nodo corrente alla destinazione (Euristica)
        public double F => G + H; // Costo totale

        public NodoAStar(CellaGriglia cella)
        {
            Cella = cella;
            G = float.MaxValue;
            H = 0;
            Padre = null;
        }
    }
    public static List<CellaGriglia>? A_Star(MappaGriglia mappa, CellaGriglia partenza, CellaGriglia destinazione)
    {
        if (!partenza.IsCalpestabile || !destinazione.IsCalpestabile)
            return null;

        // Dizionario per associare ogni CellaGriglia al suo NodoAStar
        Dictionary<CellaGriglia, NodoAStar> nodi = new Dictionary<CellaGriglia, NodoAStar>();

        // Funzione helper per recuperare o creare il nodo associato alla cella
        NodoAStar GetNodo(CellaGriglia cella)
        {
            if (!nodi.TryGetValue(cella, out var nodo))
            {
                nodo = new NodoAStar(cella);
                nodi[cella] = nodo;
            }
            return nodo;
        }

        NodoAStar nodoPartenza = GetNodo(partenza);
        NodoAStar nodoDestinazione = GetNodo(destinazione);


        List<NodoAStar> openSet = new List<NodoAStar>();
        HashSet<NodoAStar> closedSet = new HashSet<NodoAStar>();

        // Inizializzazione nodo di partenza
        nodoPartenza.G = 0;
        nodoPartenza.H = DistanzaManhattan(nodoPartenza, nodoDestinazione);
        openSet.Add(nodoPartenza);


        while (openSet.Count > 0)
        {
            // Estrae il nodo con il valore F minore
            NodoAStar corrente = openSet.OrderBy(n => n.F).First();

            // Destinazione raggiunta
            if (corrente.Cella.Equals(destinazione))
            {
                return RicostruisciPercorso(corrente);
            }

            openSet.Remove(corrente);
            closedSet.Add(corrente);

            // Ottiene i vicini calpestabili dalla MappaGriglia
            foreach (CellaGriglia cellaVicino in mappa.GetVicini(corrente.Cella))
            {
                NodoAStar nodoVicino = GetNodo(cellaVicino);

                if (closedSet.Contains(nodoVicino))
                    continue;

                double gTentativo = corrente.G + 1.0;

                if (gTentativo < nodoVicino.G)
                {
                    nodoVicino.Padre = corrente;
                    nodoVicino.G = gTentativo;
                    nodoVicino.H = DistanzaManhattan(nodoVicino, nodoDestinazione);

                    if (!openSet.Contains(nodoVicino))
                    {
                        openSet.Add(nodoVicino);
                    }
                }
            }
        }

        return null; // Nessun percorso disponibile
    }



    private static List<CellaGriglia> RicostruisciPercorso(NodoAStar nodoDestinazione)
    {
        List<CellaGriglia> percorso = new List<CellaGriglia>();
        NodoAStar? corrente = nodoDestinazione;

        while (corrente != null)
        {
            percorso.Add(corrente.Cella);
            corrente = corrente.Padre;
        }

        percorso.Reverse();
        return percorso;
    }

    private static double DistanzaManhattan(NodoAStar partenza, NodoAStar arrivo)
    {
        CellaGriglia inizio, fine;
        inizio = partenza.Cella;
        fine = arrivo.Cella;

        return Math.Abs(inizio.Posizione.X - fine.Posizione.X) +
               Math.Abs(inizio.Posizione.Y - fine.Posizione.Y) +
               Math.Abs(inizio.Posizione.Z - fine.Posizione.Z);
    }

    #endregion


}
