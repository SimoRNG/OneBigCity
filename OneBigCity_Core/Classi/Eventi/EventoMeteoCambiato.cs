using OneBigCity_Model.Classi.Meteo;
namespace OneBigCity_Model.Classi.Eventi;

public class EventoMeteoCambiato : EventoLoggatoBase
{
    public CondizioneAtmosferica Precedente { get; }
    public CondizioneAtmosferica Nuova { get; }

    public EventoMeteoCambiato(CondizioneAtmosferica precedente, CondizioneAtmosferica nuova)
    {
        Precedente = precedente;
        Nuova = nuova;
    }
}
