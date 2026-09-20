using OneBigCity_Model.Classi.Geografia;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Agenti;

[JsonDerivedType(typeof(Persona), typeDiscriminator: "Persona")]
[JsonDerivedType(typeof(Animale), typeDiscriminator: "Animale")]
public abstract class AgenteBase
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public Punto3D Posizione { get; set; } = new(0, 0, 0);

    // --- Movimento ---
    public bool PuoMuoversi { get; set; } = true;
    public List<Punto3D> Percorso { get; set; } = new();
    public int IndiceStepPercorso = -1;


}
