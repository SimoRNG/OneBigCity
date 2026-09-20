using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Model.Classi.Eventi;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.GestioneSimulazione.ComandiSistemi;

namespace OneBigCity_Simulation.GestioneSimulazione.Sistemi;

public class SistemaAgenti : ISistemaSimulazione, IDisposable
{
    public SistemaAgenti(EventBus eventBus) { _eventBus = eventBus; }

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
        GestisciIntenzioniAgenti(contesto.OrarioCorrente, contesto.Citta, buffer);

    }

    private void GestisciIntenzioniAgenti(DateTime orarioCorrente, Citta citta, CommandBuffer buffer)
    {
        if (citta.Abitanti.Count == 0 || citta.Edifici.Count == 0) return;

        List<Persona> listaAbitanti = citta.Abitanti.ToList();
        List<EdificioBase> listaLuoghiIndustriali = citta.Edifici.Where(e => e.Tipo == TipoEdificio.Industriale).ToList();
        List<EdificioBase> listaAbitazioni = citta.Edifici.Where(e2 => e2.Tipo == TipoEdificio.Residenziale).ToList();

        TimeSpan orario = orarioCorrente.TimeOfDay;

        if ((orario.Hours == 8 && orario.Minutes == 0) || (orario.Hours == 17 && orario.Minutes == 0))
        {
            foreach (Persona abitante in listaAbitanti)
            {
                Punto3D destinazione = new();
                if (orario.Hours == 8)
                {
                    destinazione = listaLuoghiIndustriali[0].CoordinateAccessiEsterni[0];
                }
                else
                {
                    EdificioBase? casa = listaAbitazioni.Where(a => a.Proprietario == abitante.ID).FirstOrDefault();
                    if (casa != null) destinazione = casa.CoordinateAccessiEsterni[0];
                }
                if (destinazione == new Punto3D()) continue;

                if (orarioCorrente.TimeOfDay == new TimeSpan(8, 0, 0) || orarioCorrente.TimeOfDay == new TimeSpan(17, 0, 0))
                {
                    buffer.Aggiungi(new ComandoRichiediMovimento(abitante.ID, destinazione));
                }
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
