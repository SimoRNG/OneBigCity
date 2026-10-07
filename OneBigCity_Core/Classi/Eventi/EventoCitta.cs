namespace OneBigCity_Model.Classi.Eventi;

public class EventoCitta 
{
    // Classe per eventi riguardanti la città
    public DateTime DataOraCitta { get; }
    public string Log { get; }

    public EventoCitta(DateTime dataOraEvento, string log)
    {
        DataOraCitta = dataOraEvento;
        Log = log;
    }
}
