using OneBigCity_Model.Classi.Entita.EnumEntita;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Edifici;

public class Industria : EdificioBase
{
    public Industria()
    {
        this.Tipo = TipoEdificio.Industriale;
        this.CapienzaMassima = 10;
    }
}
