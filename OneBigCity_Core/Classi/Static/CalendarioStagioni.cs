using OneBigCity_Model.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Model.Classi.Static;

public static class CalendarioStagioni
{
    // Array di lookup: l'indice corrisponde al giorno dell'anno (da 1 a 366)
    private static readonly float[] TemperatureStagionaliPrecalcolate = InizializzaTemperatureAnnuali();

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

    public static float CalcolaTemperaturaStagionaleBase(DateTime data)
    {
        return TemperatureStagionaliPrecalcolate[data.DayOfYear];
    }

    private static float[] InizializzaTemperatureAnnuali()
    {
        // Metodo per pre-calcolare la curva delle temperature

        var tabella = new float[367]; // Indici da 1 a 366

        float tempMediaAnnua = 15.0f;
        float ampiezzaStagionale = 12.0f;

        for (int giornoAnno = 1; giornoAnno <= 366; giornoAnno++)
        {
            // Eseguo il calcolo trigonometrico solo qui - 366 volte in totale all'avvio
            double radianti = (2.0 * Math.PI * (giornoAnno - 20)) / 365.0;
            tabella[giornoAnno] = tempMediaAnnua - (ampiezzaStagionale * (float)Math.Cos(radianti));
        }

        return tabella;
    }
}
