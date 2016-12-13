using onlineKredit.freigabe;
using onlineKredit.logic;
using onlineKredit.web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace onlineKredit.web.Controllers
{
    public class KonsumKreditController : Controller
    {
        [HttpGet]
        public ActionResult KreditRahmen()
        {
            Debug.WriteLine("GET - KonsumKredit - KreditRahmen");

            KreditRahmenModel model = new KreditRahmenModel()
            {
                GewünschterBetrag = 25000,  // default Werte
                Laufzeit = 12   // default Werte
            };
            int id = -1;
            if (Request.Cookies["idKunde"] != null && int.TryParse(Request.Cookies["idKunde"].Value, out id))
            {
                /// lade Daten aus Datenbank
                KreditWunsch wunsch = KonsumKreditVerwaltung.KreditRahmenLaden(id);
                model.GewünschterBetrag = (int)wunsch.Betrag.Value;
                model.Laufzeit = wunsch.Laufzeit.Value;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreditRahmen(KreditRahmenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - KreditRahmen");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (Request.Cookies["idKunde"] == null)
                {
                    Kunde neuerKunde = KonsumKreditVerwaltung.ErzeugeKunde();

                    if (neuerKunde != null && KonsumKreditVerwaltung.KreditRahmenSpeichern(model.GewünschterBetrag, model.Laufzeit, neuerKunde.ID))
                    {
                        /// ich benötige für alle weiteren Schritte die ID
                        /// des angelegten Kunden. Damit ich diese bei der nächsten Action
                        /// habe, speichere ich sie für diesen Zweck in ein Cookie
                        Response.Cookies.Add(new HttpCookie("idKunde", neuerKunde.ID.ToString()));

                        /// gehe zum nächsten Schritt
                        return RedirectToAction("FinanzielleSituation");
                    }
                }
                else
                {
                    int idKunde = int.Parse(Request.Cookies["idKunde"].Value);

                    if (KonsumKreditVerwaltung.KreditRahmenSpeichern(model.GewünschterBetrag, model.Laufzeit, idKunde))
                    {
                        /// gehe zum nächsten Schritt
                        return RedirectToAction("FinanzielleSituation");
                    }
                }
            }

            /// falls der ModelState NICHT valid ist, bleibe hier und
            /// gib die Daten (falls vorhanden) wieder auf das UI
            return View(model);
        }

        [HttpGet]
        public ActionResult FinanzielleSituation()
        {
            Debug.WriteLine("GET - KonsumKredit - FinanzielleSituation");

            FinanzielleSituationModel model = new FinanzielleSituationModel()
            {
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };

            FinanzielleSituation situation = KonsumKreditVerwaltung.FinanzielleSituationLaden(model.ID_Kunde);
            if (situation != null)
            {
                model.EinkünfteAlimenteUnterhalt = (double)situation.EinkuenfteAlimenteUnterhalt.Value;
                model.NettoEinkommen = (double)situation.MonatsEinkommen.Value;
                model.RatenVerpflichtungen = (double)situation.RatenZahlungen.Value;
                model.UnterhaltsZahlungen = (double)situation.AusgabenALIUNT.Value;
                model.Wohnkosten = (double)situation.Wohnkosten.Value;
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinanzielleSituation(FinanzielleSituationModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - FinanzielleSituation");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKreditVerwaltung.FinanzielleSituationSpeichern(
                                                model.NettoEinkommen,
                                                model.RatenVerpflichtungen,
                                                model.Wohnkosten,
                                                model.EinkünfteAlimenteUnterhalt,
                                                model.UnterhaltsZahlungen,
                                                model.ID_Kunde))
                {
                    return RedirectToAction("PersönlicheDaten");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult PersönlicheDaten()
        {
            Debug.WriteLine("GET - KonsumKredit - PersönlicheDaten");

            List<BildungsModel> alleBildungsAngaben = new List<BildungsModel>();
            List<FamilienStandModel> alleFamilienStandAngaben = new List<FamilienStandModel>();
            List<IdentifikationsModel> alleIdentifikationsAngaben = new List<IdentifikationsModel>();
            List<StaatsbuergerschaftsModel> alleStaatsbuergerschaftsAngaben = new List<StaatsbuergerschaftsModel>();
            List<TitelModel> alleTitelAngaben = new List<TitelModel>();
            List<WohnartModel> alleWohnartAngaben = new List<WohnartModel>();
            List<TitelNachstehendModel> alleTitelNachstehenAngaben = new List<TitelNachstehendModel>();

            /// Lade Daten aus Logic
            foreach (var bildungsAngabe in KonsumKreditVerwaltung.BildungsAngabenLaden())
            {
                alleBildungsAngaben.Add(new BildungsModel()
                {
                    ID = bildungsAngabe.ID.ToString(),
                    Bezeichnung = bildungsAngabe.Bezeichnung
                });
            }

            foreach (var familienStand in KonsumKreditVerwaltung.FamilienStandAngabenLaden())
            {
                alleFamilienStandAngaben.Add(new FamilienStandModel()
                {
                    ID = familienStand.ID.ToString(),
                    Bezeichnung = familienStand.Bezeichnung
                });
            }
            foreach (var identifikationsAngabe in KonsumKreditVerwaltung.IdentifikiationsAngabenLaden())
            {
                alleIdentifikationsAngaben.Add(new IdentifikationsModel()
                {
                    ID = identifikationsAngabe.ID.ToString(),
                    Bezeichnung = identifikationsAngabe.Bezeichnung
                });
            }
            foreach (var land in KonsumKreditVerwaltung.LaenderLaden())
            {
                alleStaatsbuergerschaftsAngaben.Add(new StaatsbuergerschaftsModel()
                {
                    ID = land.ID,
                    Bezeichnung = land.Bezeichnung
                });
            }
            foreach (var titel in KonsumKreditVerwaltung.TitelLaden())
            {
                alleTitelAngaben.Add(new TitelModel()
                {
                    ID = titel.ID.ToString(),
                    Bezeichnung = titel.Bezeichnung
                });
            }
            foreach (var wohnart in KonsumKreditVerwaltung.WohnartenLaden())
            {
                alleWohnartAngaben.Add(new WohnartModel()
                {
                    ID = wohnart.ID.ToString(),
                    Bezeichnung = wohnart.Bezeichnung
                });
            }
            foreach (var titelNachstehend in KonsumKreditVerwaltung.TitelNachstehendLaden())
            {
                alleTitelNachstehenAngaben.Add(new TitelNachstehendModel()
                {
                    ID = titelNachstehend.ID.ToString(),
                    Bezeichnung = titelNachstehend.Bezeichnung
                });
            }


            PersönlicheDatenModel model = new PersönlicheDatenModel()
            {
                AlleBildungAngaben = alleBildungsAngaben,
                AlleFamilienStandAngaben = alleFamilienStandAngaben,
                AlleIdentifikationsAngaben = alleIdentifikationsAngaben,
                AlleStaatsbuergerschaftsAngaben = alleStaatsbuergerschaftsAngaben,
                AlleTitelAngaben = alleTitelAngaben,
                AlleTitelNachstehendAngaben = alleTitelNachstehenAngaben,
                AlleWohnartAngaben = alleWohnartAngaben,
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };

            Kunde kunde = KonsumKreditVerwaltung.PersönlicheDatenLaden(model.ID_Kunde);
            if (kunde != null)
            {
                model.Geschlecht = !string.IsNullOrEmpty(kunde.Gechlecht) && kunde.Gechlecht == "m" ? onlineKredit.web.Models.Geschlecht.Männlich : onlineKredit.web.Models.Geschlecht.Weiblich;
                model.Vorname = kunde.Vorname;
                model.Nachname = kunde.Nachname;
                model.ID_Titel = kunde.FKTitel.HasValue ? kunde.FKTitel.Value : 0;
                model.ID_TitelNachstehend = kunde.FKTitelNachstehend.HasValue ? kunde.FKTitelNachstehend.Value : 0;
                //model.GeburtsDatum = DateTime.Now;
                model.ID_Staatsbuergerschaft = kunde.FKStaatsangehoerigkeit;
                model.ID_Familienstand = kunde.FKFamilienstand.HasValue ? kunde.FKFamilienstand.Value : 0;
                model.ID_Wohnart = kunde.FKWohnart.HasValue ? kunde.FKWohnart.Value : 0;
                model.ID_Bildung = kunde.FKSchulabschluss.HasValue ? kunde.FKSchulabschluss.Value : 0;
                model.ID_Identifikationsart = kunde.FKIdentifikationsArt.HasValue ? kunde.FKIdentifikationsArt.Value : 0;
                model.IdentifikationsNummer = kunde.IdentifikationsNummer;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersönlicheDaten(PersönlicheDatenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - PersönlicheDaten");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKreditVerwaltung.PersönlicheDatenSpeichern(
                                                model.ID_Titel,
                                                model.Geschlecht == onlineKredit.web.Models.Geschlecht.Männlich ? "m" : "w",
                                                model.GeburtsDatum,
                                                model.Vorname,
                                                model.Nachname,
                                                model.ID_TitelNachstehend,
                                                model.ID_Bildung,
                                                model.ID_Familienstand,
                                                model.ID_Identifikationsart,
                                                model.IdentifikationsNummer,
                                                model.ID_Staatsbuergerschaft,
                                                model.ID_Wohnart,
                                                model.ID_Kunde))
                {
                    return RedirectToAction("Arbeitgeber");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Arbeitgeber()
        {
            Debug.WriteLine("GET - KonsumKredit - Arbeitgeber");

            List<BeschaeftigungsArtModel> alleBeschaeftigungen = new List<BeschaeftigungsArtModel>();
            List<BrancheModel> alleBranchen = new List<BrancheModel>();

            foreach (var branche in KonsumKreditVerwaltung.BranchenLaden())
            {
                alleBranchen.Add(new BrancheModel()
                {
                    ID = branche.ID.ToString(),
                    Bezeichnung = branche.Bezeichnung
                });
            }

            foreach (var beschaeftigungsArt in KonsumKreditVerwaltung.BeschaeftigungsArtenLaden())
            {
                alleBeschaeftigungen.Add(new BeschaeftigungsArtModel()
                {
                    ID = beschaeftigungsArt.ID.ToString(),
                    Bezeichnung = beschaeftigungsArt.Bezeichnung
                });
            }

            ArbeitgeberModel model = new ArbeitgeberModel()
            {
                AlleBeschaeftigungen = alleBeschaeftigungen,
                AlleBranchen = alleBranchen,
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };

            Arbeitgeber arbeitgeberDaten = KonsumKreditVerwaltung.ArbeitgeberAngabenLaden(model.ID_Kunde);
            if (arbeitgeberDaten != null)
            {
                model.BeschäftigtSeit = arbeitgeberDaten.BeschaeftigtSeit.Value.ToString("MM.yyyy");
                model.FirmenName = arbeitgeberDaten.Firma;
                model.ID_BeschäftigungsArt = arbeitgeberDaten.FKBeschaeftigungsArt.Value; ;
                model.ID_Branche = arbeitgeberDaten.FKBranche.Value;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Arbeitgeber(ArbeitgeberModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - Arbeitgeber");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKreditVerwaltung.ArbeitgeberAngabenSpeichern(
                                                model.FirmenName,
                                                model.ID_BeschäftigungsArt,
                                                model.ID_Branche,
                                                model.BeschäftigtSeit,
                                                model.ID_Kunde))
                {
                    return RedirectToAction("KontoInformationen");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult KontoInformationen()
        {
            Debug.WriteLine("GET - KonsumKredit - KontoInformationen");

            KontoInformationenModel model = new KontoInformationenModel()
            {
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };

            KontoDaten daten = KonsumKreditVerwaltung.KontoInformationenLaden(model.ID_Kunde);
            if (daten != null)
            {
                model.BankName = daten.BankName;
                model.BIC = daten.BIC;
                model.IBAN = daten.IBAN;
                model.NeuesKonto = !daten.IstDB_Kunde.Value;
            }

            KreditKarte kkDaten = KonsumKreditVerwaltung.KreditKartenDatenLaden(model.ID_Kunde);
            if (kkDaten != null)
            {
                model.KreditKartenInhaber = kkDaten.Inhaber;
                model.KreditKartenNummer = kkDaten.Nummer;
                model.KreditKartenGültigBis = kkDaten.GültigBis.ToString("MM.yyyy");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KontoInformationen(KontoInformationenModel model, string auswahl)
        {
            Debug.WriteLine("POST - KonsumKredit - KontoInformationen");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                bool erfolgreich = false;

                if (auswahl == "neu" || auswahl == "bestehend")
                {
                    erfolgreich = KonsumKreditVerwaltung.KontoInformationenSpeichern(
                                    model.BankName,
                                    model.IBAN,
                                    model.BIC,
                                    auswahl == "neu",
                                    model.ID_Kunde);
                }
                else if (auswahl == "kreditkarte")
                {
                    erfolgreich = KonsumKreditVerwaltung.KreditKartenDatenSpeichern(
                                    model.KreditKartenInhaber,
                                    model.KreditKartenNummer,
                                    DateTime.Parse(model.KreditKartenGültigBis),
                                    model.ID_Kunde);
                }

                if (erfolgreich)
                {
                    return RedirectToAction("Zusammenfassung");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Zusammenfassung()
        {
            Debug.WriteLine("GET - KonsumKredit - Zusammenfassung");

            /// ermittle für diese Kunden_ID
            /// alle gespeicherten Daten (ACHTUNG! das sind viele ....)
            /// gib Sie alle in das ZusammenfassungsModel (bzw. die UNTER-Modelle) 
            /// hinein.
            ZusammenfassungModel model = new ZusammenfassungModel();
            model.ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value);

            /// lädt ALLE Daten zu diesem Kunden (also auch die angehängten/referenzierten
            /// Entities) aus der DB
            Kunde aktKunde = KonsumKreditVerwaltung.KundeLaden(model.ID_Kunde);

            model.GewünschterBetrag = (int)aktKunde.KreditWunsch.Betrag.Value;
            model.Laufzeit = aktKunde.KreditWunsch.Laufzeit.Value;

            model.NettoEinkommen = (double)aktKunde.FinanzielleSituation.MonatsEinkommen.Value;
            model.Wohnkosten = (double)aktKunde.FinanzielleSituation.Wohnkosten.Value;
            model.EinkünfteAlimenteUnterhalt = (double)aktKunde.FinanzielleSituation.EinkuenfteAlimenteUnterhalt.Value;
            model.UnterhaltsZahlungen = (double)aktKunde.FinanzielleSituation.AusgabenALIUNT.Value;
            model.RatenVerpflichtungen = (double)aktKunde.FinanzielleSituation.RatenZahlungen.Value;

            model.Geschlecht = aktKunde.Gechlecht == "m" ? "Herr" : "Frau";
            model.Vorname = aktKunde.Vorname;
            model.Nachname = aktKunde.Nachname;
            model.Titel = aktKunde.Titel?.Bezeichnung;
            model.TitelNachstehend = aktKunde.TitelNachstehend?.Bezeichnung;
            model.GeburtsDatum = DateTime.Now;
            model.Staatsbuergerschaft = aktKunde.Staatsangehoerigkeit?.Bezeichnung;
            model.AnzahlUnterhaltspflichtigeKinder = -1;
            model.Familienstand = aktKunde.Familienstand?.Bezeichnung;
            model.Wohnart = aktKunde.Wohnart?.Bezeichnung;
            model.Bildung = aktKunde.Schulabschluss?.Bezeichnung;
            model.Identifikationsart = aktKunde.IdentifikationsArt?.Bezeichnung;
            model.IdentifikationsNummer = aktKunde.IdentifikationsNummer;

            model.FirmenName = aktKunde.Arbeitgeber?.Firma;
            model.BeschäftigungsArt = aktKunde.Arbeitgeber?.BeschaeftigungsArt?.Bezeichnung;
            model.Branche = aktKunde.Arbeitgeber?.Branche?.Bezeichnung;
            model.BeschäftigtSeit = aktKunde.Arbeitgeber?.BeschaeftigtSeit.Value.ToShortDateString();

            model.Strasse = aktKunde.KontaktDaten?.Strasse;
            model.Hausnummer = aktKunde.KontaktDaten?.Hausnummer;
            model.Ort = aktKunde.KontaktDaten?.Ort.PLZ;
            model.Mail = aktKunde.KontaktDaten?.EMail;
            model.TelefonNummer = aktKunde.KontaktDaten?.Telefonnummer;

            model.NeuesKonto = (bool)aktKunde.KontoDaten?.IstDB_Kunde.Value;
            model.BankName = aktKunde.KontoDaten?.BankName;
            model.IBAN = aktKunde.KontoDaten?.IBAN;
            model.BIC = aktKunde.KontoDaten?.BIC;

            /// gib model an die View
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bestätigung(int id, bool? bestätigt)
        {
            if (bestätigt.HasValue && bestätigt.Value)
            {
                Debug.WriteLine("POST - KonsumKredit - Bestätigung");
                Debug.Indent();


                //int idKunde = int.Parse(Request.Cookies["idKunde"].Value);
                Kunde aktKunde = KonsumKreditVerwaltung.KundeLaden(id);
                Response.Cookies.Remove("idKunde");

                bool istFreigegeben = KreditFreigabe.FreigabeErteilt(
                                                          aktKunde.Gechlecht,
                                                            aktKunde.Vorname,
                                                            aktKunde.Nachname,
                                                            aktKunde.Familienstand.Bezeichnung,
                                                            (double)aktKunde.FinanzielleSituation.MonatsEinkommen,
                                                            (double)aktKunde.FinanzielleSituation.Wohnkosten,
                                                            (double)aktKunde.FinanzielleSituation.EinkuenfteAlimenteUnterhalt,
                                                            (double)aktKunde.FinanzielleSituation.AusgabenALIUNT,
                                                            (double)aktKunde.FinanzielleSituation.RatenZahlungen);

                /// Rüfe Service/DLL auf und prüfe auf Kreditfreigabe
                Debug.WriteLine($"Kreditfreigabe {(istFreigegeben ? "" : "nicht")}erteilt!");

                Debug.Unindent();
                return RedirectToAction("Index", "Freigabe", new { erfolgreich = istFreigegeben });

            }
            else
            {
                return RedirectToAction("Zusammenfassung");
            }
        }

        [HttpGet]
        public ActionResult WerbeBanner()
        {
            Debug.WriteLine("GET - KonsumKredit - WerbeBanner");

            /// lade aus der DB die letzten 10 Kreditanträge
            List<Kunde> alleKunden = KonsumKreditVerwaltung.LetzteKundenLaden();
            List<ZusammenfassungModel> alleKundenModel = new List<ZusammenfassungModel>();

            foreach (var aktKunde in alleKunden)
            {
                ZusammenfassungModel model = new ZusammenfassungModel();

                model.ID_Kunde = aktKunde.ID;

                model.GewünschterBetrag = (int)aktKunde.KreditWunsch.Betrag.Value;
                model.Laufzeit = aktKunde.KreditWunsch.Laufzeit.Value;

                model.NettoEinkommen = (double)aktKunde.FinanzielleSituation.MonatsEinkommen.Value;
                model.Wohnkosten = (double)aktKunde.FinanzielleSituation.Wohnkosten.Value;
                model.EinkünfteAlimenteUnterhalt = (double)aktKunde.FinanzielleSituation.EinkuenfteAlimenteUnterhalt.Value;
                model.UnterhaltsZahlungen = (double)aktKunde.FinanzielleSituation.AusgabenALIUNT.Value;
                model.RatenVerpflichtungen = (double)aktKunde.FinanzielleSituation.RatenZahlungen.Value;

                model.Geschlecht = aktKunde.Gechlecht == "m" ? "Herr" : "Frau";
                model.Vorname = aktKunde.Vorname;
                model.Nachname = aktKunde.Nachname;
                model.Titel = aktKunde.Titel?.Bezeichnung;
                model.TitelNachstehend = aktKunde.TitelNachstehend?.Bezeichnung;
                model.GeburtsDatum = DateTime.Now;
                model.Staatsbuergerschaft = aktKunde.Staatsangehoerigkeit?.Bezeichnung;
                model.AnzahlUnterhaltspflichtigeKinder = -1;
                model.Familienstand = aktKunde.Familienstand?.Bezeichnung;
                model.Wohnart = aktKunde.Wohnart?.Bezeichnung;
                model.Bildung = aktKunde.Schulabschluss?.Bezeichnung;
                model.Identifikationsart = aktKunde.IdentifikationsArt?.Bezeichnung;
                model.IdentifikationsNummer = aktKunde.IdentifikationsNummer;

                model.FirmenName = aktKunde.Arbeitgeber?.Firma;
                model.BeschäftigungsArt = aktKunde.Arbeitgeber?.BeschaeftigungsArt?.Bezeichnung;
                model.Branche = aktKunde.Arbeitgeber?.Branche?.Bezeichnung;
                model.BeschäftigtSeit = aktKunde.Arbeitgeber?.BeschaeftigtSeit.Value.ToShortDateString();

                model.Strasse = aktKunde.KontaktDaten?.Strasse;
                model.Hausnummer = aktKunde.KontaktDaten?.Hausnummer;
                model.Ort = aktKunde.KontaktDaten?.Ort.PLZ;
                model.Mail = aktKunde.KontaktDaten?.EMail;
                model.TelefonNummer = aktKunde.KontaktDaten?.Telefonnummer;

                model.NeuesKonto = (bool)aktKunde.KontoDaten?.IstDB_Kunde.Value;
                model.BankName = aktKunde.KontoDaten?.BankName;
                model.IBAN = aktKunde.KontoDaten?.IBAN;
                model.BIC = aktKunde.KontoDaten?.BIC;

                alleKundenModel.Add(model);
            }

            return PartialView(alleKundenModel);
        }
    }
}