using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;

namespace OneBigCity_Model.Classi.Entita.Edifici;

public class Abitazione : EdificioBase
{
    public Abitazione()
    {
        this.Tipo = TipoEdificio.Residenziale;
        this.CapienzaMassima = 5;
    }

    public void ImpostaProprietario(Guid nuovoProprietario)
    {
        this.Proprietario = nuovoProprietario;
    }
}
