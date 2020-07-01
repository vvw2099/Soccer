using Prism.Ioc;
using Prism.Modularity;
using Soccer.Prism.Views;
using Soccer.Prism.ViewModels;

namespace Soccer.Prism
{
    public class PrismModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA, ViewAViewModel>();
        }
    }
}
