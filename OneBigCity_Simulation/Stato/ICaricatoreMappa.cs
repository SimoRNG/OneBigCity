using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.Stato;

public interface ICaricatoreMappa
{
    public MappaGriglia CaricaMappa(string sorgente);
}
