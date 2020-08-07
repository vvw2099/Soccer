using Prism.Navigation;
using Soccer.Common.Models;

namespace Soccer.Pris.ViewModels
{
    public class TournamentTabbedPageViewModel : ViewModelBase
    {
        private TournamentResponse _tournament;
        public TournamentTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Soccer";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _tournament = parameters.GetValue<TournamentResponse>("tournaments");
            Title = _tournament.Name;
        }
    }
}
