using OneBigCity_Core.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Core.Classi.Eventi;


public class EventoMeteoCambiato
{
    public TipoMeteo MeteoPrecedente { get; }
    public TipoMeteo NuovoMeteo { get; }

    public EventoMeteoCambiato(TipoMeteo meteoPrecedente, TipoMeteo nuovoMeteo)
    {
        MeteoPrecedente = meteoPrecedente;
        NuovoMeteo = nuovoMeteo;
    }
}
