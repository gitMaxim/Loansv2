using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class ProjectController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Index(ProjectParamToSortBy? sortParam, SortOrderType? sortOrder,
            ProjectParamToSearchBy? searchParam, string searchText, int? page)
        {
            var viewModel = new ProjectIndexViewModel(_db.Projects.Where(x => true), sortParam, sortOrder, searchParam, searchText);
            viewModel.ApplyFilters(page ?? 1, GlobalParams.PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(ProjectIndexViewModel viewModel)
        {
            return RedirectToAction("Index", new
            {
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

            var project = _db.Projects.Find(id);
            if (project == null)
                return HttpNotFound("Не удалось найти проект с указанным Id");
            
            var viewModel = new ProjectDetailsViewModel(_db.Projects.Where(x => true), project);
            viewModel.ApplyFilters(_db.LoanAgreements.Where(x => true), _db.Payments.Where(x => true));

            return View(viewModel);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                _db.Projects.Add(project);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = _db.Projects.Find(id);
            if (project == null)
                return HttpNotFound();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var fieldsToBind = new[] { "Name", "RowVersion" };

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var modelToUpdate = _db.Projects.Find(id);
            if (modelToUpdate == null)
            {
                var deletedModel = new Project();
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

                    return RedirectToAction("Details", new { id = id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Project)entry.Entity;

                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                        ModelState.AddModelError(string.Empty, GlobalMessages.EntryDeleted);
                    else
                    {
                        var databaseValues = (Project)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Текущее значение: " + databaseValues.Name);

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

            var project = _db.Projects.Find(id);
            if (project == null)
                return concurrencyError.GetValueOrDefault() ? (ActionResult) RedirectToAction("Index") : HttpNotFound();

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Project project)
        {
            try
            {
                var loanAgreements = _db.LoanAgreements
                    .Where(l => l.CreditorProjectId == project.Id || l.DebtorProjectId == project.Id)
                    .ToList();

                if (loanAgreements.Any())
                {
                    foreach (var l in loanAgreements)
                    {
                        if (l.CreditorProjectId == project.Id)
                            l.CreditorProjectId = null;
                        if (l.DebtorProjectId == project.Id)
                            l.DebtorProjectId = null;
                    }
                }

                _db.Entry(project).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = project.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(project);
            }
        }

        public ActionResult Stats()
        {
            var viewModel = new ProjectStatsViewModel();
            viewModel.ApplyFilters(_db.Payments.Where(x => true));

            return View(viewModel);
        }


        #region Remote validations
        public JsonResult UniqueProjectName(string Name)
        {
            var result = _db.Projects.FirstOrDefault(p => p.Name == Name) == null;
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
