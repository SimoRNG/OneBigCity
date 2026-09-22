using OneBigCity_Model.Classi.Entita.EnumEntita;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Edifici;

public class Abitazione : EdificioBase
{
    [JsonConstructor]
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
