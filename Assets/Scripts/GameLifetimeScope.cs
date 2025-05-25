using VContainer;
using VContainer.Unity;
using Model;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Score>(Lifetime.Singleton); // Scoreをシングルトンで登録
    }
}
