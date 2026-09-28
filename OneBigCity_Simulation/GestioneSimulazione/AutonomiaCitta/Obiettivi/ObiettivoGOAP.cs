using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Interfacce;

namespace OneBigCity_Model.AutonomiaCitta.Obiettivi;

public abstract class ObiettivoGOAP : IObiettivoGOAP
{
    public string Nome { get; protected set; } = "ObiettivoGenerico";

    public float Priorita { get; set; } = 0f; // Più è alta più l'agente vuole soddisfarla

    /// <summary>
    /// Chiavi valori che rappresentano le condizioni per dire che l'obiettivo è raggiunto
    /// Es: <"FameSoddisfatta", true> oppure <"PortafoglioMaggioreDi", 50>
    /// </summary>
    public Dictionary<string, object> StatoDesiderato { get; protected set; } = new();

    public abstract bool IsSoddisfatto(AgenteBase agente, Citta citta);

    public abstract double CalcolaPriorita(AgenteBase agente, Citta citta); // Gestita dall'utility
}
