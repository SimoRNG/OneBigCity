using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public class SistemaEdifici : ISistemaSimulazione
{
    public SistemaEdifici(EventBus eventBus) { }

    public void Aggiorna(DateTime orarioCorrente, Citta citta)
    {
        ApplicaUsuraEdifici(citta);
        GestisciStatoEdifici(citta);
    }

    #region Entra/Esci

    /*
    Espongo metodi per far entrare/uscire una persona
    Non è questo sistema a gestire direttamente l'accesso ad un edificio perchè una persona potrebbe
    trovarsi su una casella di accesso esterno ma sta andando altrove e non vuole effettivamente entrare
    nel dato edificio
     */

    // Metodo chiamato quando una persona vuole entrare
    public bool ProvaIngresso(Persona persona, EdificioBase edificio)
    {
        if (edificio.Stato != StatoEdificio.Operativo) return false;
        if (edificio.OccupantiAttuali.Count >= edificio.CapienzaMassima) return false;

        edificio.OccupantiAttuali.Add(persona);

        return true;
    }

    // Metodo chiamato quando una persona vuole uscire
    public void FaiUscire(Persona persona, EdificioBase edificio)
    {
        edificio.OccupantiAttuali.Remove(persona);
    }
    #endregion


    private void ApplicaUsuraEdifici(Citta citta)
    {
        // 10% di possibilità di peggiorare la condizione degli edifici

        // L'usurazione si applica ad edifici non distrutti
        var edificiUsirabili = citta.Edifici.Where(e => e.PercentualeCondizione > 0 && e.Stato != StatoEdificio.Distrutto).ToList();
        foreach (var edificio in edificiUsirabili)
        {
            if (OneBigCity_Model.Classi.Static.Utilita.RNG(1, 100) <= 10)
                edificio.PercentualeCondizione -= 1;
        }
    }

    private void GestisciStatoEdifici(Citta citta)
    {
        // Cambio lo stato degli edifici in pessime condizioni

        foreach (var edificio in citta.Edifici)
        {
            if (edificio.PercentualeCondizione == 0)
            {
                edificio.Stato = StatoEdificio.Distrutto;
                edificio.Accesso = TipoAccessoEdificio.Vietato;
                edificio.CoordinateAccessiEsterni = new(); // Edificio distrutto - nessun accesso
                edificio.PlanimetriaInterna = new();
                if (edificio.OccupantiAttuali.Count > 0)
                {
                    // TODO - Elimina persona (l'edificio le è crollato addosso)
                }
            }
        }
    }
}
