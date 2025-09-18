using VContainer;
using VContainer.Unity;
using Scene.Home;

namespace Di
{
    public class HomeLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<HomeView>().As<IHomeView>();
            builder.Register<HomePresenter>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        }
    }
}