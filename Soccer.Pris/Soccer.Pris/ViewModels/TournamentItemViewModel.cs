﻿using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Models;

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

        public DelegateCommand SelectTournamentCommand => _selectTournamentCommand ?? (_selectTournamentCommand = new DelegateCommand(SelectTournamentAsync));
        public DelegateCommand selectTournament2Command => _selectTournament2Command ?? (_selectTournament2Command = new DelegateCommand(SelectTournamentForPredictionAsync));

        private async void SelectTournamentForPredictionAsync()
        {
            var parameters = new NavigationParameters
            {
                {"tournaments",this }
            };

            await _navigationService.NavigateAsync(nameof(TournamentsPageViewModel),parameters);
        }

        private async void SelectTournamentAsync()
        {
            var parameters = new NavigationParameters
            {
                {"tournaments",this }
            };

            await _navigationService.NavigateAsync(nameof(TournamentItemViewModel), parameters);
        }
    }
}