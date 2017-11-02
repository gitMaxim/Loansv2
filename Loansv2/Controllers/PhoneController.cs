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
    public class PhoneController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Create(int? partyId)
        {
            if (partyId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var party = _db.Parties.Find(partyId);
            if (party == null)
                return HttpNotFound();

            var phone = Mapper.Map<Party, Phone>(party);

            return View(phone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartyId,Number")] Phone phone)
        {
            if (ModelState.IsValid)
            {
                _db.Phones.Add(phone);
                _db.SaveChanges();
                return RedirectToAction("Details", "Party", new { id = phone.PartyId });
            }

            return View(phone);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var phone = _db.Phones.Find(id);
            if (phone == null)
                return HttpNotFound();
            
            return View(phone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "PartyId", "Number", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.Phones.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new Phone();
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
                    var clientValues = (Phone)entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (Phone)databaseEntry.ToObject();

                        if (databaseValues.PartyId != clientValues.PartyId)
                            ModelState.AddModelError("PartyId", "Текущее значение: " + _db.Parties.Find(databaseValues.PartyId)?.Name);

                        if (databaseValues.Number != clientValues.Number)
                            ModelState.AddModelError("Number", "Текущее значение: " + databaseValues.Number);

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

            var phone = _db.Phones.Find(id);
            if (phone == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index", "Party");
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(phone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Phone phone)
        {
            int partyId = phone.PartyId;
            try
            {
                _db.Entry(phone).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Details", "Party", new { id = partyId });
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = phone.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(phone);
            }
        }

        #region Remote validations
        public JsonResult UniquePhone(string phone, int partyId)
        {
            var result = _db.Phones.FirstOrDefault(p => p.PartyId == partyId && p.Number == phone) == null;
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
