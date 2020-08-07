using Prism.Ioc;
using Prism.Modularity;
using Soccer.prueba.Views;
using Soccer.prueba.ViewModels;

namespace Soccer.prueba
{
    public class pruebaModule : IModule
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
