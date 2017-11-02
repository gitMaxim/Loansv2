using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class PaymentController : Controller
    {
        private LoansContext _db = new LoansContext();

        
        public ActionResult Index()
        {
            var payments = _db.Payments.Include(p => p.LoanAgreement).OrderByDescending(p => p.Date);
            return View(payments.ToList());
        }


        public ActionResult Credit()
        {
            var payments = _db.Payments.Where(p => p.PaymentType == PaymentType.Credit)
                .Include(p => p.LoanAgreement)
                .OrderBy(p => p.Date);

            return View(payments.ToList());
        }

        public ActionResult Debt()
        {
            var payments = _db.Payments.Where(p => p.PaymentType != PaymentType.Credit)
                .Include(p => p.LoanAgreement)
                .OrderBy(p => p.Date);

            return View(payments.ToList());
        }


        public ActionResult Create(int? loanAgreementId, PaymentType? paymentType)
        {
            var viewModel = new Payment();
            if (loanAgreementId != null)
                viewModel.LoanAgreementId = (int) loanAgreementId;
            if (paymentType != null)
                viewModel.PaymentType = (PaymentType) paymentType;
            viewModel.Date = DateTime.Today;
            ViewBag.LoanAgreementId = new SelectList(_db.LoanAgreements, "Id", "Number", viewModel.LoanAgreement);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LoanAgreementId,PaymentType,Date,Value")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _db.Payments.Add(payment);
                _db.SaveChanges();
                return RedirectToAction("Details", "LoanAgreement", new { id = payment.LoanAgreementId });
            }

            ViewBag.LoanAgreementId = new SelectList(_db.LoanAgreements, "Id", "Name", payment.LoanAgreementId);
            return View(payment);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var payment = _db.Payments.Find(id);
            if (payment == null)
                return HttpNotFound();

            ViewBag.LoanAgreementId = new SelectList(_db.LoanAgreements, "Id", "Number", payment.LoanAgreementId);
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Id", "LoanAgreementId", "PaymentType", "Value" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.Payments.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new Payment();
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
                    var clientValues = (Payment)entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (Payment)databaseEntry.ToObject();

                        if (databaseValues.LoanAgreementId != clientValues.LoanAgreementId)
                            ModelState.AddModelError("LoanAgreementId", "Текущее значение: " 
                                + _db.LoanAgreements.Find(databaseValues.LoanAgreementId)?.Number);

                        if (databaseValues.Date != clientValues.Date)
                            ModelState.AddModelError("Date", $"Текущее значение: {databaseValues.Date:d}");

                        if (databaseValues.PaymentType != clientValues.PaymentType)
                            ModelState.AddModelError("PaymentType", $"Текущее значение: {databaseValues.DisplayTypeName()}");

                        if (databaseValues.Value != clientValues.Value)
                            ModelState.AddModelError("Value", $"Текущее значение: {databaseValues.Value:N}");

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

            var payment = _db.Payments.Find(id);
            if (payment == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index", "Party");
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Payment payment)
        {
            int id = payment.LoanAgreementId;
            try
            {
                _db.Entry(payment).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Details", "LoanAgreement", new { id = id });
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = payment.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(payment);
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
