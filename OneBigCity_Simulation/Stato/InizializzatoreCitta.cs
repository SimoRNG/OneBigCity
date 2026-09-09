using OneBigCity_Model.Classi.Entita;
using OneBigCity_Model.Classi.Entita.Agenti;
using OneBigCity_Model.Classi.Entita.Edifici;
using OneBigCity_Model.Classi.Geografia;
using OneBigCity_Simulation.DTO;
using OneBigCity_Simulation.Utilita;

namespace OneBigCity_Simulation.Stato;

public class InizializzatoreCitta
{
    private readonly ICaricatoreEntita _caricatoreEntita;
    private readonly ICaricatoreMappa _caricatoreMappa;
    public InizializzatoreCitta(ICaricatoreEntita caricatoreEntita, ICaricatoreMappa caricatoreMappa)
    {
        _caricatoreEntita = caricatoreEntita;
        _caricatoreMappa = caricatoreMappa;
    }

    public Citta CaricaCitta(string percorsoMappa, string percorsoEntita, string nomeCitta = "NuovaCitta")
    {
        if (string.IsNullOrWhiteSpace(percorsoMappa) || !File.Exists(percorsoMappa)) throw new ArgumentException("Percorso file mappa non valido");
        if (string.IsNullOrWhiteSpace(percorsoEntita) || !File.Exists(percorsoEntita)) throw new ArgumentException("Percorso file entità non valido");

        MappaGriglia mappaCitta = _caricatoreMappa.CaricaMappa(percorsoMappa);
        EntitaIniziali entitaIniziali = _caricatoreEntita.CaricaEntita(percorsoEntita);

        Citta citta = new Citta(nomeCitta, mappaCitta);

        // EDIFICI
        foreach (EdificioBase edificio in entitaIniziali.Edifici)
        {
            citta.Edifici.Add(edificio);

            // Popola il dizionario di lookup O(1) per ogni cella occupata dall'edificio
            foreach (Punto3D coord in edificio.CoordinateOccupate)
            {
                if (mappaCitta.IsPosizioneValida(coord))
                {
                    citta.MappaEdifici[coord] = edificio;
                }
                else
                {
                    Console.WriteLine($"[AVVISO] L'edificio '{edificio.Nome}' ha coordinate fuori mappa: {coord}");
                }
            }
        }

        // PERSONE
        foreach (Persona persona in entitaIniziali.Persone)
        {
            CellaGriglia? cellaOspite = mappaCitta.OttieniCella(persona.Posizione);

            if (cellaOspite == null)
            {
                Console.WriteLine($"[AVVISO] La persona '{persona.Nome} {persona.Cognome}' è fuori dai confini della mappa. Posizione reset a (0,0,0).");
                persona.Posizione = new Punto3D(0, 0, 0);
            }
            else if (!cellaOspite.IsCalpestabile)
            {
                // TODO - Algoritmo BFS per cercare la prima cella disponibile su cui spostare la persona
                Punto3D nuovaPosizioneValida = AlgoritmiPathFinding.CercaPrimaCellaCalpestabileBFS(mappaCitta, cellaOspite);
                persona.Posizione = nuovaPosizioneValida;
                Console.WriteLine($"[AVVISO] La persona '{persona.Nome} {persona.Cognome}' è su una cella non calpestabile. Nuova posizione assegnata: {nuovaPosizioneValida.ToString()}");

            }

            citta.Abitanti.Add(persona);
        }

        return citta;
    }
}
