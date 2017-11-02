using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class PhysicalPartyController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var physicalParty = _db.PhysicalParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .Include(p => p.Party.Phones)
                .Include(p => p.Party.Emails)
                .FirstOrDefault();

            if (physicalParty == null)
                return HttpNotFound();

            return View(physicalParty);
        }

        
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhysicalPartyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var party = Mapper.Map<PhysicalPartyViewModel, Party>(viewModel);
                _db.Parties.Add(party);
                _db.SaveChanges();

                viewModel.Id = party.Id;
                var physicalParty = Mapper.Map<PhysicalPartyViewModel, PhysicalParty>(viewModel);
                _db.PhysicalParties.Add(physicalParty);
                _db.SaveChanges();

                return RedirectToAction("Details", "PhysicalParty", new { id = viewModel.Id });
            }

            return View(viewModel);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var physicalParty = _db.PhysicalParties.Where(p => p.Id == id).Include(p => p.Party).FirstOrDefault();
            if (physicalParty == null)
                return HttpNotFound();

            var viewModel = Mapper.Map<PhysicalParty, PhysicalPartyViewModel>(physicalParty);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "FirstName", "MiddleName", "LastName", "RowVersion", "VatId" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.PhysicalParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .FirstOrDefault();

            if (modelToUpdate == null)
            {
                var deletedModelViewModel = new PhysicalPartyViewModel();
                TryUpdateModel(deletedModelViewModel, fieldsToBind);
                ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);

                return View(deletedModelViewModel);
            }

            var modelToUpdateViewModel = new PhysicalPartyViewModel();

            if (TryUpdateModel(modelToUpdateViewModel, fieldsToBind))
            {
                try
                {
                    TryUpdateModel(modelToUpdate, fieldsToBind);
                    _db.Entry(modelToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    modelToUpdate.Party.VatId = modelToUpdateViewModel.VatId;
                    modelToUpdate.Party.Name = modelToUpdateViewModel.ShortName;
                    _db.SaveChanges();

                    return RedirectToAction("Details", new { id = id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (PhysicalParty) entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (PhysicalParty)databaseEntry.ToObject();

                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Текущее значение: " + databaseValues.LastName);

                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Текущее значение: " + databaseValues.FirstName);

                        if (databaseValues.MiddleName != clientValues.MiddleName)
                            ModelState.AddModelError("MiddleName", "Текущее значение: " + databaseValues.MiddleName);

                        if (modelToUpdate.Party?.VatId != modelToUpdateViewModel.VatId)
                            ModelState.AddModelError("VatId", "Текущее значение: " + modelToUpdate.Party?.VatId);

                        ModelState.AddModelError(string.Empty, GlobalMessages.ConcurrencyEdit);

                        modelToUpdateViewModel.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", GlobalMessages.LaterOrAdmin);
                }
            }

            return View(modelToUpdateViewModel);
        }


        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var physicalParty = _db.PhysicalParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .FirstOrDefault();

            if (physicalParty == null)
                return concurrencyError.GetValueOrDefault() ? 
                    (ActionResult) RedirectToAction("Index", "Party")
                    : HttpNotFound();

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            ViewBag.LoanAgreements = _db.LoanAgreements.Where(l => l.CreditorId == id || l.DebtorId == id).ToList();

            return View(physicalParty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PhysicalParty physicalParty)
        {
            try
            {
                var phones = _db.Phones.Where(p => p.PartyId == physicalParty.Id).ToList();
                var emails = _db.Emails.Where(p => p.PartyId == physicalParty.Id).ToList();
                var party = _db.Parties.Find(physicalParty.Id);

                if (phones.Any() || emails.Any())
                {
                    _db.Phones.RemoveRange(phones);
                    _db.Emails.RemoveRange(emails);
                }

                _db.Entry(physicalParty).State = EntityState.Deleted;
                _db.Parties.Remove(party);
                _db.SaveChanges();

                return RedirectToAction("Index", "Party");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = physicalParty.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(physicalParty);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();

            base.Dispose(disposing);
        }
    }
}
