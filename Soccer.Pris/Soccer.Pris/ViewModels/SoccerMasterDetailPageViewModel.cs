using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;

namespace Soccer.Pris.ViewModels
{
    public class SoccerMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        public SoccerMasterDetailPageViewModel(INavigationService navigationService,IApiService apiService):base(navigationService)
        {
           
            _navigationService = navigationService;
            _apiService = apiService;
            LoadMenus();
        }
        
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }
        
        
        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Icon="tournament",
                    PageName="TournamentsPage",
                    Title="Tournaments",
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="prediction",
                    PageName="MyPredictionsPage",
                    Title="Predictions",
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="medal",
                    PageName="MyPositionsPage",
                    Title="My Position",
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="user",
                    PageName="ModifyUserPage",
                    Title="Modify User",
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="login",
                    PageName="LoginPage",
                    Title="Login",
                    IsLoginRequired=false
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
    }
}
