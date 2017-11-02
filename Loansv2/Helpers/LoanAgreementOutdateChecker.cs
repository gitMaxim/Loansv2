using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Helpers
{
    public class LoanAgreementOutdateChecker
    {
        public List<string> Check(int loanAgreementId)
        {
            List<string> res = null;

            using (var db = new LoansContext())
            {
                var loanAgreement = db.LoanAgreements.Find(loanAgreementId);
                if (loanAgreement == null)
                    return null;

                var creditPlans = db.CreditPlans
                    .Where(p => p.LoanAgreementId == loanAgreementId)
                    .OrderBy(p => p.Date);
                var debtPlans = db.DebtPlans
                    .Where(p => p.LoanAgreementId == loanAgreementId)
                    .OrderBy(p => p.Date);
                var payments = db.Payments
                    .Where(p => p.LoanAgreementId == loanAgreementId && p.PaymentType != PaymentType.DebtPercents)
                    .OrderBy(p => p.Date);

                var today = DateTime.Today;
                res = new List<string>();

                if (loanAgreement.CreditPlans != null)
                {
                    var expectedValue = creditPlans
                        .Where(p => p.Date <= today)
                        .Select(p => p.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                    var givenValue = payments
                        .Where(p => p.PaymentType == PaymentType.Credit && p.Date <= today)
                        .Select(p => p.Value)
                        .DefaultIfEmpty(0)
                        .Sum();

                    if (givenValue < expectedValue)
                        res.Add($"На сегодняшнее число ({today:dd.MM.yyyy}) займа предоставлено ({givenValue:N}) меньше, " +
                                $"чем предполагалось ({expectedValue:N}) согласно плану. Для ликвидации задолженности," +
                                $"необходимо предоставить сумму: {expectedValue - givenValue:N}.");
                }

                if (loanAgreement.CreditPlans != null)
                {
                    var expectedValue = debtPlans
                        .Where(p => p.Date <= today)
                        .Select(p => p.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                    var givenValue = payments
                        .Where(p => p.PaymentType == PaymentType.DebtLoan && p.Date <= today)
                        .Select(p => p.Value)
                        .DefaultIfEmpty(0)
                        .Sum();

                    if (givenValue < expectedValue)
                        res.Add($"На сегодняшнее число ({today:dd.MM.yyyy}) займа погашено ({givenValue:N}) меньше, " +
                                $"чем предполагалось ({expectedValue:N}) согласно плану. Для ликвидации задолженности," +
                                $"необходимо погасить сумму: {expectedValue - givenValue:N}.");
                }
            }

            return res;
        }
    }
}