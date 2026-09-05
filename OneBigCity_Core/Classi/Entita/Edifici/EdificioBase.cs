using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Model.Classi.Entita.Edifici;

/// <summary>
/// Classe base per rappresentare una struttura
/// </summary>
public class EdificioBase
{
    public int ID { get; set; } = 0;
    public string Nome { get; set; } = "";
    public string Descrizione { get; set; } = "";
    TipoEdificio Tipo { get; set; } = TipoEdificio.Non_Specificato;

    public List<Punto3D> CoordinateIngressi { get; set; } = new();
    public int CapienzaMassima { get; set; }
}
