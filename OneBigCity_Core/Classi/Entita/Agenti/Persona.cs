using OneBigCity_Model.Classi.Entita.EnumEntita;

namespace OneBigCity_Model.Classi.Entita.Agenti;

public class Persona
{
    public Persona(string nome, string cognome, Sesso sesso, DateTime dataNascita)
    {
        Nome = nome;
        Cognome = cognome;
        Sesso = sesso;
        DataNascita = dataNascita;
    }

    public string Nome { get; set; }
    public string Cognome { get; set; }
    public Sesso Sesso { get; set; }
    public DateTime DataNascita { get; set; }

    public int CalcolaEta(DateTime dataCorrenteSimulazione)
    {
        int eta = dataCorrenteSimulazione.Year - DataNascita.Year;
        if (DataNascita.Date > dataCorrenteSimulazione.AddYears(-eta))
            eta--;
        return eta;
    }
}
