using onlineKredit.web.Models;
using System;
using System.Collections.Generic;
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
            return View();
        }

        [HttpPost]
        public ActionResult KreditRahmen(KreditRahmenModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult FinanzielleSituation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FinanzielleSituation(FinanzielleSituationModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult PersönlicheDaten()
        {
            return View();
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