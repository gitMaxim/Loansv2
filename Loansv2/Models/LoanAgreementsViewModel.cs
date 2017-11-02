using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Loansv2.DAL;
using PagedList;

namespace Loansv2.Models
{
    public enum LoanAgreementParamToSearchBy
    {
        [Display(Name = "№ договора")]
        Number,
        [Display(Name = "Контрагент")]
        Party,
        [Display(Name = "Займодатель")]
        Creditor,
        [Display(Name = "Заёмщик")]
        Debtor,
        [Display(Name = "Проект")]
        Project,
        [Display(Name = "Проект займодатель")]
        CreditorProject,
        [Display(Name = "Проект заёмщик")]
        DebtorProject,
        [Display(Name = "Дата подписания не раньше")]
        SignDateAfter,
        [Display(Name = "Дата подписания не позднее")]
        SignDateBefore,
        [Display(Name = "Дата окончания не раньше")]
        DeadlineDateAfter,
        [Display(Name = "Дата окончания не позднее")]
        DeadlineDateBefore,
        [Display(Name = "Сумма займа не более")]
        SumNotAbove,
        [Display(Name = "Сумма займа не менее")]
        SumNotBelow
    }

    public enum LoanAgreementParamToSortBy
    {
        None,
        [Display(Name = "№ договора")]
        Number,
        [Display(Name = "Займодатель")]
        Creditor,
        [Display(Name = "Проект займодателя")]
        CreditorProject,
        [Display(Name = "Заёмщик")]
        Debtor,
        [Display(Name = "Проект заёмщика")]
        DebtorProject,
        [Display(Name = "Дата подписания")]
        SignDate,
        [Display(Name = "Дата окончания")]
        DeadlineDate,
        [Display(Name = "Сумма займа")]
        Sum
    }

    public enum SortOrderType
    {
        Ascending,
        Descending,
        Default
    }


    public class LoanAgreementsViewModel
    {
        private IQueryable<LoanAgreement> _query;
        private bool _searchNeeded = false;

        public LoanAgreement LoanAgreement { get; set; }
        public IPagedList<LoanAgreement> LoanAgreements { get; set; }


        public LoanAgreementParamToSearchBy SearchParam { get; set; }
        public string SearchText { get; set; }

        public LoanAgreementParamToSortBy? SortParam { get; set; }
        public SortOrderType? SortOrder { get; set; }


        #region Constructors
        public LoanAgreementsViewModel()
        {
            LoanAgreement = new LoanAgreement();
        }

        public LoanAgreementsViewModel(IQueryable<LoanAgreement> query, LoanAgreementParamToSortBy? sortParam, SortOrderType? sortOrder, 
            LoanAgreementParamToSearchBy? searchParam, string searchText) : this()
        {
            _query = query;
            SortParam = sortParam;
            SortOrder = sortOrder;
            if (searchParam != null)
            {
                SearchParam = (LoanAgreementParamToSearchBy) searchParam;
                _searchNeeded = true;
            }
            SearchText = searchText;
        }
        #endregion

        #region Search
        public void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            SearchText = SearchText.Trim();
            SearchText = SearchText.ToLower();
            decimal num;
            DateTime date;

            switch (SearchParam)
            {
                case LoanAgreementParamToSearchBy.Number:
                    _query = _query.Where(l => l.Number.Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.Project:
                    _query = _query.Where(l => l.CreditorProject.Name.ToLower().Contains(SearchText) 
                        || l.DebtorProject.Name.ToLower().Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.CreditorProject:
                    _query = _query.Where(l => l.CreditorProject.Name.ToLower().Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.DebtorProject:
                    _query = _query.Where(l => l.DebtorProject.Name.ToLower().Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.Party:
                    _query = _query.Where(l => l.Creditor.Name.ToLower().Contains(SearchText) 
                        || l.Debtor.Name.Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.Creditor:
                    _query = _query.Where(l => l.Creditor.Name.ToLower().Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.Debtor:
                    _query = _query.Where(l => l.Debtor.Name.ToLower().Contains(SearchText));
                    break;
                case LoanAgreementParamToSearchBy.SignDateAfter:
                    if (DateTime.TryParse(SearchText, out date))
                        _query = _query.Where(l => l.SignDate.CompareTo(date) >= 0);
                    break;
                case LoanAgreementParamToSearchBy.SignDateBefore:
                    if (DateTime.TryParse(SearchText, out date))
                        _query = _query.Where(l => l.SignDate.CompareTo(date) <= 0);
                    break;
                case LoanAgreementParamToSearchBy.DeadlineDateAfter:
                    if (DateTime.TryParse(SearchText, out date))
                        _query = _query.Where(l => l.DeadlineDate.CompareTo(date) >= 0);
                    break;
                case LoanAgreementParamToSearchBy.DeadlineDateBefore:
                    if (DateTime.TryParse(SearchText, out date))
                        _query = _query.Where(l => l.DeadlineDate.CompareTo(date) <= 0);
                    break;
                case LoanAgreementParamToSearchBy.SumNotAbove:
                    if (decimal.TryParse(SearchText, out num))
                        _query = _query.Where(l => l.Sum <= num);
                    break;
                case LoanAgreementParamToSearchBy.SumNotBelow:
                    if (decimal.TryParse(SearchText, out num))
                        _query = _query.Where(l => l.Sum >= num);
                    break;
            }
        }
        #endregion

        #region Sort
        public void Sort()
        {
            if (SortParam == null || SortParam == LoanAgreementParamToSortBy.None || 
                SortOrder == null || SortOrder == SortOrderType.Default)
            {
                _query = _query.OrderBy(l => l.Id);
                return;
            }

            switch (SortParam)
            {
                case LoanAgreementParamToSortBy.Number:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.Number)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.Number) : _query);
                    break;
                case LoanAgreementParamToSortBy.Creditor:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.Creditor.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.Creditor.Name) : _query);
                    break;
                case LoanAgreementParamToSortBy.CreditorProject:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.CreditorProject.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.CreditorProject.Name) : _query);
                    break;
                case LoanAgreementParamToSortBy.Debtor:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.Debtor.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.Debtor.Name) : _query);
                    break;
                case LoanAgreementParamToSortBy.DebtorProject:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.DebtorProject.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.DebtorProject.Name) : _query);
                    break;
                case LoanAgreementParamToSortBy.SignDate:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.SignDate)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.SignDate) : _query);
                    break;
                case LoanAgreementParamToSortBy.DeadlineDate:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.DeadlineDate)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.DeadlineDate) : _query);
                    break;
                case LoanAgreementParamToSortBy.Sum:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(l => l.Sum)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(l => l.Sum) : _query);
                    break;
            }
        }

        public SortOrderType NextSortOrder(LoanAgreementParamToSortBy sortParam)
        {
            if (sortParam != SortParam)
                return SortOrderType.Ascending;
            if (SortOrder == null)
                return SortOrderType.Default;

            int nextOrder = (int) SortOrder + 1;
            return nextOrder >= Enum.GetNames(typeof(SortOrderType)).Length ? SortOrderType.Ascending : (SortOrderType) nextOrder;
        }
        #endregion

        public void ApplyFilters(int pageNumber, int pageSize)
        {
            _query = _query.Include(l => l.Creditor)
                .Include(l => l.CreditorProject)
                .Include(l => l.Debtor)
                .Include(l => l.DebtorProject);

            if (_searchNeeded)
                Search();
            Sort();
            LoanAgreements = _query.ToPagedList(pageNumber, pageSize);
        }
    }
}