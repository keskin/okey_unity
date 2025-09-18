using VContainer;
using VContainer.Unity;
using Scene.Game; // Game namespace'ini ekledik

namespace Di
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<GameView>().As<IGameView>();
            builder.Register<GamePresenter>(Lifetime.Scoped);
        }
    }
}