using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class PartyController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Index(PartyParamToSortBy? sortParam, SortOrderType? sortOrder,
            PartyParamToSearchBy? searchParam, string searchText, int? page)
        {
            var viewModel = new PartiesViewModel(_db.Parties.Where(x => true), sortParam, sortOrder, searchParam, searchText);
            viewModel.ApplyFilters(_db.Phones.Where(x => true), _db.Emails.Where(x => true), 
                _db.PhysicalParties.Where(x => true), page ?? 1, GlobalParams.PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(PartiesViewModel viewModel)
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


        public ActionResult Create()
        {
            var viewModel = new Party();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartyType")] Party party)
        {
            switch (party.PartyType)
            {
                case PartyType.Individual:
                    return RedirectToAction("Create", "IndividualParty");
                case PartyType.Juristic:
                    return RedirectToAction("Create", "JuristicParty");
                case PartyType.Physical:
                    return RedirectToAction("Create", "PhysicalParty");
            }

            return View(party);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var party = _db.Parties.Where(p => p.Id == id)
                .Include(p => p.Emails)
                .Include(p => p.Phones)
                .FirstOrDefault();

            if (party == null)
                return HttpNotFound("Не удалось найти контрагента с указанным Id");

            switch (party.PartyType)
            {
                case PartyType.Individual:
                    return RedirectToAction("Details", "IndividualParty", new { id = party.Id });
                case PartyType.Juristic:
                    return RedirectToAction("Details", "JuristicParty", new { id = party.Id });
                case PartyType.Physical:
                    return RedirectToAction("Details", "PhysicalParty", new { id = party.Id });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();

            base.Dispose(disposing);
        }
    }
}
