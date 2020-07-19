using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Pris.ViewModels
{
    public class GroupsPageViewModel : ViewModelBase
    {
        private readonly ITransformHelper _transformHelper;
        private TournamentResponse _tournament;
        private List<Group> _groups;
        public GroupsPageViewModel(INavigationService navigationService, ITransformHelper transformHelper):base(navigationService)
        {
            _transformHelper = transformHelper;
            Title = "Groups";
        }

        public List<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }
        
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _tournament = parameters.GetValue<TournamentResponse>("tournaments");
            Groups = _transformHelper.ToGroups(_tournament.Groups);
        }
    }
}
