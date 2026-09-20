using OneBigCity_Model.Classi.Entita;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public interface ISistemaSimulazione
{
    /// <summary>
    /// Esegue il ciclo di aggiornamento del sistema
    /// </summary>
    /// <param name="buffer">Riferimento al buffer dei comandi per azioni delegate ad altri sistemi</param>
    void Aggiorna(ContestoTick contesto, CommandBuffer buffer);
}
