using Networking;
using Service;
using GameLogic;

namespace Scene.Game
{
    public class GamePresenter
    {
        private readonly IAssetManager _assetManager;
        private readonly IWebSocketNetworking _webSocketNetworking;
        private readonly SmartBot _smartBot;

        public GamePresenter(IAssetManager assetManager, IWebSocketNetworking webSocketNetworking, SmartBot smartBot)
        {
            _assetManager = assetManager;
            _webSocketNetworking = webSocketNetworking;
            _smartBot = smartBot;
        }

        public void InitializeGame()
        {
            _smartBot.CalculateNextMove();
        }
    }
}