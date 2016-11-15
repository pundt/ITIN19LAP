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
                    /// habe, speichere ich sie für diesen Zweck in die TempData Variable
                    /// (ähnlich wie Session)
                    TempData["idKunde"] = neuerKunde.ID;

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
                ID_Kunde = int.Parse(TempData["idKunde"].ToString())
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
                    TempData["idKunde"] = model.ID_Kunde;
                    return RedirectToAction("PersönlicheDaten");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult PersönlicheDaten()
        {
            Debug.WriteLine("GET - KonsumKredit - PersönlicheDaten");
            PersönlicheDatenModel model = new PersönlicheDatenModel()
            {
                ID_Kunde = int.Parse(TempData["idKunde"].ToString())
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PersönlicheDaten(PersönlicheDatenModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Arbeitgeber()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Arbeitgeber(ArbeitgeberModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult KontoInformationen()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KontoInformationen(KontoInformationenModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Zusammenfassung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Zusammenfassung(ZusammenfassungModel model)
        {
            return View();
        }
    }
}