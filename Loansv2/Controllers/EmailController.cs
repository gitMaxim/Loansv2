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
    public class EmailController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Create(int? partyId)
        {
            if (partyId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var party = _db.Parties.Find(partyId);
            if (party == null)
                return HttpNotFound();

            var email = Mapper.Map<Party, Email>(party);

            return View(email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartyId,Address")] Email email)
        {
            if (ModelState.IsValid)
            {
                _db.Emails.Add(email);
                _db.SaveChanges();
                return RedirectToAction("Details", "Party", new { id = email.PartyId });
            }

            ViewBag.PartyId = new SelectList(_db.Parties, "Id", "Name", email.PartyId);
            return View(email);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var email = _db.Emails.Find(id);
            if (email == null)
                return HttpNotFound();

            return View(email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "PartyId", "Address", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.Emails.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new Email();
                TryUpdateModel(deletedModel, fieldsToBind);
                ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);

                return View(deletedModel);
            }

            if (TryUpdateModel(modelToUpdate, fieldsToBind))
            {
                try
                {
                    _db.Entry(modelToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    _db.SaveChanges();

                    return RedirectToAction("Details", "Party", new { id = modelToUpdate.PartyId });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Email) entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (Email) databaseEntry.ToObject();

                        if (databaseValues.PartyId != clientValues.PartyId)
                            ModelState.AddModelError("PartyId", "Текущее значение: " + _db.Parties.Find(databaseValues.PartyId)?.Name);

                        if (databaseValues.Address != clientValues.Address)
                            ModelState.AddModelError("Address", "Текущее значение: " + databaseValues.Address);

                        ModelState.AddModelError(string.Empty, GlobalMessages.ConcurrencyEdit);

                        modelToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", GlobalMessages.LaterOrAdmin);
                }
            }

            return View(modelToUpdate);
        }


        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var email = _db.Emails.Find(id);
            if (email == null)
                return concurrencyError.GetValueOrDefault()
                    ? (ActionResult) RedirectToAction("Index", "Party")
                    : HttpNotFound();

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Email email)
        {
            int partyId = email.PartyId;
            try
            {
                _db.Entry(email).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Details", "Party", new { id = partyId });
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = email.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(email);
            }
        }


        #region Remote validations
        public JsonResult UniqueEmail(string email, int partyId)
        {
            var result = _db.Emails.FirstOrDefault(p => p.PartyId == partyId && p.Address == email) == null;
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
