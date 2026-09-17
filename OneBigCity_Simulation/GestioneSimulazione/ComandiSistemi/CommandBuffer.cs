using OneBigCity_Model.Classi.Entita;

namespace OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

public class CommandBuffer
{
    private readonly List<IComandoSimulazione> _comandi = new();

    public void Aggiungi(IComandoSimulazione comando)
    {
        _comandi.Add(comando);
    }
    public void ApplicaTutti(Citta citta)
    {
        foreach (var comando in _comandi)
        {
            comando.Esegui(citta);
        }
    }
    public void Svuota()
    {
        _comandi.Clear();
    }

}
