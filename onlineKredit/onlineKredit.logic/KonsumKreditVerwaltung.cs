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
                        Nachname = "anonym",
                        Gechlecht = "x"
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
        /// Lädt den Kreditrahmen für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kreditrahmens</param>
        /// <returns>der Kreditwunsch für die übergebene ID</returns>
        public static KreditWunsch KreditRahmenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditRahmenLaden");
            Debug.Indent();

            KreditWunsch wunsch = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    wunsch = context.AlleKreditWünsche.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("KreditRahmen geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditRahmenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return wunsch;
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
                        /// ermittle ob es bereits einen Kreditwunsch gibt
                        KreditWunsch kreditWunsch = context.AlleKreditWünsche.FirstOrDefault(x => x.ID == idKunde);

                        /// nur wenn noch keiner existiert
                        if (kreditWunsch == null)
                        {
                            /// lege einen neuen an
                            kreditWunsch = new KreditWunsch();
                            context.AlleKreditWünsche.Add(kreditWunsch);
                        }

                        kreditWunsch.Betrag = (decimal)kreditBetrag;
                        kreditWunsch.Laufzeit = laufzeit;
                        kreditWunsch.ID = idKunde;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
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

        /// <summary>
        /// Lädt die FinanzielleSituation für die übergebene ID
        /// </summary>
        /// <param name="id">die id der zu ladenden FinanzielleSituation</param>
        /// <returns>die FinanzielleSituation für die übergebene ID</returns>
        public static FinanzielleSituation FinanzielleSituationLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FinanzielleSituationLaden");
            Debug.Indent();

            FinanzielleSituation finanzielleSituation = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    finanzielleSituation = context.AlleFinanzielleSituationen.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("FinanzielleSituation geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FinanzielleSituationLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return finanzielleSituation;
        }


        /// <summary>
        /// Speichert die Daten aus der Finanziellen Situation zu einem Kunden
        /// </summary>
        /// <param name="nettoEinkommen">das Netto Einkommen des Kunden</param>
        /// <param name="ratenVerpflichtungen">Raten Verpflichtungen des Kunden</param>
        /// <param name="wohnkosten">die Wohnkosten des Kunden</param>
        /// <param name="einkünfteAlimenteUnterhalt">Einkünfte aus Alimente und Unterhalt</param>
        /// <param name="unterhaltsZahlungen">Zahlungen für Alimente und Unterhalt</param>
        /// <param name="idKunde">die id des Kunden</param>
        /// <returns>true wenn die finanzielle Situation erfolgreich gespeichert werden konnte, ansonsten false</returns>
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
                        FinanzielleSituation finanzielleSituation = context.AlleFinanzielleSituationen.FirstOrDefault(x => x.ID == idKunde);

                        if (finanzielleSituation == null)
                        {
                            finanzielleSituation = new FinanzielleSituation();
                            context.AlleFinanzielleSituationen.Add(finanzielleSituation);
                        }

                        finanzielleSituation.MonatsEinkommen = (decimal)nettoEinkommen;
                        finanzielleSituation.AusgabenALIUNT = (decimal)unterhaltsZahlungen;
                        finanzielleSituation.EinkuenfteAlimenteUnterhalt = (decimal)einkünfteAlimenteUnterhalt;
                        finanzielleSituation.Wohnkosten = (decimal)wohnkosten;
                        finanzielleSituation.RatenZahlungen = (decimal)ratenVerpflichtungen;
                        finanzielleSituation.ID = idKunde;

                        int anzahlZeilenBetroffen = context.SaveChanges();
                        erfolgreich = anzahlZeilenBetroffen >= 0;
                        Debug.WriteLine($"{anzahlZeilenBetroffen} FinanzielleSituation gespeichert!");
                    }
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

        /// <summary>
        /// Liefert alle Branchen zurück
        /// </summary>
        /// <returns>alle Branchen oder null bei einem Fehler</returns>
        public static List<Branche> BranchenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - BranchenLaden");
            Debug.Indent();

            List<Branche> alleBranchen = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleBranchen = context.AlleBranchen.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in BranchenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleBranchen;
        }

        /// <summary>
        /// Liefert alle Beschaefitgungsarten zurück
        /// </summary>
        /// <returns>alle Beschaefitgungsarten oder null bei einem Fehler</returns>
        public static List<Beschaeftigungsart> BeschaeftigungsArtenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - Beschaeftigungsart");
            Debug.Indent();

            List<Beschaeftigungsart> alleBeschaeftigungsArten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleBeschaeftigungsArten = context.AlleBeschaeftigungsarten.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in Beschaeftigungsart");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleBeschaeftigungsArten;
        }

        /// <summary>
        /// Liefert alle Schulabschlüsse zurück
        /// </summary>
        /// <returns>alle Schulabschlüsse oder null bei einem Fehler</returns>
        public static List<Schulabschluss> BildungsAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - BildungsAngabenLaden");
            Debug.Indent();

            List<Schulabschluss> alleAbschlüsse = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleAbschlüsse = context.AlleSchulabschlüsse.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in BildungsAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleAbschlüsse;
        }

        /// <summary>
        /// Liefert alle FamilienStand zurück
        /// </summary>
        /// <returns>alle FamilienStand oder null bei einem Fehler</returns>
        public static List<FamilienStand> FamilienStandAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FamilienStandAngabenLaden");
            Debug.Indent();

            List<FamilienStand> alleFamilienStandsAngaben = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleFamilienStandsAngaben = context.AlleFamilienStandAngaben.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FamilienStandAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleFamilienStandsAngaben;
        }

        /// <summary>
        /// Liefert alle Länder zurück
        /// </summary>
        /// <returns>alle Länder oder null bei einem Fehler</returns>
        public static List<Land> LaenderLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - LaenderLaden");
            Debug.Indent();

            List<Land> alleLänder = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleLänder = context.AlleLänder.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in LaenderLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleLänder;
        }

        /// <summary>
        /// Liefert alle Wohnarten zurück
        /// </summary>
        /// <returns>alle Wohnarten oder null bei einem Fehler</returns>
        public static List<Wohnart> WohnartenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - WohnartenLaden");
            Debug.Indent();

            List<Wohnart> alleWohnarten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleWohnarten = context.AlleWohnarten.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in WohnartenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleWohnarten;
        }

        public static KreditKarte KreditKartenDatenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditKartenDatenLaden");
            Debug.Indent();

            KreditKarte kkDaten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    kkDaten = context.AlleKreditKarten.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("KreditKartenDatenLaden geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditKartenDatenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return kkDaten;
        }

        /// <summary>
        /// Liefert alle IdentifikatikonsArt zurück
        /// </summary>
        /// <returns>alle IdentifikatikonsArt oder null bei einem Fehler</returns>
        public static List<IdentifikationsArt> IdentifikiationsAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - IdentifikiationsAngabenLaden");
            Debug.Indent();

            List<IdentifikationsArt> alleIdentifikationsArten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleIdentifikationsArten = context.AlleIdentifikationsArten.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in IdentifikiationsAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleIdentifikationsArten;
        }

        /// <summary>
        /// Lädt zu einer übergebenen ID alle Informationen zu diesem Kunden aus der DB
        /// </summary>
        /// <param name="iKunde">die ID des zu landenden Kunden</param>
        /// <returns>alle Daten aus der DB zu diesem Kunden</returns>
        public static Kunde KundeLaden(int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KundeLaden");
            Debug.Indent();

            Kunde aktuellerKunde = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    aktuellerKunde = context.AlleKunden
                        .Include("Arbeitgeber")
                        .Include("Arbeitgeber.BeschaeftigungsArt")
                        .Include("Arbeitgeber.Branche")
                        .Include("Familienstand")
                        .Include("FinanzielleSituation")
                        .Include("IdentifikationsArt")
                        .Include("KontaktDaten")
                        .Include("KontoDaten")
                        .Include("KreditWunsch")
                        .Include("Schulabschluss")
                        .Include("Titel")
                        .Include("TitelNachstehend")
                        .Include("Wohnart")
                        .Include("Staatsangehoerigkeit")
                        .Where(x => x.ID == idKunde).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KundeLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return aktuellerKunde;
        }

        /// <summary>
        /// Liefert alle Titel zurück
        /// </summary>
        /// <returns>alle Titel oder null bei einem Fehler</returns>
        public static List<Titel> TitelLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - TitelLaden");
            Debug.Indent();

            List<Titel> alleTitel = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleTitel = context.AlleTitel.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in TitelLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleTitel;
        }

        /// <summary>
        /// Liefert alle TitelNachstehend zurück
        /// </summary>
        /// <returns>alle TitelNachstehend oder null bei einem Fehler</returns>
        public static List<TitelNachstehend> TitelNachstehendLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - TitelNachstehendLaden");
            Debug.Indent();

            List<TitelNachstehend> alleTitelNachstehend = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    alleTitelNachstehend = context.AlleTitelNachstehend.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in TitelNachstehendLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleTitelNachstehend;
        }

        /// <summary>
        /// Lädt den Kunden für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kunden</param>
        /// <returns>der Kunde für die übergebene ID</returns>
        public static Kunde PersönlicheDatenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - PersönlicheDatenLaden");
            Debug.Indent();

            Kunde persönlicheDaten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    persönlicheDaten = context.AlleKunden.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("PersönlicheDatenLaden geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in PersönlicheDatenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return persönlicheDaten;
        }

        /// <summary>
        /// Speichert die Daten für die übergebene idKunde
        /// </summary>
        /// <param name="idTitel">der Titel des Kunden</param>
        /// <param name="geschlecht">das Geschlecht des Kunden</param>
        /// <param name="geburtsDatum">das Geburtsdatum des Kunden</param>
        /// <param name="vorname">der Vorname des Kunden</param>
        /// <param name="nachname">der Nachname des Kunden</param>
        /// <param name="idTitelNachstehend">der nachstehende Titel des Kunden</param>
        /// <param name="idBildung">die Bildung des Kunden</param>
        /// <param name="idFamilienstand">der Familienstand des Kunden</param>
        /// <param name="idIdentifikationsart">die Identifikations des Kunden</param>
        /// <param name="identifikationsNummer">der Identifikations-Nummer des Kunden</param>
        /// <param name="idStaatsbuergerschaft">die Staatsbürgerschaft des Kunden</param>
        /// <param name="idWohnart">die Wohnart des Kunden</param>
        /// <param name="idKunde">die ID des Kunden</param>
        /// <returns>true wenn das Anpassen der Werte erfolgreich war, ansonsten false</returns>
        public static bool PersönlicheDatenSpeichern(int? idTitel, string geschlecht, DateTime geburtsDatum, string vorname, string nachname, int? idTitelNachstehend, int idBildung, int idFamilienstand, int idIdentifikationsart, string identifikationsNummer, string idStaatsbuergerschaft, int idWohnart, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - PersönlicheDatenSpeichern");
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
                        aktKunde.Vorname = vorname;
                        aktKunde.Nachname = nachname;
                        aktKunde.FKFamilienstand = idFamilienstand;
                        aktKunde.FKSchulabschluss = idBildung;
                        aktKunde.FKStaatsangehoerigkeit = idStaatsbuergerschaft;
                        aktKunde.FKTitel = idTitel;
                        aktKunde.FKTitelNachstehend = idTitelNachstehend;
                        aktKunde.FKIdentifikationsArt = idIdentifikationsart;
                        aktKunde.IdentifikationsNummer = identifikationsNummer;
                        aktKunde.Gechlecht = geschlecht;
                        aktKunde.FKWohnart = idWohnart;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} PersönlicheDaten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in PersönlicheDatenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        /// <summary>
        /// Lädt den Kreditrahmen für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kreditrahmens</param>
        /// <returns>der Kreditwunsch für die übergebene ID</returns>
        public static Arbeitgeber ArbeitgeberAngabenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ArbeitgeberAngabenLaden");
            Debug.Indent();

            Arbeitgeber arbeitGeber = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    arbeitGeber = context.AlleArbeitgeber.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("ArbeitgeberAngaben geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ArbeitgeberAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return arbeitGeber;
        }

        /// <summary>
        /// Speichert die Angaben des Arbeitsgebers zu einem Kunden
        /// </summary>
        /// <param name="firmenName">der Firmenname des Arbeitgeber des Kunden</param>
        /// <param name="idBeschäftigungsArt">die Beschäftigungsart des Arbeitgeber des Kunden</param>
        /// <param name="idBranche">die Branche des Arbeitgeber des Kunden</param>
        /// <param name="beschäftigtSeit"> BeschäftigtSeit Wert des Kunden</param>
        /// <param name="idKunde">die ID des Kunden</param>
        /// <returns>true wenn das Speichern erfolgreich war, ansonsten false</returns>
        public static bool ArbeitgeberAngabenSpeichern(string firmenName, int idBeschäftigungsArt, int idBranche, string beschäftigtSeit, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ArbeitgeberAngabenSpeichern");
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
                        Arbeitgeber arbeitgeber = context.AlleArbeitgeber.FirstOrDefault(x => x.ID == idKunde);

                        if (arbeitgeber==null)
                        {
                            arbeitgeber = new Arbeitgeber();
                            context.AlleArbeitgeber.Add(arbeitgeber);
                        }
                        arbeitgeber.BeschaeftigtSeit = DateTime.Parse(beschäftigtSeit);
                        arbeitgeber.FKBranche = idBranche;
                        arbeitgeber.FKBeschaeftigungsArt = idBeschäftigungsArt;
                        arbeitgeber.Firma = firmenName;                        
                        aktKunde.Arbeitgeber = arbeitgeber;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} ArbeitgeberDaten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ArbeitgeberAngabenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        /// <summary>
        /// Lädt die KontoDaten für die übergebene ID
        /// </summary>
        /// <param name="id">die id der zu ladenden KontoDaten</param>
        /// <returns>die KontoDaten für die übergebene ID</returns>
        public static KontoDaten KontoInformationenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontoInformationenLaden");
            Debug.Indent();

            KontoDaten kontoDaten = null;

            try
            {
                using (var context = new OnlineKredit())
                {
                    kontoDaten = context.AlleKontoDaten.Where(x => x.ID == id).FirstOrDefault();
                    Debug.WriteLine("KontoInformationen geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontoInformationenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return kontoDaten;
        }

        public static bool KontoInformationenSpeichern(string bankName, string iban, string bic, bool neuesKonto, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontoInformationenSpeichern");
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
                        KontoDaten kontoDaten = context.AlleKontoDaten.FirstOrDefault(x => x.ID == idKunde);

                        if (kontoDaten== null)
                        {
                            kontoDaten = new KontoDaten();
                            context.AlleKontoDaten.Add(kontoDaten);
                        }
                        kontoDaten.BankName = bankName;
                        kontoDaten.IBAN = iban;
                        kontoDaten.BIC = bic;
                        kontoDaten.IstDB_Kunde = !neuesKonto;
                        kontoDaten.ID = idKunde;                        
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} Konto-Daten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontoInformationenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        public static bool KreditKartenDatenSpeichern(string inhaber, string nummer, DateTime gültigBis, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditKartenDatenSpeichern");
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
                        KreditKarte kreditKartenDaten = context.AlleKreditKarten.FirstOrDefault(x => x.ID == idKunde);

                        if (kreditKartenDaten == null)
                        {
                            kreditKartenDaten = new KreditKarte();
                            context.AlleKreditKarten.Add(kreditKartenDaten);
                        }
                        kreditKartenDaten.Inhaber = inhaber;
                        kreditKartenDaten.Nummer= nummer;
                        kreditKartenDaten.GültigBis = gültigBis;
                        kreditKartenDaten.ID = idKunde;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} KreditKarten-Daten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditKartenDatenSpeichern");
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
