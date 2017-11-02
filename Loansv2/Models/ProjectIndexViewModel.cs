using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Loansv2.DAL;
using PagedList;

namespace Loansv2.Models
{
    public enum ProjectParamToSearchBy
    {
        [Display(Name = "Название")]
        Name
    }

    public enum ProjectParamToSortBy
    {
        None,
        [Display(Name = "Название")]
        Name
    }

    public class ProjectIndexViewModel
    {
        private IQueryable<Project> _query;
        private bool _searchNeeded = false;

        [Display(Name = "Проект")]
        public string Name { get; set; }

        public IPagedList<Project> Projects { get; set; }


        public ProjectParamToSearchBy SearchParam { get; set; }
        public string SearchText { get; set; }

        public ProjectParamToSortBy? SortParam { get; set; }
        public SortOrderType? SortOrder { get; set; }


        #region Constructors
        public ProjectIndexViewModel()
        {
        }

        public ProjectIndexViewModel(IQueryable<Project> query, ProjectParamToSortBy? sortParam, SortOrderType? sortOrder,
            ProjectParamToSearchBy? searchParam, string searchText)
        {
            _query = query;
            SortParam = sortParam;
            SortOrder = sortOrder;
            if (searchParam != null)
            {
                SearchParam = (ProjectParamToSearchBy) searchParam;
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

            switch (SearchParam)
            {
                case ProjectParamToSearchBy.Name:
                    _query = _query.Where(l => l.Name.ToLower().Contains(SearchText));
                    break;
            }
        }
        #endregion

        #region Sort
        private void Sort()
        {
            if (SortParam == null || SortParam == ProjectParamToSortBy.None ||
                SortOrder == null || SortOrder == SortOrderType.Default)
            {
                _query = _query?.OrderBy(p => p.Id);
                return;
            }

            switch (SortParam)
            {
                case ProjectParamToSortBy.Name:
                    _query = SortOrder == SortOrderType.Ascending
                        ? _query.OrderBy(p => p.Name)
                        : (SortOrder == SortOrderType.Descending ? _query.OrderByDescending(p => p.Name) : _query);
                    break;
            }
        }

        public SortOrderType NextSortOrder(ProjectParamToSortBy sortParam)
        {
            if (sortParam != SortParam)
                return SortOrderType.Ascending;
            if (SortOrder == null)
                return SortOrderType.Default;

            int nextOrder = (int)SortOrder + 1;
            return nextOrder >= Enum.GetNames(typeof(SortOrderType)).Length ? SortOrderType.Ascending : (SortOrderType)nextOrder;
        }
        #endregion

        public void ApplyFilters(int pageNumber, int pageSize)
        {
            if (_searchNeeded)
                Search();
            Sort();
            Projects = _query.ToPagedList(pageNumber, pageSize);
        }
    }
}