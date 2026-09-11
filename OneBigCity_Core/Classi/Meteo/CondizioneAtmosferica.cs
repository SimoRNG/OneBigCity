using OneBigCity_Model.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Model.Classi.Meteo;

public readonly record struct CondizioneAtmosferica(TipoMeteo Tipo, float TemperaturaCelsius)
{
    public CondizioneAtmosferica() : this(TipoMeteo.Sereno, 25f) { }
    public override string ToString() => $"{Tipo} ({TemperaturaCelsius:F1}°C)";
}
