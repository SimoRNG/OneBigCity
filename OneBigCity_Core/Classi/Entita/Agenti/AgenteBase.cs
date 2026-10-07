using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Model.Interfacce;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Entita.Agenti;

[JsonDerivedType(typeof(Persona), typeDiscriminator: "Persona")]
[JsonDerivedType(typeof(Animale), typeDiscriminator: "Animale")]
public abstract class AgenteBase
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public Punto3D Posizione { get; set; } = new(0, 0, 0);
    
    /// <summary>
    /// null = in città , altrimenti l'id dell'edificio
    /// Posizione di conseguenza sarà relativa alla griglia della città o dell'edificio (planimetria interna)
    /// </summary>
    public Guid? EdificioCorrente { get; set; } = null;


    public Dictionary<Bisogno, double> Bisogni { get; set; } = new(); // Chiave: Bisogno - Valore: Percentuale soddisfamento
    public Dictionary<Risorsa, double> Inventario { get; set; } = new();

    // --- Movimento ---
    public bool PuoMuoversi { get; set; } = true;
    public List<CellaGriglia> Percorso { get; set; } = new();
    public int IndiceStepPercorso { get; set; } = -1;
    public int VelocitaMovimento { get; set; } = 8; // celle per tick


    // --- Piano d'esecuzione ---
    public Queue<IAzioneGOAP> PianoCorrente { get; set; } = new(); // Lista ordinata di operazioni da eseguire
    public IAzioneGOAP? AzioneInEsecuzione { get; set; }
    public IObiettivoGOAP? ObiettivoCorrente { get; set; }
}
