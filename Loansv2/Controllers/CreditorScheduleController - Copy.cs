using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class CreditorScheduleController : Controller
    {
        private DataContext db = new DataContext();


        public ActionResult Index()
        {
            var schedule = new List<CreditorScheduleViewModel>();
            if (Session["creditorSchedule"] != null)
                schedule = Session["creditorSchedule"] as List<CreditorScheduleViewModel>;

            return View(schedule);
        }

        public ActionResult Change()
        {
            var loanAgreement = Session["loanAgreement"] as LoanAgreementModel;
            if (Session["creditorSchedule"] == null)
            {
                Session["maxSumValue"] = loanAgreement?.MaxSum;
                Session["creditorSchedule"] = new List<CreditorScheduleViewModel>();
            }

            var viewModel = new FullCreditorScheduleViewModel(DateTime.Today, 0,
                Session["creditorSchedule"] as List<CreditorScheduleViewModel>, loanAgreement);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Change([Bind(Include = "Date,Value")] FullCreditorScheduleViewModel viewModel)
        {
            viewModel.Schedule = Session["creditorSchedule"] as List<CreditorScheduleViewModel>;
            viewModel.LoanAgreement = Session["loanAgreement"] as LoanAgreementModel;

            if (ModelState.IsValid)
            {
                if (viewModel.Schedule != null)
                {
                    viewModel.AddScheduledDate();
                    viewModel.SortSchedule();
                    Session["maxSumValue"] = viewModel.ValuesAvailable;
                }
                return RedirectToAction("Change");
            }

            return View(viewModel);
        }

        public ActionResult ChangeComplete()
        {
            var loanAgreement = Session["loanAgreement"] as LoanAgreementModel;
            if (loanAgreement == null)
                return HttpNotFound("Информация сессии по договору займа не найдена");

            var schedule = Session["creditorSchedule"] as List<CreditorScheduleViewModel>;

            if (schedule != null)
            {
                foreach (var x in schedule)
                    db.CreditorSchedule.Add(new CreditorSchedule(x, loanAgreement.Id));

                db.SaveChanges();
                Session["creditorSchedule"] = null;
            }

            return RedirectToAction("Change", "DebtorSchedule");
        }

        public ActionResult Delete(DateTime date)
        {
            var schedule = Session["creditorSchedule"] as List<CreditorScheduleViewModel>;
            if (schedule != null)
            {
                var item = schedule.Single(x => x.Date == date);
                Session["maxSumValue"] = (decimal)Session["maxSumValue"] + item.Value;
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
