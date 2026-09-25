using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta.Azioni;

public class AzioneMangia : AzioneGOAP
{
    private double _secondiTrascorsi = 0;

    public AzioneMangia()
    {
        Nome = "MangiaCibo";
        Costo = 1.0;
        DurataInSecondiSimulazione = 10; // Dura 10 secondi virtuali

        // SODDISFA IL GOAL: ObiettivoSoddisfaFame cerca "FameSoddisfatta" = true
        Effetti.Add("FameSoddisfatta", true);
    }

    public override bool VerificAPrecondizioni(AgenteBase agente, Citta citta)
    {
        return true;
    }

    public override void Inizia(AgenteBase agente, Citta citta, CommandBuffer buffer)
    {
        base.Inizia(agente, citta, buffer);
        _secondiTrascorsi = 0;
    }

    public override void Aggiorna(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        _secondiTrascorsi += contesto.DeltaTempoSecondi;

        if (_secondiTrascorsi >= DurataInSecondiSimulazione)
        {
            // Ripristina il bisogno della fame nell'agente
            if (agente.Bisogni.ContainsKey(Bisogno.Fame))
            {
                agente.Bisogni[Bisogno.Fame] = 100;
            }

            Stato = StatoAzione.Completata;
        }
    }
}