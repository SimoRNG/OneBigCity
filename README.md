# OneBigCity

Progetto di studio per la simulazione di una città con agenti autonomi basati sui bisogni

# Obiettivi
- Sistemi di decision-making: Studio e implementazione di concetti di Utility AI (stile The Sims) e GOAP per determinare i piani di esecuzione degli agenti
- Interazione tra sistemi: Capire come più sistemi indipendenti possano influenzarsi a vicenda all'interno di una simulazione
- Architettura a comandi: Raccolta delle richieste dei vari sistemi in un CommandBuffer, eseguito e svuotato ad ogni tick del MotoreTemporale. EventBus relegato alle notifiche UI

# Stato attuale
- Funzionamento: OK. Output minimo a console, progetto di avvio OneBigCity_Console
- Visualizzazione: Configurato in modalità hard-coded per seguire i movimenti e le azioni di un singolo agente
