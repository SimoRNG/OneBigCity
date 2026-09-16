using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Geografia;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public class SistemaEdifici : ISistemaSimulazione
{
    private readonly EventBus _eventBus;
    public SistemaEdifici(EventBus eventBus) { _eventBus = eventBus; }

    public void Aggiorna(DateTime orarioCorrente, Citta citta)
    {
        ApplicaUsuraEdifici(citta);
        GestisciStatoEdifici(citta);
    }


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
                _eventBus.Pubblica(new EventoSistema("L'edificio è andato distrutto!"));

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
