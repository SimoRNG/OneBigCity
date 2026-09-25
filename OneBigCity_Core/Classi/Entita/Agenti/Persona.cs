using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Interfacce;

namespace OneBigCity_Model.Classi.Entita.Agenti;

public class Persona : AgenteBase, IEntita
{
    public Persona() { InizializzaBisogni(); } // Costruttore di default necessario per la deserializzazione JSON
    public Persona(string nome, string cognome, Sesso sesso, DateTime dataNascita)
    {
        Nome = nome;
        Cognome = cognome;
        Sesso = sesso;
        DataNascita = dataNascita;

        InizializzaBisogni();
    }

    private void InizializzaBisogni()
    {
        this.Bisogni = new();
        Bisogni.Add(Bisogno.Fame, 100);
        Bisogni.Add(Bisogno.Sete, 100);
        Bisogni.Add(Bisogno.Energia, 100);
        Bisogni.Add(Bisogno.Igiene ,100);
        Bisogni.Add(Bisogno.Socialita, 100);
        Bisogni.Add(Bisogno.Relax, 100);
    }

    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public Sesso Sesso { get; set; }
    public DateTime DataNascita { get; set; }

    public Dictionary<Risorsa, double> Inventario { get; set; } = new();

    public int CalcolaEta(DateTime dataCorrenteSimulazione)
    {
        int eta = dataCorrenteSimulazione.Year - DataNascita.Year;
        if (DataNascita.Date > dataCorrenteSimulazione.AddYears(-eta))
            eta--;
        return eta;
    }
}
