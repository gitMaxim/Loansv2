using System;
using System.Collections.Generic;
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
    public class CreditPlanController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Index(int? id)
        {
            var plan = new List<CreditPlan>();

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            plan = _db.CreditPlans.Where(l => l.LoanAgreementId == id).ToList();

            ViewBag.loanAgreementId = id;
            return PartialView(plan);
        }


        public ActionResult Create(int? loanAgreementId)
        {
            if (loanAgreementId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loanAgreement = _db.LoanAgreements.Find(loanAgreementId);
            if (loanAgreement == null)
                return HttpNotFound();

            var credit = Mapper.Map<LoanAgreement, CreditPlan>(loanAgreement);

            return View(credit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreditPlan credit)
        {
            if (ModelState.IsValid)
            {
                _db.CreditPlans.Add(credit);
                _db.SaveChanges();
                return RedirectToAction("Details", "LoanAgreement", new { id = credit.LoanAgreementId });
            }

            return View(credit);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var credit = _db.CreditPlans.Find(id);
            if (credit == null)
                return HttpNotFound();

            return View(credit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "LoanAgreementId", "Date", "Value", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.CreditPlans.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new CreditPlan();
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

                    return RedirectToAction("Details", "LoanAgreement", new { id = modelToUpdate.LoanAgreementId });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (CreditPlan) entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (CreditPlan) databaseEntry.ToObject();

                        if (databaseValues.LoanAgreementId != clientValues.LoanAgreementId)
                            ModelState.AddModelError("LoanAgreementId", $"Текущее значение: {databaseValues.LoanAgreementId}");

                        if (databaseValues.Value != clientValues.Value)
                            ModelState.AddModelError("Value", $"Текущее значение: {databaseValues.Value:N}");

                        if (databaseValues.Date != clientValues.Date)
                            ModelState.AddModelError("Date", $"Текущее значение: {databaseValues.Date:dd.MM.yyyy}");

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

            var credit = _db.CreditPlans.Find(id);
            if (credit == null)
                return concurrencyError.GetValueOrDefault()
                    ? (ActionResult)RedirectToAction("Index", "LoanAgreement")
                    : HttpNotFound();

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(credit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CreditPlan credit)
        {
            int loanAgreementId = credit.LoanAgreementId;
            try
            {
                _db.Entry(credit).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Details", "LoanAgreement", new { id = loanAgreementId });
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = credit.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(credit);
            }
        }


        #region Remote validations
        public JsonResult ValidateCreditDate(DateTime date, int loanAgreementId)
        {
            var loanAgreement = _db.LoanAgreements.Find(loanAgreementId);
            if (loanAgreement == null)
                return Json("Не удаётся найти соответствующий договор", JsonRequestBehavior.AllowGet);

            if (date < loanAgreement.SignDate || date > loanAgreement.DeadlineDate)
                return Json("Дата должна быть в рамках действия договора: " +
                            $"с {loanAgreement.SignDate:dd.MM.yyyy} по {loanAgreement.DeadlineDate:dd.MM.yyyy}",
                    JsonRequestBehavior.AllowGet);

            if (_db.CreditPlans.FirstOrDefault(r => r.LoanAgreementId == loanAgreementId && r.Date == date) != null)
                return Json("На дату уже запланирована выдача займа", JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCreditSum(decimal value, int loanAgreementId)
        {
            var loanAgreement = _db.LoanAgreements.Find(loanAgreementId);
            if (loanAgreement == null)
                return Json("Не удаётся найти соответствующий договор", JsonRequestBehavior.AllowGet);

            var planCreditSum = _db.CreditPlans.Where(p => p.LoanAgreementId == loanAgreementId).Sum(p => p.Value);
            if (planCreditSum + value > loanAgreement.Sum)
                return Json($"Общая планируемая сумма выдачи займа не может превосходить сумму в договоре: {loanAgreement.Sum:N}", 
                    JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
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
