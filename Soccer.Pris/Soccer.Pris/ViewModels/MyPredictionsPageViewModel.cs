using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Pris.ViewModels
{
    public class MyPredictionsPageViewModel : ViewModelBase
    {
        public MyPredictionsPageViewModel(INavigationService navigationService):base(navigationService)
        {
            Title = "Predictions";
        }
    }
}
