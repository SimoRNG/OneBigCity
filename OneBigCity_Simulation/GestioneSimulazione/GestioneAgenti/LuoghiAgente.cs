using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Entita.EnumEntita;
namespace OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;
public static class LuoghiAgente
{
    public static EdificioBase? GetEdificio(AgenteBase agente, Citta citta)
        => agente.EdificioCorrente == null
            ? null
            : citta.Edifici.FirstOrDefault(e => e.ID == agente.EdificioCorrente);

    public static string NomeLuogo(EdificioBase e, AgenteBase agente)
    {
        // Nome logico dell'edificio dal punto di vista di questo agente

        if (e.Proprietario == agente.ID) return "Casa";
        return e.Tipo switch
        {
            TipoEdificio.Commerciale => "Negozio",
            TipoEdificio.Industriale => "Lavoro",
            _ => "Altro"
        };
    }

    //Tutti gli edifici che dal punto di vista dell'agente corrispondono al luogo ("Casa", "Negozio", "Lavoro")
    public static IEnumerable<EdificioBase> TrovaEdifici(string luogo, AgenteBase agente, Citta citta)
        => citta.Edifici.Where(e => NomeLuogo(e, agente) == luogo);

    public static string GetLuogo(AgenteBase agente, Citta citta)
    {
        var e = GetEdificio(agente, citta);
        return e == null ? "Citta" : NomeLuogo(e, agente);
    }

    public static bool IsInCasa(AgenteBase agente, Citta citta) => GetLuogo(agente, citta) == "Casa";

    public static IEnumerable<EdificioBase> GetEdificiVicini(AgenteBase agente, Citta citta) => agente.EdificioCorrente != null
        ? Enumerable.Empty<EdificioBase>()
        : citta.Edifici.Where(e => e.CoordinateAccessiEsterni.Contains(agente.Posizione));

    public static EdificioBase? GetEdificioVicino(AgenteBase agente, Citta citta, string luogo) => GetEdificiVicini(agente, citta).FirstOrDefault(e => NomeLuogo(e, agente) == luogo);

    // Luoghi raggiungibili dalla posizione attuale
    public static bool IsVicinoA(AgenteBase agente, Citta citta, string luogo) => GetEdificioVicino(agente, citta, luogo) != null;
}