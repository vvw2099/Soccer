using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Pris.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;

namespace Soccer.Pris.ViewModels
{
    public class SoccerMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private UserResponse _user;
        public SoccerMasterDetailPageViewModel(INavigationService navigationService,IApiService apiService):base(navigationService)
        {
           
            _navigationService = navigationService;
            _apiService = apiService;
            LoadUser();
            LoadMenus();
        }
        
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }
        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Icon="tournament",
                    PageName="TournamentsPage",
                    Title=Languages.Tournaments,
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="prediction",
                    PageName="MyPredictionsPage",
                    Title=Languages.MyPredictions,
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="medal",
                    PageName="MyPositionsPage",
                    Title= Languages.MyPositions,
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="user",
                    PageName="ModifyUserPage",
                    Title=Languages.ModifyUser,
                    IsLoginRequired=false
                },
                new Menu
                {
                    Icon="login",
                    PageName="LoginPage",
                    Title= Settings.IsLogin? Languages.Logout:Languages.Login
                    
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
        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            }
        }
    }
}
