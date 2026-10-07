using OneBigCity_Model.Classi.Entita.EnumEntita;

namespace OneBigCity_Model.Classi.Entita.Edifici;

public class Negozio:EdificioBase
{
    public Negozio()
    {
        this.Tipo = TipoEdificio.Commerciale;
        this.CapienzaMassima = 10;
    }
}
