using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;
using DIP.Amenities;
using System;
using DIP.Categories;
using DIP.SubCategories;
using DIP.PressReleases;
using DIP.Commercials;
using Volo.Abp.Application.Dtos;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.JSInterop;

namespace DIP.Blazor.Pages.Site.Directory
{
    public partial class Index
    {
        [Parameter]
        public string Lang { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        private DotNetObjectReference<Index>? dotNetHelper;
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICategoriesAppService CategoriesAppService { get; set; }
        public List<CategoryFrontEnd> CategoriesList { get; set; }
        public List<CategorySubCategoryLookUp> FeaturedCategorySubCategory { get; set; }
        public Guid? SelectedCategory { get; set; } = Guid.Empty;
        public string SelectedCategoryTitle { get; set; } = String.Empty;
        [Inject]
        public ISubCategoriesAppService SubCategoriesAppService { get; set; }
        public List<SubCategoryFrontEnd> SubCategoriesList { get; set; }
        public List<SubCategoryFrontEnd> SubCategoriesIds { get; set; }
        public Guid? SelectedSubCategory { get; set; } = Guid.Empty;
        public char? SelectedStartLetter { get; set; } = null;

        [Inject]
        public ICommercialsAppService CommercialsAppService { get; set; }

        private IReadOnlyList<CommercialFrontEnd> CommercialList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int CommercialsTotalCount { get; set; } = 0;
        private int CommercialsTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 8;
        private GetCommercialsInputDetails Filter { get; set; }

        private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private string FilterText = String.Empty;

        private int start = 0;
        private int end = 5;
        private List<string> FilteredSuggestions = new();

        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("Directory");

            FeaturedCategorySubCategory = await CategoriesAppService.GetCategorySubcategoryLookupAsync();

            GetCategoriesInput getCategoriesInput = new GetCategoriesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            CategoriesList = await CategoriesAppService.GetListFrontEndAsync(getCategoriesInput);

            Filter = new GetCommercialsInputDetails()
            {
                IsActive = true,
            };
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.Sorting = "Commercial.Order";
            var res = await CommercialsAppService.GetViewListAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);

        }
        //private async Task SetSelectedCategory(string id)
        //{
        //    if (id != null)
        //    {
        //        if (Guid.Parse(id) != SelectedSubCategory)
        //        {
        //            SelectedCategory = Guid.Parse(id);
        //            SelectedSubCategory = null;
        //            GetSubCategoriesInput getSubCategoriesInput = new GetSubCategoriesInput
        //            {
        //                MaxResultCount = 1000,
        //                SkipCount = 0,
        //                CategoryId = SelectedCategory,
        //                IsActive = true,
        //                Sorting = "Order"
        //            };
        //            SubCategoriesList = await SubCategoriesAppService.GetListFrontEndAsync(getSubCategoriesInput);
        //        }
        //    }
        //    else
        //        SelectedCategory = null;
        //}

        private async Task UpdateCommercials(int page)
        {
            if (page >= 0 && page < CommercialsTotalPage && page != CurrentPage)
            {
                CurrentPage = page;
                Filter.SkipCount = CurrentPage * PageSize;
                Filter.MaxResultCount = PageSize;
                Filter.IsActive = true;
                Filter.Sorting = "Commercial.Order";
                var res = await CommercialsAppService.GetViewListAsync(Filter);
                CommercialList = res.Items;
                start = Math.Max(0, CurrentPage - 2);
                end = Math.Min(CommercialsTotalPage, CurrentPage + 3);

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }

        private async Task Search()
        {
            Filter = new GetCommercialsInputDetails();
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.Sorting = "Commercial.Order";
            Filter.StartWithLetter = SelectedStartLetter;
            Filter.SubCategoryIds = new List<Guid>();
            //Set Title
            if (SelectedCategory != Guid.Empty)
                SelectedCategoryTitle = CategoriesList.Where(category => category.Id == SelectedCategory.Value).FirstOrDefault().Title;
            else
                SelectedCategoryTitle = String.Empty;
            //Search Filter
            if (SelectedSubCategory != Guid.Empty)
                Filter.SubCategoryIds.Add(SelectedSubCategory.Value);
            else
            {
                if (SelectedCategory != Guid.Empty)
                {

                    Filter.SubCategoryIds = !SubCategoriesList.IsNullOrEmpty() ? SubCategoriesList.Select(s => s.Id).ToList() : new List<Guid>();
                }
            }
            var res = await CommercialsAppService.GetViewListAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            //if (page >= 0 && page < CommercialsTotalPage)
            //{
            //    CurrentPage = page;
            //    Filter.SkipCount = CurrentPage * PageSize;
            //    Filter.MaxResultCount = PageSize;
            //    Filter.Sorting = "Order";
            //    var res = await CommercialsAppService.GetViewListAsync(Filter);
            //    CommercialList = res.Items;
            //    await InvokeAsync(() =>
            //    {
            //        StateHasChanged();
            //    });
            //}

        }

        private async Task Search(CategorySubCategoryLookUp featuredCategorySubCategory)
        {
            Filter = new GetCommercialsInputDetails();
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.Sorting = "Commercial.Order";
            Filter.StartWithLetter = SelectedStartLetter;
            Filter.SubCategoryIds = new List<Guid>();
            if (featuredCategorySubCategory.IsCategory)
                SelectedCategoryTitle = featuredCategorySubCategory.Title;
            else
                SelectedCategoryTitle = featuredCategorySubCategory.ParentTitle;

            if (featuredCategorySubCategory.IsCategory == false)
                Filter.SubCategoryIds.Add(featuredCategorySubCategory.Id);
            else
            {
                if (featuredCategorySubCategory != null)
                {
                    GetSubCategoriesInput getSubCategoriesInput = new GetSubCategoriesInput
                    {
                        MaxResultCount = 1000,
                        SkipCount = 0,
                        CategoryId = featuredCategorySubCategory.Id,
                        IsActive = true,
                        Sorting = "SubCategory.Order"
                    };
                    SubCategoriesIds = await SubCategoriesAppService.GetListFrontEndAsync(getSubCategoriesInput);
                    Filter.SubCategoryIds = !SubCategoriesIds.IsNullOrEmpty() ? SubCategoriesIds.Select(s => s.Id).ToList() : new List<Guid>();
                }
            }
            var res = await CommercialsAppService.GetViewListAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }


        private async Task SearchByText()
        {

            Filter = new GetCommercialsInputDetails();
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.Sorting = "Commercial.Order";
            Filter.SubCategoryIds = new List<Guid>();

            //Search Filter
            Filter.FilterText = FilterText;

            var res = await CommercialsAppService.GetViewListByTextAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        [JSInvokable]
        public async Task OnSelectedCategoryValueChanged(Guid? value)
        {
            SelectedCategory = value;
            if (SelectedCategory != Guid.Empty)
            {
                GetSubCategoriesInput getSubCategoriesInput = new GetSubCategoriesInput
                {
                    MaxResultCount = 1000,
                    SkipCount = 0,
                    CategoryId = SelectedCategory,
                    IsActive = true,
                    Sorting = "SubCategory.Order"
                };
                SubCategoriesList = await SubCategoriesAppService.GetListFrontEndAsync(getSubCategoriesInput);
            }
            else
            {
                SubCategoriesList = new List<SubCategoryFrontEnd>();
            }
            SelectedSubCategory = Guid.Empty;

            await Search();
        }
        [JSInvokable]
        public async Task OnSelectedSubCategoryValueChanged(Guid? value)
        {
            SelectedSubCategory = value;

            await Search();

        }
        async Task OnSelectedStartLetterValueChanged(char? value)
        {
            SelectedStartLetter = value;
            await Search();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await Task.Delay(1000);
                await JS.InvokeVoidAsync("select2_init", dotNetHelper);
            }
            //// await JS.InvokeVoidAsync("hideProgress");
        }
        private async Task OnSearchChanged(ChangeEventArgs e)
        {
            var searchText = e.Value.ToString();
            // You can call your search method here
            Filter = new GetCommercialsInputDetails();
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.Sorting = "Commercial.Order";
            Filter.SubCategoryIds = new List<Guid>();

            //Search Filter
            Filter.FilterText = searchText;

            var res = await CommercialsAppService.GetViewListByTextAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);
          
            if(CommercialList.Count> 0)
            {
                FilteredSuggestions = CommercialList.Select(b => b.Title).ToList();

            }
            if(searchText.IsNullOrEmpty())
            {
                FilteredSuggestions.Clear();
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private async Task OnSearchSelectChanged(ChangeEventArgs e)
        {
            var searchText = e.Value.ToString();
            // You can call your search method here
            Filter = new GetCommercialsInputDetails();
            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.Sorting = "Commercial.Order";
            Filter.SubCategoryIds = new List<Guid>();

            //Search Filter
            Filter.FilterText = searchText;

            var res = await CommercialsAppService.GetViewListByTextAsync(Filter);
            CommercialList = res.Items;
            CurrentPage = 0;
            CommercialsTotalCount = Convert.ToInt32(res.TotalCount);
            CommercialsTotalPage = CommercialsTotalCount / PageSize;
            if (CommercialsTotalCount % PageSize != 0)
                CommercialsTotalPage += 1;
            start = Math.Max(0, CurrentPage - 2);
            end = Math.Min(CommercialsTotalPage, CurrentPage + 3);


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private async void SelectSuggestion(string suggestion)
        {
            ChangeEventArgs changeEventArgs = new ChangeEventArgs();
            changeEventArgs.Value = suggestion;
            FilteredSuggestions.Clear();
            FilterText = String.Empty;
            await OnSearchSelectChanged(changeEventArgs);
          

        }
    }

    
    }

