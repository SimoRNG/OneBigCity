using OneBigCity_Model.Classi.Entita;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public interface ISistemaSimulazione
{
    /// <summary>
    /// Esegue il ciclo di aggiornamento del sistema
    /// </summary>
    /// <param name="orarioCorrente">Data e ora del tick</param>
    /// <param name="citta">Riferimento allo stato globale della città</param>
    void Aggiorna(DateTime orarioCorrente, Citta citta, CommandBuffer buffer);
}
