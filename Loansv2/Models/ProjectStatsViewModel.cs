using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Loansv2.DAL;
using Loansv2.Models.Chart;
using Newtonsoft.Json;

namespace Loansv2.Models
{
    public class ProjectStatsViewModel
    {
        private JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public List<DataPoint> CreditDataPoints { get; set; }
        public List<DataPoint> DebtDataPoints { get; set; }

        [NotMapped]
        public string JsonCreditPoints { get; set; }
        [NotMapped]
        public string JsonDebtPoints { get; set; }

        [NotMapped]
        public int TotalProjectCount { get; set; }


        public void ApplyFilters(IQueryable<Payment> queryPayments)
        {
            CreditDataPoints = queryPayments
                .Include(p => p.LoanAgreement)
                .Include(p => p.LoanAgreement.CreditorProject)
                .Where(p => p.PaymentType == PaymentType.Credit)
                .GroupBy(p => p.LoanAgreement.CreditorProjectId)
                .Select(s => new DataPoint
                {
                    Label = s.FirstOrDefault().LoanAgreement.CreditorProject.Name,
                    Y = s.Sum(x => x.Value)
                })
                .ToList();

            JsonCreditPoints = JsonConvert.SerializeObject(CreditDataPoints, _jsonSetting);


            DebtDataPoints = queryPayments
                .Include(p => p.LoanAgreement)
                .Include(p => p.LoanAgreement.DebtorProject)
                .Where(p => p.PaymentType == PaymentType.DebtLoan)
                .GroupBy(p => p.LoanAgreement.DebtorProjectId)
                .Select(s => new DataPoint
                {
                    Label = s.FirstOrDefault().LoanAgreement.DebtorProject.Name,
                    Y = s.Sum(x => x.Value)
                })
                .ToList();

            JsonDebtPoints = JsonConvert.SerializeObject(DebtDataPoints, _jsonSetting);
        }
    }
}