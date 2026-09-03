using OneBigCity_Core.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Core.Classi.Static;

public static class CalendarioStagioni
{
    public static Stagione OttieniStagioneDaData(DateTime data)
    {
        return (data.Month * 100 + data.Day) switch
        {
            >= 321 and <= 620 => Stagione.Primavera,
            >= 621 and <= 922 => Stagione.Estate,
            >= 923 and <= 1220 => Stagione.Autunno,
            _ => Stagione.Inverno // Copre sia la fine dell'anno che l'inizio
        };
    }

    public static TipoMeteo OttieniMeteoStagionale(Stagione stagione)
    {
        return stagione switch
        {
            Stagione.Autunno => TipoMeteo.PioggiaLeggera,
            Stagione.Inverno => TipoMeteo.Temporale,
            _ => TipoMeteo.Sereno
        };
    }

    public static float OttieniFattoreStagionale(TipoMeteo meteo, Stagione stagione)
    {
        return (stagione, meteo) switch
        {
            // Estate: No Neve, favoriti Sereno e Temporali
            (Stagione.Estate, TipoMeteo.Neve) => 0.0f,
            (Stagione.Estate, TipoMeteo.Sereno) => 1.5f,

            // Inverno: Meno Sereno, favoriti Neve e Nuvoloso
            (Stagione.Inverno, TipoMeteo.Neve) => 1.5f,
            (Stagione.Inverno, TipoMeteo.Grandine) => 0.1f,

            _ => 1.0f // Peso di default per combinazioni standard
        };
    }


}
