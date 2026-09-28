using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;
using OneBigCity_Simulation.GestioneSimulazione.GestioneAgenti;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public class SistemaAgenti : ISistemaSimulazione, IDisposable
{
    public SistemaAgenti(EventBus eventBus, GestoreUtilityAgenti gestoreUtility, List<AzioneGOAP> azioniTotali)
    {
        _eventBus = eventBus;
        _gestoreUtility = gestoreUtility;
        _azioniTotaliDisponibili = azioniTotali;
    }

    private readonly GestoreUtilityAgenti _gestoreUtility;
    private readonly List<AzioneGOAP> _azioniTotaliDisponibili;
    public readonly EventBus _eventBus;

    public void Dispose()
    {

    }

    public void Aggiorna(ContestoTick contesto, CommandBuffer buffer)
    {
        // Logica iniziale base - gli agenti alle ore 8 vanno verso il primo luogo industriale della lista edifici della città
        // alle 17 invece tornano a casa (se ce l'hanno)
        // Per le persone che hanno raggiunto la destinazione provo a farle entrare

        GestisciSaluteAgenti(contesto.OrarioCorrente, contesto.Citta, buffer);
        GestisciBisogniAgente(contesto.OrarioCorrente, contesto.Citta, buffer);

        foreach (AgenteBase agente in contesto.Citta.Abitanti)
        {
            EseguiCicloGOAPAgente(agente, contesto, buffer);
        }

    }

    private void GestisciBisogniAgente(DateTime orarioCorrente, Citta citta, CommandBuffer buffer)
    {
        // TODO - Gestire meglio

        foreach (Persona persona in citta.Abitanti)
        {
            buffer.Aggiungi(new ComandoVariaBisogno(persona.ID, Bisogno.Fame, -10));
            buffer.Aggiungi(new ComandoVariaBisogno(persona.ID, Bisogno.Sete, -10));
            buffer.Aggiungi(new ComandoVariaBisogno(persona.ID, Bisogno.Energia, -10));
        }
    }

    private void EseguiCicloGOAPAgente(AgenteBase agente, ContestoTick contesto, CommandBuffer buffer)
    {
        // Se l'agente non ha un'azione in corso né un piano ne generaro uno
        if (agente.AzioneInEsecuzione == null && agente.PianoCorrente.Count == 0)
        {
            _gestoreUtility.AggiornaDecisioneAgente(agente, contesto.Citta, _azioniTotaliDisponibili);
  
        }

        // Se l'agente non ha un'azione in corso ma ha azioni nel piano passo alla prossima
        if (agente.AzioneInEsecuzione == null && agente.PianoCorrente.Count > 0)
        {
            if (agente.PianoCorrente.Dequeue() is AzioneGOAP prossimaAzione)
            {
                agente.AzioneInEsecuzione = prossimaAzione;
                prossimaAzione.Inizia(agente, contesto.Citta, buffer);
            }
        }

        // Se c'è un'azione in corso aggiorno
        if (agente.AzioneInEsecuzione is AzioneGOAP azioneCorrente)
        {
            azioneCorrente.Aggiorna(agente, contesto, buffer);

            if (azioneCorrente.Stato == StatoAzione.Completata)
            {
                azioneCorrente.Termina(agente, contesto.Citta, buffer);
                agente.AzioneInEsecuzione = null; // Pronti per la prossima azione al prossimo tick
            }
            else if (azioneCorrente.Stato == StatoAzione.Fallita)
            {
                // In caso di fallimento forzo il re-planning
                agente.PianoCorrente.Clear();
                agente.AzioneInEsecuzione = null;
            }
        }

    }

    private void GestisciSaluteAgenti(DateTime orarioCorrente, Citta citta, CommandBuffer buffer)
    {
        // Elimino over 80 anni
        var daRimuovere = citta.Abitanti.Where(a => a.CalcolaEta(orarioCorrente) > 80).ToList();

        foreach (Persona anziano in daRimuovere) buffer.Aggiungi(new ComandoEliminaPersona(anziano.ID));

        if (daRimuovere.Count > 0) _eventBus.Pubblica(new EventoCitta(orarioCorrente, $"Sono stati rimossi {daRimuovere.Count.ToString()} abitanti!"));
    }
}
