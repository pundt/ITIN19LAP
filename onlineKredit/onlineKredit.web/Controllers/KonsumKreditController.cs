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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreditRahmen(KreditRahmenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - KreditRahmen");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
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
                    Bezeichnung = identifikationsAngabe.IdentifikationsArt
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
                                                model.Geschlecht == Geschlecht.Männlich ? "m" : "w",
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Arbeitgeber(ArbeitgeberModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - Arbeitgeber");
            return View();
        }

        [HttpGet]
        public ActionResult KontoInformationen()
        {
            Debug.WriteLine("GET - KonsumKredit - KontoInformationen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KontoInformationen(KontoInformationenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - KontoInformationen");
            return View();
        }

        [HttpGet]
        public ActionResult Zusammenfassung()
        {
            Debug.WriteLine("GET - KonsumKredit - Zusammenfassung");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Zusammenfassung(ZusammenfassungModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - Zusammenfassung");
            return View();
        }
    }
}