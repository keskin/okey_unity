using UnityEngine;
using VContainer;
using TMPro;

namespace Scene.Game
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private TextMeshProUGUI messagesText;
        private GamePresenter _presenter;

        [Inject]
        public void Construct(GamePresenter presenter)
        {
            _presenter = presenter;
            _presenter.OnNewMessage += AppendMessage;
        }

        private void OnDestroy()
        {
            if (_presenter != null)
            {
                _presenter.OnNewMessage -= AppendMessage;
            }
        }

        private void AppendMessage(string message)
        {
            if (messagesText != null)
            {
                messagesText.text += message + "\n";
            }
        }
    }
}