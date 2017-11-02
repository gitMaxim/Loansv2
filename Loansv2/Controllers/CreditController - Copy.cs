using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;
using Loansv2;

namespace Loansv2.Controllers
{
    public class CreditController : Controller
    {
        private DataContext db = new DataContext();


        /// <summary>
        /// Displays session creditor shedule
        /// </summary>
        public ActionResult Index(int? id)
        {
            var schedule = new List<CreditItemViewModel>();

            if (id != null)
                db.Credits.Where(l => l.AgreementId == id);
            else
            {
                
                if (Session[SessionNames.VnCredits] != null)
                    schedule = Session[SessionNames.VnCredits] as List<CreditItemViewModel>;
            }

            return View(schedule);
        }

        public ActionResult Change()
        {
            var loanAgreement = Session[SessionNames.VnLoanAgreement] as LoanAgreement;
            if (loanAgreement == null)
                return HttpNotFound("Информация сессии утеряна");

            if (Session[SessionNames.VnCredits] == null)
            {
                Session[SessionNames.VnMaxSumValue] = loanAgreement?.MaxSum;
                Session[SessionNames.VnCredits] = new List<CreditItemViewModel>();
            }

            var viewModel = new CreditViewModel(DateTime.Today, 0,
                Session[SessionNames.VnCredits] as List<CreditItemViewModel>, loanAgreement);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Change([Bind(Include = "Date,Value")] CreditViewModel viewModel)
        {
            viewModel.Schedule = Session[SessionNames.VnCredits] as List<CreditItemViewModel>;
            viewModel.LoanAgreement = Session[SessionNames.VnLoanAgreement] as LoanAgreement;

            if (viewModel.Schedule == null || viewModel.LoanAgreement == null || Session[SessionNames.VnMaxSumValue] == null)
                return HttpNotFound("Информация сессии утеряна");

            if (ModelState.IsValid)
            {
                if (viewModel.Schedule != null)
                {
                    viewModel.AddScheduledDate();
                    viewModel.SortSchedule();
                    Session[SessionNames.VnMaxSumValue] = viewModel.ValuesAvailable;
                }
                return RedirectToAction("Change");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Saves creditor session schedule to database and redirects to 
        /// another action
        /// </summary>
        public ActionResult ChangeComplete()
        {
            var loanAgreement = Session[SessionNames.VnLoanAgreement] as LoanAgreement;
            if (loanAgreement == null)
                return HttpNotFound("Информация сессии утеряна");

            var schedule = Session[SessionNames.VnCredits] as List<CreditItemViewModel>;

            if (schedule != null)
            {
                foreach (var x in schedule)
                    db.Credits.Add(new Credit(x, loanAgreement.Id));

                db.SaveChanges();
            }

            return RedirectToAction("Change", "Debt");
        }

        /// <summary>
        /// Deletes creditor scheduled date from session
        /// </summary>
        public ActionResult Remove(DateTime date)
        {
            var schedule = Session[SessionNames.VnCredits] as List<CreditItemViewModel>;
            if (schedule != null)
            {
                var item = schedule.Single(x => x.Date == date);
                if (Session[SessionNames.VnMaxSumValue] == null)
                    return HttpNotFound("Информация сессии утеряна");

                Session[SessionNames.VnMaxSumValue] = (decimal)Session[SessionNames.VnMaxSumValue] + item.Value;
                schedule.Remove(item);
            }
            
            return RedirectToAction("Change");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
