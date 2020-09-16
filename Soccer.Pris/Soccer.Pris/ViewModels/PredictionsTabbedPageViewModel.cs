using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.Common.Models;
using Soccer.Pris.Helpers;
using Prism.Navigation;

namespace Soccer.Pris.ViewModels
{
    public class PredictionsTabbedPageViewModel : ViewModelBase
    {
        private TournamentResponse _tournament;
        public PredictionsTabbedPageViewModel(INavigationService navigationService):base(navigationService)
        {
            Title = Languages.PredictionsFor;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("tournament"))
            {
                _tournament = parameters.GetValue<TournamentResponse>("tournament");
                Title = $"{_tournament.Name}";
            }
        }
    }
}
