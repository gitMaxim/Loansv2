using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Loansv2.Controllers;
using Loansv2.Models;
using Loansv2.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Loansv2.Tests.Helpers
{
    [TestClass]
    public class LoanPercentCalculatorTest
    {
        [TestMethod]
        public void CalculatePercents()
        {
            var rates = new List<AnnumRate>();
            var payments = new List<Payment>();

            rates.Add(new AnnumRate(new DateTime(2017, 09, 01), 10));
            rates.Add(new AnnumRate(new DateTime(2017, 10, 01), 15));
            rates.Add(new AnnumRate(new DateTime(2017, 11, 01), 20));

            payments.Add(new Payment(new DateTime(2017, 09, 30), 100000, PaymentType.Credit));
            payments.Add(new Payment(new DateTime(2017, 10, 30), 200000, PaymentType.Credit));
            payments.Add(new Payment(new DateTime(2017, 11, 05), 300000, PaymentType.Credit));
            payments.Add(new Payment(new DateTime(2017, 11, 11), 100000, PaymentType.DebtLoan));

            var calculator = new LoanPercentsCalculator(rates, payments);

            var percents = calculator.CalculatePercentsForOnePeriod(new DateTime(2017, 09, 01), new DateTime(2017, 12, 31));
            var percentsMonthly = calculator.CalculatePercentsMonthly(new DateTime(2017, 09, 01), new DateTime(2017, 12, 31));

            var sum = percentsMonthly.Sum(p => p.Value);

            Assert.IsTrue(Math.Abs(sum - percents) < (decimal) 0.00001);
        }
    }
}
