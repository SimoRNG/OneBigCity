using OneBigCity_Model.AutonomiaCitta.Obiettivi;
using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.EnumEntita;
using OneBigCity_Simulation.GestioneSimulazione.Azioni;

namespace OneBigCity_Simulation.GestioneSimulazione.AutonomiaCitta;

public class PianificatoreGOAP
{
    public Queue<AzioneGOAP>? CalcolaPiano(AgenteBase agente, ObiettivoGOAP obiettivo, List<AzioneGOAP> azioniDisponibili, Citta citta)
    {
        Dictionary<string, object> statoIniziale = CostruisciStatoAttuale(agente, citta);

        // Se l'obiettivo è già soddisfatto non serve alcun piano
        if (obiettivo.IsSoddisfatto(agente, citta))
            return new Queue<AzioneGOAP>();

        // A*
        List<NodoGOAP> openSet = new();
        HashSet<string> closedSet = new();

        NodoGOAP radice = new NodoGOAP(
            padre: null,
            azione: null,
            stato: statoIniziale,
            costoG: 0,
            costoH: CalcolaEuristica(statoIniziale, obiettivo.StatoDesiderato)
        );

        openSet.Add(radice);

        while (openSet.Count > 0)
        {
            NodoGOAP nodoCorrente = openSet.OrderBy(n => n.CostoF).First();
            openSet.Remove(nodoCorrente);

            if (SoddisfaCondizioni(nodoCorrente.Stato, obiettivo.StatoDesiderato))
            {
                return RicostruisciPiano(nodoCorrente);
            }

            string chiaveStato = GeneraChiaveStato(nodoCorrente.Stato);
            closedSet.Add(chiaveStato);

            // Azioni eseguibili da questo stato
            foreach (var azione in azioniDisponibili)
            {
                // Controllo se l'azione è eseguibile dato lo stato corrente del nodo
                if (SoddisfaPrecondizioni(nodoCorrente.Stato, azione.Precondizioni) &&
                    azione.VerificaPrecondizioni(agente, citta))
                {

                    Dictionary<string, object> nuovoStato = ApplicaEffetti(nodoCorrente.Stato, azione.Effetti);
                    string chiaveNuovoStato = GeneraChiaveStato(nuovoStato);

                    if (closedSet.Contains(chiaveNuovoStato))
                        continue;

                    double nuovoCostoG = nodoCorrente.CostoG + azione.Costo;
                    double euristica = CalcolaEuristica(nuovoStato, obiettivo.StatoDesiderato);

                    NodoGOAP? nodoEsistente = openSet.FirstOrDefault(n => GeneraChiaveStato(n.Stato) == chiaveNuovoStato);

                    if (nodoEsistente == null)
                    {
                        openSet.Add(new NodoGOAP(nodoCorrente, azione, nuovoStato, nuovoCostoG, euristica));
                    }
                    else if (nuovoCostoG < nodoEsistente.CostoG)
                    {
                        openSet.Remove(nodoEsistente);
                        openSet.Add(new NodoGOAP(nodoCorrente, azione, nuovoStato, nuovoCostoG, euristica));
                    }
                }
            }
        }

        // Nessun piano trovato per raggiungere l'obiettivo
        return null;
    }

    #region Helper Metodi

    private Dictionary<string, object> CostruisciStatoAttuale(AgenteBase agente, Citta citta)
    {
        var stato = new Dictionary<string, object>();

        // Fatti derivati dall'inventario: "ho almeno un'unità di questa risorsa"
        foreach (Risorsa risorsa in Enum.GetValues<Risorsa>())
        {
            agente.Inventario.TryGetValue(risorsa, out double quantita);
            stato[$"HaRisorsa_{risorsa}"] = quantita > 0;
        }

        // Fatti di contesto
        stato["InCasa"] = ControllaSeInCasa(agente, citta);
        stato["InLuogoLavoro"] = ControllaSeInLuogoLavoro(agente, citta);
        stato["InNegozio"] = ControllaSeInEdificioCommerciale(agente, citta);

        return stato;
    }

    private bool SoddisfaPrecondizioni(Dictionary<string, object> statoAttuale, Dictionary<string, object> precondizioni)
    {
        foreach (var precondizione in precondizioni)
        {
            if (!statoAttuale.TryGetValue(precondizione.Key, out object? valore))
                return false;

            if (!Equals(valore, precondizione.Value))
                return false;
        }
        return true;
    }

    private bool SoddisfaCondizioni(Dictionary<string, object> statoAttuale, Dictionary<string, object> statoDesiderato)
    {
        return SoddisfaPrecondizioni(statoAttuale, statoDesiderato);
    }

    private Dictionary<string, object> ApplicaEffetti(Dictionary<string, object> statoAttuale, Dictionary<string, object> effetti)
    {
        var nuovoStato = new Dictionary<string, object>(statoAttuale);
        foreach (var effetto in effetti)
        {
            nuovoStato[effetto.Key] = effetto.Value;
        }
        return nuovoStato;
    }

    private double CalcolaEuristica(Dictionary<string, object> statoAttuale, Dictionary<string, object> statoDesiderato)
    {
        int condizioniMancanti = 0;
        foreach (var cond in statoDesiderato)
        {
            if (!statoAttuale.TryGetValue(cond.Key, out object? val) || !Equals(val, cond.Value))
            {
                condizioniMancanti++;
            }
        }
        return condizioniMancanti;
    }

    private string GeneraChiaveStato(Dictionary<string, object> stato)
    {
        return string.Join(";", stato.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}"));
    }

    private Queue<AzioneGOAP> RicostruisciPiano(NodoGOAP nodoFinale)
    {
        Stack<AzioneGOAP> azioniInvertite = new();
        NodoGOAP? curr = nodoFinale;

        while (curr != null && curr.Azione != null)
        {
            azioniInvertite.Push(curr.Azione);
            curr = curr.Padre;
        }

        Queue<AzioneGOAP> piano = new();
        while (azioniInvertite.Count > 0)
        {
            piano.Enqueue((AzioneGOAP)azioniInvertite.Pop().Clone());
        }

        return piano;
    }

    #endregion


    #region Helper Metodi per lo Stato

    private bool ControllaSeInCasa(AgenteBase agente, Citta citta)
    {
        var casa = citta.Edifici.FirstOrDefault(e => e.Proprietario == agente.ID);
        if (casa == null) return false;

        return casa.OccupantiAttuali.Contains(agente.ID);
    }

    private bool ControllaSeInLuogoLavoro(AgenteBase agente, Citta citta)
    {
        var luogoLavoro = citta.Edifici.FirstOrDefault(e => e.OccupantiAttuali.Contains(agente.ID));
        return luogoLavoro != null && luogoLavoro.Tipo == TipoEdificio.Industriale;
    }

    private bool ControllaSeInEdificioCommerciale(AgenteBase agente, Citta citta)
    {
        var luogoNegozio = citta.Edifici.FirstOrDefault(e => e.OccupantiAttuali.Contains(agente.ID));
        return luogoNegozio != null && luogoNegozio.Tipo == TipoEdificio.Commerciale;
    }

    #endregion
}


public class NodoGOAP
{
    public NodoGOAP? Padre { get; }
    public AzioneGOAP? Azione { get; }
    public Dictionary<string, object> Stato { get; }
    public double CostoG { get; } // Costo accumulato dall'inizio
    public double CostoH { get; } // Euristica (distanza stimata dal goal)
    public double CostoF => CostoG + CostoH;

    public NodoGOAP(NodoGOAP? padre, AzioneGOAP? azione, Dictionary<string, object> stato, double costoG, double costoH)
    {
        Padre = padre;
        Azione = azione;
        Stato = new Dictionary<string, object>(stato); // Copia dello stato
        CostoG = costoG;
        CostoH = costoH;
    }
}