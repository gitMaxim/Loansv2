using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using Loansv2.DAL;
using Loansv2.Models.Chart;
using Newtonsoft.Json;
using Loansv2.Helpers;

namespace Loansv2.Models
{
    public class ProjectDetailsViewModel
    {
        private JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        private IQueryable<Project> _query;


        public int Id { get; set; }

        [Display(Name = "Проект")]
        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [DisplayName("Количество договоров в проекте")]
        public int TotalLoanAgreementsCount { get; set; }

        [DisplayName("Всего выдано займа")]
        public decimal CreditPaymentsSum { get; set; }
        public List<DataPoint> CreditPaymentPoints { get; set; }
        public string JsonCreditPaymentPoints { get; set; }

        public List<ProjectCreditorsViewModel> CreditAgreements { get; set; }


        [DisplayName("Всего погашено займа")]
        public decimal DebtLoanPaymentsSum { get; set; }
        [DisplayName("Всего погашено процентов")]
        public decimal DebtPercentPaymentsSum { get; set; }
        public List<DataPoint> DebtPaymentPoints { get; set; }
        public string JsonDebtPaymentPoints { get; set; }

        public List<ProjectDebtorsViewModel> DebtAgreements { get; set; }


        #region Constructors
        public ProjectDetailsViewModel()
        {
        }

        public ProjectDetailsViewModel(IQueryable<Project> query, Project project)
        {
            _query = query;
            Id = project.Id;
            Name = project.Name;
        }
        #endregion


        public void ApplyFilters(IQueryable<LoanAgreement> queryLoanAgreements, IQueryable<Payment> queryPayments)
        {
            var creditAgreements = queryLoanAgreements.Where(l => l.CreditorProjectId == Id)
                .Include(l => l.Creditor)
                .Include(l => l.CreditorProject)
                .Include(l => l.Debtor)
                .Include(l => l.DebtorProject)
                .OrderBy(l => l.SignDate)
                .ToList();
            var creditPayments = queryPayments.Where(p => p.PaymentType != PaymentType.DebtPercents)
                .Include(p => p.LoanAgreement)
                .Where(p => p.LoanAgreement.CreditorProjectId == Id)
                .OrderBy(p => p.Date)
                .ToList();

            CalculateCreditBalance(creditPayments, creditAgreements);
            CreditAgreements = Mapper.Map<List<LoanAgreement>, List<ProjectCreditorsViewModel>>(creditAgreements);
            CreditPaymentsSum = CreditAgreements.Sum(l => l.LoanedSum);

            var debtPayments = queryPayments.Where(p => p.PaymentType != PaymentType.DebtPercents)
                .Include(p => p.LoanAgreement)
                .Where(p => p.LoanAgreement.DebtorProjectId == Id)
                .OrderBy(p => p.Date)
                .ToList();
            var debtAgreements = queryLoanAgreements.Where(l => l.DebtorProjectId == Id)
                .Include(l => l.Creditor)
                .Include(l => l.CreditorProject)
                .Include(l => l.Debtor)
                .Include(l => l.DebtorProject)
                .OrderBy(l => l.SignDate)
                .ToList();

            CaluclateDebtBalance(debtPayments, creditAgreements);
            DebtAgreements = Mapper.Map<List<LoanAgreement>, List<ProjectDebtorsViewModel>>(debtAgreements);
            DebtLoanPaymentsSum = DebtAgreements.Sum(l => l.RepayedSum);

            TotalLoanAgreementsCount = creditAgreements.Count + debtAgreements.Count;
        }


        private void CalculateCreditBalance(List<Payment> creditPayments, List<LoanAgreement> creditAgreements)
        {
            CreditPaymentPoints?.Clear();
            CreditPaymentPoints = new List<DataPoint>(creditPayments.Count);

            if (creditPayments.Count > 0 && creditAgreements.Count > 0)
            {
                var agreement = creditAgreements.ElementAt(0);
                var date = new DateTime(agreement.SignDate.Year, agreement.SignDate.Month, 1);
                decimal sum = 0;

                foreach (var p in creditPayments)
                {
                    for (; date.Year < p.Date.Year || (date.Year == p.Date.Year && date.Month < p.Date.Month); date = date.AddMonths(1))
                        CreditPaymentPoints.Add(new DataPoint(DateLabel.GetMonthYear(date), sum));

                    switch (p.PaymentType)
                    {
                        case PaymentType.Credit:
                            sum += p.Value;
                            break;
                        case PaymentType.DebtLoan:
                            sum -= p.Value;
                            break;
                    }
                }

                CreditPaymentPoints.Add(new DataPoint(DateLabel.GetMonthYear(date), sum));
            }

            JsonCreditPaymentPoints = JsonConvert.SerializeObject(CreditPaymentPoints, _jsonSetting);
        }

        private void CaluclateDebtBalance(List<Payment> debtPayments, List<LoanAgreement> creditAgreements)
        {
            DebtPaymentPoints?.Clear();
            DebtPaymentPoints = new List<DataPoint>(debtPayments.Count);

            if (debtPayments.Count > 0 && creditAgreements.Count > 0)
            {
                var agreement = creditAgreements.ElementAt(0);
                var date = new DateTime(agreement.SignDate.Year, agreement.SignDate.Month, 1);
                decimal sum = 0;
                foreach (var p in debtPayments)
                {
                    for (; date.Year < p.Date.Year || (date.Year == p.Date.Year && date.Month < p.Date.Month); date = date.AddMonths(1))
                        DebtPaymentPoints.Add(new DataPoint(DateLabel.GetMonthYear(date), sum));

                    switch (p.PaymentType)
                    {
                        case PaymentType.Credit:
                            sum += p.Value;
                            break;
                        case PaymentType.DebtLoan:
                            sum -= p.Value;
                            break;
                    }
                }

                DebtPaymentPoints.Add(new DataPoint(DateLabel.GetMonthYear(date), sum));
            }

            JsonDebtPaymentPoints = JsonConvert.SerializeObject(DebtPaymentPoints, _jsonSetting);
        }
    }
}