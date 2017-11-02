using System;
using System.Collections.Generic;
using System.Linq;
using Loansv2.Models;

namespace Loansv2.Helpers
{
    public class LoanPercentsCalculator
    {
        private AnnumRate _rate;
        private int _rateId = -1;
        private int _paymentId = -1;
        private decimal _debt;
        private decimal _percentsTotalLeft;
        private decimal _percents;
        private decimal _percentsPaid;
        private IDateDecimal[] _events = new IDateDecimal[4];

        public readonly List<AnnumRate> Rates;
        public readonly List<Payment> Payments;

        public List<PercentViewModel> History { get; set; }


        #region Constructors
        /// <summary>
        /// Lists must be sorted in date ascending order before passing to constructor
        /// </summary>
        /// <param name="rates"></param>
        /// <param name="payments"></param>
        public LoanPercentsCalculator(List<AnnumRate> rates, List<Payment> payments)
        {
            Rates = rates;
            Payments = payments;
        }
        #endregion


        public decimal CalculatePercentsForOneDay(DateTime date, decimal debt, decimal rate)
        {
            return debt * rate / (DateHelper.GetDaysInYear(date) * 100);
        }

        public decimal CalculatePercentsForOnePeriod(DateTime begin, DateTime end, decimal percentsTotalLeft = 0, bool percentPayments = false)
        {
            _percents = 0;
            _percentsTotalLeft = percentsTotalLeft;
            if (begin > end)
                return _percents;

            _rateId = -1;
            _paymentId = -1;
            _rate = GetRateBefore(begin);
            _debt = CalculateDebtBefore(begin);

            for (int count; begin <= end; )
            {
                var rate = _rate?.Value ?? 0;
                var events = GetClosestEvents(out count, _events);

                var eventsDate = end.AddDays(1);
                if (events != null && events.ElementAt(0).Date <= end)
                    eventsDate = events.ElementAt(0).Date;

                var days = (eventsDate - begin).Days;
                var p = CalculatePercentsForOneDay(begin, _debt, rate);
                var ps = days * p;
                _percents += ps;
                HandleEvents(events, count, end, percentPayments);
                begin = eventsDate;
            }

            return Math.Round(_percents, 2);
        }

        public List<PercentViewModel> CalculatePercentsMonthly(DateTime begin, DateTime end, bool percentPayments = false)
        {
            if (begin > end)
                return null;

            var monthBegin = new DateTime(begin.Year, begin.Month, begin.Day);
            var monthEnd = new DateTime(begin.Year, begin.Month, DateTime.DaysInMonth(begin.Year, begin.Month));
            if (monthEnd > end)
            {
                monthEnd = end;
                monthBegin = monthEnd.AddDays(1);
            }

            History?.Clear();
            History = new List<PercentViewModel>();

            _percentsTotalLeft = 0;

            for (; monthBegin < end;)
            {
                var p = CalculatePercentsForOnePeriod(monthBegin, monthEnd, _percentsTotalLeft, percentPayments);
                _percentsTotalLeft += _percents;
                if (p != 0)
                    History.Add(new PercentViewModel(monthEnd, p, _percentsTotalLeft, 0));
                monthBegin = monthEnd.AddDays(1);
                monthEnd = new DateTime(monthBegin.Year, monthBegin.Month, DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

                if (monthEnd > end)
                    monthEnd = end;
            }

            return History;
        }



        #region Related private functions
        private IDateDecimal[] GetClosestEvents(out int count, IDateDecimal[] events)
        {
            count = 0;
            events[count] = GetNextAnnumRate(_rateId);
            if (events[count] != null)
                ++count;

            events[count] = GetNextPayment(_paymentId);
            if (events[count] == null)
                return null;

            switch (count == 0 ? 0 : events[0].Date.CompareTo(events[1].Date))
            {
                case -1:
                    return events;
                case 1:
                    events[0] = events[1];
                    break;
                default:
                    ++count;
                    break;
            }

            int j = -1;
            for (int i = 1; i < 3; ++i, ++count)
            {
                var p = GetNextPayment(_paymentId + i);
                events[count] = p;
                if (events[count] == null || events[count].Date != events[count - 1].Date)
                    break;
                if (p.PaymentType == PaymentType.DebtPercents)
                    j = i;
            }

            if (j >= 0)
            {
                var tmp = events[count - 1];
                events[count - 1] = events[j];
                events[j] = tmp;
            }

            return events;
        }

        private void HandleEvents(IDateDecimal[] events, int count, DateTime end, bool includePercents = false)
        {
            bool percentsWerePaid = false;
            decimal pcp = 0;

            for (int i = 0; i < count; ++i)
            {
                var e = events.ElementAt(i);
                if (e.Date > end)
                    continue;

                if (e is Payment)
                {
                    var p = (Payment) e;
                    switch (p.PaymentType)
                    {
                        case PaymentType.Credit:
                            _debt += p.Value;
                            break;
                        case PaymentType.DebtLoan:
                            _debt -= p.Value;
                            break;
                        case PaymentType.DebtPercents:
                            if (includePercents)
                            {
                                pcp = Math.Round(p.Value, 2);
                                _percentsPaid += pcp;
                                _percentsTotalLeft += _percents -  pcp;
                                percentsWerePaid = true;
                            }
                    break;
                    }

                    ++_paymentId;
                    continue;
                }

                if (e is AnnumRate)
                {
                    _rate = (AnnumRate) e;
                    ++_rateId;
                }
            }

            if (percentsWerePaid)
            {
                History.Add(new PercentViewModel(events[0].Date, _percents, Math.Round(_percentsTotalLeft, 2), pcp));
                _percents = 0;
            }
        }

        private AnnumRate GetNextAnnumRate(int previousId)
        {
            return previousId >= Rates.Count - 1 || previousId < -1 ? null : Rates.ElementAt(previousId + 1);
        }

        private Payment GetNextPayment(int previousId)
        {
            return previousId >= Payments.Count - 1 || previousId < -1 ? null : Payments.ElementAt(previousId + 1);
        }

        private AnnumRate GetRateBefore(DateTime date)
        {
            for (int i = Rates.Count - 1; i >= 0; --i)
            {
                var r = Rates.ElementAt(i);
                if (r.Date < date)
                {
                    _rateId = i;
                    return r;
                }
            }
            _rateId = -1;
            return new AnnumRate(date.AddDays(-1), 0);
        }

        private decimal CalculateDebtBefore(DateTime date)
        {
            _debt = 0;
            _percentsPaid = 0;
            _paymentId = -1;

            for (int i = 0; i < Payments.Count; ++i)
            {
                var p = Payments.ElementAt(i);
                if (p.Date >= date)
                    return _debt;
                switch (p.PaymentType)
                {
                    case PaymentType.Credit:
                        _debt += p.Value;
                        break;
                    case PaymentType.DebtLoan:
                        _debt -= p.Value;
                        break;
                    case PaymentType.DebtPercents:
                        _percentsPaid += p.Value;
                        break;
                }
                _paymentId = i;
            }

            return _debt;
        }
        #endregion
    }
}