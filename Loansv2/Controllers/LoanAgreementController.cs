using System;
using System.Data.Entity;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;
using System.Data;
using System.Linq;
using System.Net;
using System.Data.Entity.Infrastructure;
using Loansv2.Helpers;

namespace Loansv2.Controllers
{
    public class LoanAgreementController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Index(LoanAgreementParamToSortBy? sortParam, SortOrderType? sortOrder, 
            LoanAgreementParamToSearchBy? searchParam, string searchText, int? page)
        {
            var viewModel = new LoanAgreementsViewModel(_db.LoanAgreements.Where(x => true), sortParam, sortOrder, searchParam, searchText);
            viewModel.ApplyFilters(page ?? 1, GlobalParams.PageSize);

            return View(viewModel); 
        }

        [HttpPost]
        public ActionResult Index(LoanAgreementsViewModel viewModel)
        {
            return RedirectToAction("Index", new {
                sortParam = viewModel.SortParam,
                sortOrder = viewModel.SortOrder,
                searchParam = viewModel.SearchParam,
                searchText = viewModel.SearchText,
                currentFiler = viewModel.SearchText,
                page = 1
            });
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loanAgreement = _db.LoanAgreements.Where(l => l.Id == id)
                .Include(l => l.CreditPlans)
                .Include(l => l.DebtPlans)
                .Include(l => l.Files)
                .FirstOrDefault();

            if (loanAgreement == null)
                return HttpNotFound("Не удалось найти договор с указанным Id");

            var viewModel = new DetailedLoanAgreementViewModel(loanAgreement);
            viewModel.ApplyFilters(_db.Payments.Where(x => true), _db.AnnumRates.Where(x => true));

            return View(viewModel);
        }


        public ActionResult Create()
        {
            var viewModel = new LoanAgreement();
            viewModel.SignDate = DateTime.Today;
            viewModel.DeadlineDate = DateTime.Today.AddMonths(1);

            ViewBag.parties = new SelectList(_db.Parties, "Id", "Name");
            ViewBag.projects = new SelectList(_db.Projects, "Id", "Name");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Number,CreditorId,CreditorProjectId,DebtorId,DebtorProjectId,SignDate,DeadlineDate,Sum")] LoanAgreement viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loanAgreement = new LoanAgreement(viewModel);                    
                    _db.LoanAgreements.Add(loanAgreement);
                    _db.SaveChanges();

                    return RedirectToAction("Details", new { id = loanAgreement.Id });
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Невозможно сохранить изменения. Попытайтесь повторить позднее или свяжитесь с администратором.");
            }

            ViewBag.parties = new SelectList(_db.Parties, "Id", "Name");
            ViewBag.projects = new SelectList(_db.Projects, "Id", "Name");

            return View(viewModel);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loanAgreement = _db.LoanAgreements.Find(id);
            if (loanAgreement == null)
                return HttpNotFound();

            ViewBag.creditorParties = new SelectList(_db.Parties, "Id", "Name", loanAgreement.CreditorId);
            ViewBag.debtorParties = new SelectList(_db.Parties, "Id", "Name", loanAgreement.DebtorId);
            ViewBag.creditorProjects = new SelectList(_db.Projects, "Id", "Name", loanAgreement.CreditorProjectId);
            ViewBag.debtorProjects = new SelectList(_db.Projects, "Id", "Name", loanAgreement.DebtorProjectId);

            return View(loanAgreement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Number", "CreditorId", "CreditorProjectId", "DebtorId", "DebtorProjectId",
                "SignDate", "DeadlineDate", "Sum", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.LoanAgreements.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new LoanAgreement();
                TryUpdateModel(deletedModel, fieldsToBind);
                ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);

                ViewBag.creditorParties = new SelectList(_db.Parties, "Id", "Name", deletedModel.CreditorId);
                ViewBag.debtorParties = new SelectList(_db.Parties, "Id", "Name", deletedModel.DebtorId);
                ViewBag.creditorProjects = new SelectList(_db.Projects, "Id", "Name", deletedModel.CreditorProjectId);
                ViewBag.debtorProjects = new SelectList(_db.Projects, "Id", "Name", deletedModel.DebtorProjectId);

                return View(deletedModel);
            }

            if (TryUpdateModel(modelToUpdate, fieldsToBind))
            {
                try
                {
                    _db.Entry(modelToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    _db.SaveChanges();

                    return RedirectToAction("Details", "LoanAgreement", new { id = id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (LoanAgreement)entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (LoanAgreement)databaseEntry.ToObject();

                        if (databaseValues.Number != clientValues.Number)
                            ModelState.AddModelError("Number", "Текущее значение: " + databaseValues.Number);

                        if (databaseValues.CreditorId != clientValues.CreditorId)
                            ModelState.AddModelError("CreditorId", "Текущее значение: "
                                + $"{_db.Parties.Find(databaseValues.CreditorId)?.Name}");
                        if (databaseValues.CreditorProjectId != clientValues.CreditorProjectId)
                            ModelState.AddModelError("CreditorProjectId", "Текущее значение: " 
                                + $"{_db.Projects.Find(databaseValues.CreditorProjectId)?.Name}");

                        if (databaseValues.DebtorId != clientValues.DebtorId)
                            ModelState.AddModelError("DebtorId", "Текущее значение: "
                                + $"{_db.Parties.Find(databaseValues.DebtorId)?.Name}");
                        if (databaseValues.DebtorProjectId != clientValues.DebtorProjectId)
                            ModelState.AddModelError("DebtorProjectId", "Текущее значение: " 
                                + $"{_db.Projects.Find(databaseValues.DebtorProjectId)?.Name}");

                        if (databaseValues.SignDate != clientValues.SignDate)
                            ModelState.AddModelError("SignDate", $"Текущее значение: {databaseValues.SignDate:d}");
                        if (databaseValues.DeadlineDate != clientValues.DeadlineDate)
                            ModelState.AddModelError("DeadlineDate", $"Текущее значение: {databaseValues.DeadlineDate:d}");
                        if (databaseValues.Sum != clientValues.Sum)
                            ModelState.AddModelError("Sum", "Текущее значение: " + $"{databaseValues.Sum:N}");

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

            ViewBag.creditorParties = new SelectList(_db.Parties, "Id", "Name", modelToUpdate.CreditorId);
            ViewBag.debtorParties = new SelectList(_db.Parties, "Id", "Name", modelToUpdate.DebtorId);
            ViewBag.creditorProjects = new SelectList(_db.Projects, "Id", "Name", modelToUpdate.CreditorProjectId);
            ViewBag.debtorProjects = new SelectList(_db.Projects, "Id", "Name", modelToUpdate.DebtorProjectId);

            return View(modelToUpdate);
        }


        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loanAgreement = _db.LoanAgreements.Find(id);
            if (loanAgreement == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index", "Party");
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(loanAgreement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(LoanAgreement loanAgreement)
        {
            try
            {
                var creditPlans = _db.CreditPlans.Where(c => c.LoanAgreementId == loanAgreement.Id).ToList();
                var debtPlans = _db.DebtPlans.Where(d => d.LoanAgreementId == loanAgreement.Id).ToList();
                var annumRates = _db.AnnumRates.Where(r => r.LoanAgreementId == loanAgreement.Id).ToList();
                var payments = _db.Payments.Where(p => p.LoanAgreementId == loanAgreement.Id).ToList();
                var files = _db.Files.Where(f => f.LoanAgreementId == loanAgreement.Id).ToList();

                if (creditPlans.Any() || debtPlans.Any() || annumRates.Any() || payments.Any() || files.Any())
                {
                    _db.CreditPlans.RemoveRange(creditPlans);
                    _db.DebtPlans.RemoveRange(debtPlans);
                    _db.AnnumRates.RemoveRange(annumRates);
                    _db.Payments.RemoveRange(payments);
                    _db.Files.RemoveRange(files);
                }

                _db.Entry(loanAgreement).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = loanAgreement.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(loanAgreement);
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
