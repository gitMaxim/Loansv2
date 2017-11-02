using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Loansv2.DAL;
using Loansv2.Helpers;
using Newtonsoft.Json;

namespace Loansv2.Models
{
    public class CreditAgreementViewModel
    {
        public LoanAgreement LoanAgreement { get; set; }

        [Display(Name = "Всего выдано займа")]
        public decimal CreditSum { get; set; }


        #region Constructors
        public CreditAgreementViewModel()
        {
        }

        public CreditAgreementViewModel(LoanAgreement loanAgreement)
        {
            LoanAgreement = loanAgreement;
        }

        public CreditAgreementViewModel(CreditAgreementViewModel other)
        {
            LoanAgreement = other.LoanAgreement;
            CreditSum = other.CreditSum;
        }
        #endregion
    }

    public class DebtAgreementViewModel
    {
        public LoanAgreement LoanAgreement { get; set; }

        [Display(Name = "Всего погашено займа")]
        public decimal DebtSum { get; set; }

        [Display(Name = "Всего погашено %")]
        public decimal PercentSum { get; set; }


        #region Constructors
        public DebtAgreementViewModel()
        {
        }

        public DebtAgreementViewModel(LoanAgreement loanAgreement)
        {
            LoanAgreement = loanAgreement;
        }

        public DebtAgreementViewModel(DebtAgreementViewModel other)
        {
            LoanAgreement = other.LoanAgreement;
            DebtSum = other.DebtSum;
        }
        #endregion
    }


    public class DetailedLoanAgreementViewModel
    {
        public LoanAgreement LoanAgreement { get; set; }
        public List<Payment> Payments { get; set; }
        public List<AnnumRate> Rates { get; set; }

        public List<PercentViewModel> PercentStats { get; set; }
        public List<LoanViewModel> LoanStats { get; set; }

        public List<string> Messages { get; set; }


        #region Constructors
        public DetailedLoanAgreementViewModel() : this(new LoanAgreement())
        { 
        }

        public DetailedLoanAgreementViewModel(LoanAgreement loanAgreement)
        {
            LoanAgreement = loanAgreement;
            Payments = new List<Payment>();
            Rates = new List<AnnumRate>();
            LoanStats = new List<LoanViewModel>();
            PercentStats = new List<PercentViewModel>();
        }
        #endregion


        public void ApplyFilters(IQueryable<Payment> queryPayments, IQueryable<AnnumRate> annumRates)
        {
            Payments = queryPayments
                .Where(p => p.LoanAgreementId == LoanAgreement.Id)
                .OrderBy(p => p.Date)
                .ToList();

            var outdate = new LoanAgreementOutdateChecker();
            Messages = outdate.Check(LoanAgreement.Id);

            Rates = annumRates
                .Where(r => r.LoanAgreementId == LoanAgreement.Id)
                .OrderBy(r => r.Date)
                .ToList();

            if (Payments.Any())
            { 
                CalculateLoansStats();
                if (Rates.Any())
                    CalculatePercentsStats();
            }
        }


        private void CalculatePercentsStats()
        {
            var calculator = new LoanPercentsCalculator(Rates, Payments);
            PercentStats = calculator.CalculatePercentsMonthly(
                new DateTime(LoanAgreement.SignDate.Year, LoanAgreement.SignDate.Month, 1),
                new DateTime(LoanAgreement.DeadlineDate.Year, LoanAgreement.DeadlineDate.Month,
                    DateTime.DaysInMonth(LoanAgreement.DeadlineDate.Year, LoanAgreement.DeadlineDate.Month)),
                true);
        }

        private void CalculateLoansStats()
        {
            LoanStats?.Clear();
            LoanStats = new List<LoanViewModel>();

            decimal creditPayed = 0, debtPayed = 0;
            foreach (var p in Payments)
            {
                switch (p.PaymentType)
                {
                    case PaymentType.Credit:
                        creditPayed += p.Value;
                        break;
                    case PaymentType.DebtLoan:
                        debtPayed += p.Value;
                        break;
                    default:
                        continue;
                }
                LoanStats.Add(new LoanViewModel(p.Date, p.Value, creditPayed - debtPayed, p.PaymentType));
            }

        }
    }
}