using OneBigCity_Model.Classi.Entita.EnumEntita;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Edifici;

public class Industria : EdificioBase
{
    [JsonConstructor]
    public Industria()
    {
        this.Tipo = TipoEdificio.Industriale;
        this.CapienzaMassima = 5;
    }

    public void ImpostaProprietario(Guid nuovoProprietario)
    {
        this.Proprietario = nuovoProprietario;
    }
}
