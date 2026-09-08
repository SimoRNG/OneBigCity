using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Model.Classi.Entita;

public class Citta
{
    // Il motivo per cui ho sia una lista di Edifici che la mappatura MappaEdifici
    // è perché un edificio può spaziare su più celle. Avendo solo la mappa avrei n riferimenti allo stesso edificio
    // in base a quante celle occupa. La lista di edifici mi aiuta per l'univocità.


    public string Nome { get; set; } = "";
    public MappaGriglia Mappa { get; set; } = new MappaGriglia();
    public List<Persona> Abitanti { get; set; } = new();

    public List<EdificioBase> Edifici { get; } = new();
    public Dictionary<Punto3D, EdificioBase> MappaEdifici = new();
    public Citta() { }
    public Citta(string nome, MappaGriglia mappa)
    {
        Nome = nome;
        Mappa = mappa ?? throw new ArgumentNullException(nameof(mappa));
    }
}
