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
    public class IndividualPartyController : Controller
    {
        private LoansContext _db = new LoansContext();

        
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var individualParty = _db.IndividualParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .Include(p => p.Party.Phones)
                .Include(p => p.Party.Emails)
                .FirstOrDefault();

            if (individualParty == null)
                return HttpNotFound();

            return View(individualParty);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IndividualPartyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var party = Mapper.Map<IndividualPartyViewModel, Party>(viewModel);
                _db.Parties.Add(party);
                _db.SaveChanges();
                
                viewModel.Id = party.Id;
                var individualParty = Mapper.Map<IndividualPartyViewModel, IndividualParty>(viewModel);
                _db.IndividualParties.Add(individualParty);
                _db.SaveChanges();

                return RedirectToAction("Details", "IndividualParty", new { id = individualParty.Id });
            }

            return View(viewModel);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var individualParty = _db.IndividualParties.Where(p => p.Id == id).Include(p => p.Party).FirstOrDefault();
            if (individualParty == null)
                return HttpNotFound();

            var viewModel = Mapper.Map<IndividualParty, IndividualPartyViewModel>(individualParty);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "Name", "VatId", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.IndividualParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .FirstOrDefault();

            var modelToUpdateViewModel = new IndividualPartyViewModel();

            if (modelToUpdate == null)
            {
                var deletedModelViewModel = new IndividualPartyViewModel();
                TryUpdateModel(deletedModelViewModel, fieldsToBind);
                ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);

                return View(deletedModelViewModel);
            }

            if (TryUpdateModel(modelToUpdateViewModel, fieldsToBind))
            {
                try
                {
                    TryUpdateModel(modelToUpdate, fieldsToBind);
                    _db.Entry(modelToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    _db.SaveChanges();
                    modelToUpdate.Party.VatId = modelToUpdateViewModel.VatId;
                    modelToUpdate.Party.Name = modelToUpdateViewModel.Name;
                    _db.SaveChanges();

                    return RedirectToAction("Details", new { id = id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (IndividualParty)databaseEntry.ToObject();

                        if (modelToUpdate.Party?.Name != modelToUpdateViewModel.Name)
                            ModelState.AddModelError("Name", "Текущее значение: " + modelToUpdate.Party?.Name);

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

            var individualParty = _db.IndividualParties.Where(p => p.Id == id)
                .Include(p => p.Party)
                .FirstOrDefault();

            if (individualParty == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index", "Party");
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            ViewBag.LoanAgreements = _db.LoanAgreements.Where(l => l.CreditorId == id || l.DebtorId == id).ToList();

            return View(individualParty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IndividualParty individualParty)
        {
            try
            {
                var phones = _db.Phones.Where(p => p.PartyId == individualParty.Id).ToList();
                var emails = _db.Emails.Where(p => p.PartyId == individualParty.Id).ToList();
                var party = _db.Parties.Find(individualParty.Id);

                if (phones.Any() || emails.Any())
                {
                    _db.Phones.RemoveRange(phones);
                    _db.Emails.RemoveRange(emails);
                }

                _db.Entry(individualParty).State = EntityState.Deleted;
                _db.Parties.Remove(party);
                _db.SaveChanges();

                return RedirectToAction("Index", "Party");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = individualParty.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(individualParty);
            }
        }


        #region Remote validations
        public JsonResult UniqueName(string name)
        {
            var result = _db.Parties.FirstOrDefault(p => p.Name == name) == null;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();

            base.Dispose(disposing);
        }
    }
}
