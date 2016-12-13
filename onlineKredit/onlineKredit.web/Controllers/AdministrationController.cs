using onlineKredit.logic;
using onlineKredit.web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace onlineKredit.web.Controllers
{
    public class AdministrationController : Controller
    {
        [HttpGet]
        public ActionResult Anmelden()
        {
            Debug.WriteLine("GET - Administration - Anmelden");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Anmelden(AnmeldenModel model)
        {
            Debug.WriteLine("POST - Administration - Anmelden");

            if (ModelState.IsValid)
            {
                if (model.Benutzername == "admin"
                    && model.Passwort == "123user!")
                {
                    FormsAuthentication.SetAuthCookie("admin", true);
                    return RedirectToAction("KreditAnträge");
                }
                else
                {
                    ModelState.AddModelError("Benutzername", "Ungültiger Benutzername/Passwort!");
                }
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult KreditAnträge()
        {
            Debug.WriteLine("GET - Administration - KreditAnträge");

            /// lade aus der DB die letzten 10 Kreditanträge
            /// 

            List<Kunde> alleKunden = KonsumKreditVerwaltung.KundenLaden();
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

            return View(alleKundenModel);
        }
    }
}