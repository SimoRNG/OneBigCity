using OneBigCity_Core.Classi.Meteo.EnumMeteo;
using OneBigCity_Simulation.Motori;

namespace OneBigCity_Simulation.Precompilati;

/// <summary>
/// Funzioni calcolate una volta sola ad inizio simulazione per evitare calcoli pesanti ad ogni tick
/// </summary>
internal static class MeteoLUT
{
    internal static Dictionary<(TipoMeteo, TipoMeteo), float> InizializzaPesiBase()
    {
        var dizionario = new Dictionary<(TipoMeteo, TipoMeteo), float>();

        foreach (var da in MotoreMeteo.ValoriMeteo)
        {
            foreach (var a in MotoreMeteo.ValoriMeteo)
            {
                int delta = Math.Abs((int)a - (int)da);
                float peso = delta switch
                {
                    0 => 80f, // Alta possibilità di stabilità
                    1 => 14f, // Piccola possibilità di cambio meteo
                    2 => 5.5f, // Ancora più piccola possibilità di cambio meteo
                    _ => 0.5f   // Minuscola possibilita si cambio radicale
                };
                dizionario[(da, a)] = peso;
            }
        }
        return dizionario;
    }
}
