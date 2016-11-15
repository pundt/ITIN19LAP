using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineKredit.logic
{
    public class KonsumKreditVerwaltung
    {
        /// <summary>
        /// Erzeugt einen "leeren" dummy Kunden
        /// zu dem in Folge alle Konsumkredit Daten
        /// verknüpft werden können.
        /// </summary>
        /// <returns>einen leeren Kunden wenn erfolgreich, ansonsten null</returns>
        public static Kunde ErzeugeKunde()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ErzeugeKunde");
            Debug.Indent();

            Kunde neuerKunde = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    neuerKunde = new logic.Kunde()
                    {
                        Vorname = "anonym",
                        Nachname = "anonym"
                    };
                    context.AlleKunden.Add(neuerKunde);

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    Debug.WriteLine($"{anzahlZeilenBetroffen} Kunden angelegt!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ErzeugeKunde");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return neuerKunde;
        }

        /// <summary>
        /// Speichert zu einer übergebenene ID_Kunde den Wunsch Kredit und dessen Laufzeit ab
        /// </summary>
        /// <param name="kreditBetrag">die Höhe des gewünschten Kredits</param>
        /// <param name="laufzeit">die Laufzeit des gewünschten Kredits</param>
        /// <param name="idKunde">die ID des Kunden zu dem die Angaben gespeichert werden sollen</param>
        /// <returns>true wenn Eintragung gespeichert werden konnte und der Kunde existiert, ansonsten false</returns>
        public static bool KreditRahmenSpeichern(double kreditBetrag, int laufzeit, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditRahmenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new OnlineKredit())
                {

                    /// speichere zum Kunden die Angaben
                    Kunde aktKunde = context.AlleKunden.Where(x => x.ID == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        KreditWunsch neuerKreditWunsch = new KreditWunsch()
                        {
                            Betrag = (decimal)kreditBetrag,
                            Laufzeit = laufzeit,
                            ID = idKunde
                        };

                        context.AlleKreditWünsche.Add(neuerKreditWunsch);
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} KreditRahmen gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditRahmenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        public static bool FinanzielleSituationSpeichern(double nettoEinkommen, double ratenVerpflichtungen, double wohnkosten, double einkünfteAlimenteUnterhalt, double unterhaltsZahlungen, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FinanzielleSituationSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new OnlineKredit())
                {

                    /// speichere zum Kunden die Angaben
                    Kunde aktKunde = context.AlleKunden.Where(x => x.ID == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        FinanzielleSituation neueFinanzielleSituation = new FinanzielleSituation()
                        {
                            MonatsEinkommen = (decimal)nettoEinkommen,
                            AusgabenALIUNT = (decimal)unterhaltsZahlungen,
                            EinkuenfteAlimenteUnterhalt = (decimal)einkünfteAlimenteUnterhalt,
                            Wohnkosten = (decimal)wohnkosten,
                            RatenZahlungen = (decimal)ratenVerpflichtungen,
                            ID = idKunde
                        };

                        context.AlleFinanzielleSituationen.Add(neueFinanzielleSituation);
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} FinanzielleSituation gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FinanzielleSituation");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }
    }
}
