namespace OneBigCity_Core.Classi.Eventi;

/// <summary>
/// Gestore centralizzato per la pubblicazione e sottoscrizione degli eventi
/// </summary>
public class EventBus
{
    // Dizionario che associa un Tipo di Evento (es. EventoGiornoTrascorso) 
    // a una lista di azioni/ascoltatori registrati per quel tipo
    private readonly Dictionary<Type, List<Delegate>> _iscritti = new();

    /// <summary>
    /// Registra un metodo/azione da eseguire quando viene pubblicato un tipo specifico di evento
    /// </summary>
    public void Iscrivi<T>(Action<T> azione)
    {
        Type tipoEvento = typeof(T);

        if (!_iscritti.ContainsKey(tipoEvento))
        {
            _iscritti[tipoEvento] = new List<Delegate>();
        }

        _iscritti[tipoEvento].Add(azione);
    }

    /// <summary>
    /// Rimuove la registrazione di un metodo
    /// </summary>
    public void Disiscrivi<T>(Action<T> azione)
    {
        Type tipoEvento = typeof(T);

        if (_iscritti.ContainsKey(tipoEvento))
        {
            _iscritti[tipoEvento].Remove(azione);
        }
    }

    /// <summary>
    /// Pubblica un evento e notifica immediatamente tutti gli ascoltatori registrati a quel tipo
    /// </summary>
    public void Pubblica<T>(T evento)
    {
        Type tipoEvento = typeof(T);

        if (_iscritti.TryGetValue(tipoEvento, out List<Delegate>? azioni))
        {
            // Copia della lista per evitare eccezioni durante l'iterazione se un ascoltatore modifica le iscrizioni
            foreach (Delegate azione in azioni.ToList())
            {
                ((Action<T>)azione)(evento);
            }
        }
    }
}