using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;

namespace OneBigCity_Simulation.DTO;

public class EntitaIniziali
{
    public List<Persona> Persone { get; set; } = new();
    public List<EdificioBase> Edifici { get; set; } = new();
}
