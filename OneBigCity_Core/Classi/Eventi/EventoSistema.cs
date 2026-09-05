namespace OneBigCity_Model.Classi.Eventi;

public class EventoSistema
{
    // Classe per eventi di carattere log / notifiche
    public string Log { get; }

    public EventoSistema(string log)
    {
        Log = log;
    }
}
