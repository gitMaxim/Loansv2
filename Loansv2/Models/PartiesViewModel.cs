using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Loansv2.DAL;
using PagedList;

namespace Loansv2.Models
{
    public enum PartyParamToSearchBy
    {
        [Display(Name = "ИНН")]
        VatId,
        [Display(Name = "Наименование")]
        Party,
        [Display(Name = "ФЛ (Ф.И.О. через пробел)")]
        Physical,
        [Display(Name = "ИП (наименование)")]
        Invidivual,
        [Display(Name = "ЮЛ (наименование)")]
        Juristic,
        [Display(Name = "Телефон")]
        Phone,
        [Display(Name = "Эл. почта")]
        Email
    }

    public enum PartyParamToSortBy
    {
        None,
        [Display(Name = "Наименование")]
        Name,
        [Display(Name = "Тип")]
        Type
    }

    public class PartiesViewModel
    {
        private IQueryable<Party> _query;
        private List<PhysicalParty> _physicalParties;
        private bool _searchNeeded = false;
        
        public Party Party { get; set; }
        public IPagedList<Party> Parties { get; set; }

        public PartyParamToSearchBy SearchParam { get; set; }
        public string SearchText { get; set; }

        public PartyParamToSortBy? SortParam { get; set; }
        public SortOrderType? SortOrder { get; set; }
        

        #region Constructors
        public PartiesViewModel()
        {
            Party = new Party();
        }

        public PartiesViewModel(IQueryable<Party> query, PartyParamToSortBy? sortParam, SortOrderType? sortOrder, 
            PartyParamToSearchBy? searchParam, string searchText) : this()
        {
            _query = query;
            SortParam = sortParam;
            SortOrder = sortOrder;
            if (searchParam != null)
            { 
                SearchParam = (PartyParamToSearchBy) searchParam;
                _searchNeeded = true;
            }
            SearchText = searchText;
        }
        #endregion

        #region Search
        private void Search(IQueryable<Phone> queryPhones, IQueryable<Email> queryEmails, IQueryable<PhysicalParty> queryPhysicalParty)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;
            
            SearchText = SearchText.Trim();
            SearchText = SearchText.ToLower();

            switch (SearchParam)
            {
                case PartyParamToSearchBy.VatId:
                    _query = _query.Where(p => p.VatId.ToLower().Contains(SearchText.ToLower()));
                    break;
                case PartyParamToSearchBy.Party:
                    _query = _query.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()));
                    break;
                case PartyParamToSearchBy.Physical:
                    SearchPhysicalParty(queryPhysicalParty);
                    if (_physicalParties != null)
                        _query = _query.Join(_physicalParties.Select(pp => pp.Id), p => p.Id, id => id, (p, id) => p);
                    break;
                case PartyParamToSearchBy.Invidivual:
                    _query = _query.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()) && p.PartyType == PartyType.Individual);
                    break;
                case PartyParamToSearchBy.Juristic:
                    _query = _query.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()) && p.PartyType == PartyType.Juristic);
                    break;
                case PartyParamToSearchBy.Phone:
                    var phoneOwners = queryPhones.Where(p => p.Number.ToLower().Contains(SearchText))
                        .Select(p => p.PartyId)
                        .Distinct()
                        .ToList();
                    _query = _query.Join(phoneOwners, p => p.Id, id => id, (p, id) => p);
                    break;
                case PartyParamToSearchBy.Email:
                    var emailOwners = queryEmails.Where(p => p.Address.ToLower().Contains(SearchText))
                        .Select(p => p.PartyId)
                        .Distinct()
                        .ToList();
                    _query = _query.Join(emailOwners, p => p.Id, id => id, (p, id) => p);
                    break;
            }
        }

        private void SearchPhysicalParty(IQueryable<PhysicalParty> queryPhysicalParty)
        {
            var words = SearchText.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            // 'cause Linq doesn't work with array indexes I had to make temp variables
            string lastName, firstName, middleName;

            switch (words.Count())
            {
                case 1:
                    lastName = words[0];
                    _physicalParties = queryPhysicalParty.Where(p => p.LastName.ToLower().Contains(lastName.ToLower())).ToList();
                    break;
                case 2:
                    lastName = words[0];
                    firstName = words[1];
                    _physicalParties = queryPhysicalParty.Where(p => p.LastName.ToLower().Contains(lastName.ToLower())
                                                                      && p.FirstName.ToLower().Contains(firstName.ToLower())).ToList();
                    break;
                case 3:
                    lastName = words[0];
                    firstName = words[1];
                    middleName = words[2];
                    _physicalParties = queryPhysicalParty.Where(p => p.LastName.ToLower().Contains(lastName.ToLower())
                                                                      && p.FirstName.ToLower().Contains(firstName.ToLower())
                                                                      && p.MiddleName != null
                                                                      && p.MiddleName.ToLower().Contains(middleName.ToLower())).ToList();
                    break;
                default:
                    _physicalParties = null;
                    break;
            }
        }
        #endregion

        #region Sort
        private void Sort()
        {
            if (SortParam == null || SortParam == PartyParamToSortBy.None ||
                SortOrder == null || SortOrder == SortOrderType.Default)
            {
                _query = _query.OrderBy(p => p.Id);
                return;
            }

            switch (SortParam)
            {
                case PartyParamToSortBy.Name:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(p => p.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(p => p.Name) : _query);
                    break;
                case PartyParamToSortBy.Type:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(p => p.PartyType)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(p => p.PartyType) : _query);
                    break;
            }
        }

        public SortOrderType NextSortOrder(PartyParamToSortBy sortParam)
        {
            if (sortParam != SortParam)
                return SortOrderType.Ascending;
            if (SortOrder == null)
                SortOrder = SortOrderType.Default;

            int nextOrder = (int) SortOrder + 1;
            return nextOrder >= Enum.GetNames(typeof(SortOrderType)).Length ? SortOrderType.Ascending : (SortOrderType)nextOrder;
        }
        #endregion

        public void ApplyFilters(IQueryable<Phone> queryPhones, IQueryable<Email> queryEmails, IQueryable<PhysicalParty> queryPhysicalParty, 
            int pageNumber, int pageSize)
        {
            if (_searchNeeded)
                Search(queryPhones, queryEmails, queryPhysicalParty);
            Sort();

            Parties = _query.ToPagedList(pageNumber, pageSize);
        }
    }
}