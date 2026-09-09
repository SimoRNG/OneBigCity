using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Model.Interfacce;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Edifici;

/// <summary>
/// Classe base per rappresentare una struttura
/// </summary>

[JsonDerivedType(typeof(Abitazione), typeDiscriminator: "Abitazione")]
public class EdificioBase : IEntita
{
    public EdificioBase() { }
    public EdificioBase(string nome, string descrizione = "")
    {
        Nome = nome;
        Descrizione = descrizione;
    }

    public Guid ID { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = "";
    public string Descrizione { get; set; } = "";
    public TipoEdificio Tipo { get; set; } = TipoEdificio.Non_Specificato;
    public int PercentualeCondizione { get; set; } = 100;
    public Persona? Proprietario { get; set; } = null;
    public List<Punto3D> CoordinateIngressi { get; set; } = new(); // Ingressi calpestabili dall'esterno
    public List<Punto3D> CoordinateOccupate { get; set; } = new(); // Celle della mappa occupate dall'impronta a terra della struttura
    public int CapienzaMassima { get; set; }
}
