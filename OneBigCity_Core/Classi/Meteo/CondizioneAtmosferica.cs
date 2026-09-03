using OneBigCity_Core.Classi.Meteo.EnumMeteo;

namespace OneBigCity_Core.Classi.Meteo;

public readonly record struct CondizioneAtmosferica(TipoMeteo Tipo, float TemperaturaCelsius)
{
    public override string ToString() => $"{Tipo} ({TemperaturaCelsius:F1}°C)";
}
