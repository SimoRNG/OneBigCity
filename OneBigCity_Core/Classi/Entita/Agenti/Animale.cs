using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Interfacce;

namespace OneBigCity_Model.Classi.Entita.Agenti;

public class Animale: AgenteBase, IEntita
{
    public Animale() { InizializzaBisogni(); } // Costruttore di base necessario per la deserializzazione

    public Animale(string nome)
    {
        Nome = nome;
        InizializzaBisogni();
    }

    public Animale(string nome, Guid? padrone)
    {
        Nome = nome;
        Padrone = padrone;

        InizializzaBisogni();
    }

    private void InizializzaBisogni()
    {
        this.Bisogni = new();
        Bisogni.Add(Bisogno.Fame, 100);
        Bisogni.Add(Bisogno.Sete, 100);
        Bisogni.Add(Bisogno.Energia, 100);
    }

    public string Nome { get; set; } = "";

    public Guid? Padrone { get; set; } = null;
    public Sesso Sesso { get; set; }
    public SpecieAnimale Specie { get; set; } = SpecieAnimale.NonDefinita;

    public DateTime DataNascita { get; set; }

    public int CalcolaEta(DateTime dataCorrenteSimulazione)
    {
        int eta = dataCorrenteSimulazione.Year - DataNascita.Year;
        if (DataNascita.Date > dataCorrenteSimulazione.AddYears(-eta))
            eta--;
        return eta;
    }

    

}
