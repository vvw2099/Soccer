using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Helpers;
using Soccer.Pris.Views;
using Newtonsoft.Json;

namespace Soccer.Pris.ViewModels
{
    public class TournamentItemViewModel : TournamentResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectTournamentCommand;
        private DelegateCommand _selectTournament2Command;

        public TournamentItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectTournamentCommand => _selectTournamentCommand ??
            (_selectTournamentCommand = new DelegateCommand(SelectTournamentAsync));
        public DelegateCommand SelectTournament2Command => _selectTournament2Command ??
            (_selectTournament2Command = new DelegateCommand(SelectTournamentForPredictionAsync));

        private async void SelectTournamentForPredictionAsync()
        {

            NavigationParameters parameters = new NavigationParameters
            {
                {"tournament",this }
            };
            

            await _navigationService.NavigateAsync(nameof(PredictionsTabbedPage),parameters);
        }

        private async void SelectTournamentAsync()
        {
            var parameters = new NavigationParameters
            {
                {"tournament",this }
            };
            //Settings.Tournament = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync(nameof(TournamentTabbedPage), parameters);
        }
    }
}
