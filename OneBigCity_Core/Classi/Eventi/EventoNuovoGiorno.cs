namespace OneBigCity_Model.Classi.Eventi;

/// <summary>
/// Evento pubblicato sul bus ogni volta che la simulazione avanza di un giorno
/// </summary>
public class EventoNuovoGiorno
{
    public DateTime DataCorrente { get; }

    public EventoNuovoGiorno(DateTime nuovaData)
    {
        DataCorrente = nuovaData;
    }
}

