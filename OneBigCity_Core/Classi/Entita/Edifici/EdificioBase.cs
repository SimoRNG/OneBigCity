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
    public StatoEdificio Stato { get; set; } = StatoEdificio.Operativo;
    public TipoAccessoEdificio Accesso { get; set; } = TipoAccessoEdificio.Libero;
    public int PercentualeCondizione { get; set; } = 100;
    public Guid? Proprietario { get; set; } = null;
    public List<Punto3D> CoordinateAccessiEsterni { get; set; } = new(); // Ingressi calpestabili dall'esterno
    public List<Punto3D> CoordinateOccupate { get; set; } = new(); // Celle della mappa occupate dall'impronta a terra della struttura
    public int CapienzaMassima { get; set; }

    
    public List<Guid> OccupantiAttuali { get; set; } = new(); // Lista dei Guid delleS persone attualmente presenti dentro la struttura


    public MappaGriglia PlanimetriaInterna { get; set; } = new(5,5,0,0); // Mappa interna dell'edificio - dove vengono posizionate le persone all'interno
    public List<Punto3D> CoordinateAccessiInterni { get; set; } = new(); // Punti sulla griglia interna per uscire dall'edificio

}
