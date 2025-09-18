using UnityEngine;
using VContainer;

namespace Scene.Game
{
    public class GameView : MonoBehaviour, IGameView
    {
        [Inject]
        public void Construct(GamePresenter presenter)
        {
            presenter.InitializeGame();
        }
    }
}