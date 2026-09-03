using OneBigCity_Core.Classi.Meteo;
using OneBigCity_Core.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Core.Classi.Eventi;


public class EventoMeteoCambiato
{
    public CondizioneAtmosferica Precedente { get; }
    public CondizioneAtmosferica Nuova { get; }

    public EventoMeteoCambiato(CondizioneAtmosferica precedente, CondizioneAtmosferica nuova)
    {
        Precedente = precedente;
        Nuova = nuova;
    }
}
