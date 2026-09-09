using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Model.Interfacce;

namespace OneBigCity_Model.Classi.Entita.Agenti;

public class Persona : IEntita
{
    public Persona() { } // Costruttore di default necessario per la deserializzazione JSON
    public Persona(string nome, string cognome, Sesso sesso, DateTime dataNascita)
    {
        Nome = nome;
        Cognome = cognome;
        Sesso = sesso;
        DataNascita = dataNascita;
    }
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public Sesso Sesso { get; set; }
    public DateTime DataNascita { get; set; }
    public Punto3D Posizione { get; set; } = new(0, 0, 0);

    public int CalcolaEta(DateTime dataCorrenteSimulazione)
    {
        int eta = dataCorrenteSimulazione.Year - DataNascita.Year;
        if (DataNascita.Date > dataCorrenteSimulazione.AddYears(-eta))
            eta--;
        return eta;
    }
}
