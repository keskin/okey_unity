using Scene.Boot;
using VContainer;
using VContainer.Unity;

namespace Di
{
    public class BootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<BootView>().As<IBootView>();
            builder.Register<BootPresenter>(Lifetime.Scoped).As<IAsyncStartable>();
        }
    }
}