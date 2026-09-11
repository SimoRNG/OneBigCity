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
}
