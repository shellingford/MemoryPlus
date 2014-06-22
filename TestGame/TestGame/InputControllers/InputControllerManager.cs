using System;
using Microsoft.Xna.Framework;

namespace TestGame.InputControllers
{
    public sealed class InputControllerManager
    {
        private readonly InputGameController _inputGameController = InputGameController.Instance;
        private readonly InputGamePopupController _inputGamePopupController = InputGamePopupController.Instance;
        private readonly InputMenuController _inputMenuController = InputMenuController.Instance;
        private readonly InputGameEndController _inputGameEndController = InputGameEndController.Instance;
        private readonly GameState _gameState = GameState.Instance;

        #region singleton pattern
        private static readonly InputControllerManager instance = new InputControllerManager();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
         static InputControllerManager()
         {
         }

         private InputControllerManager()
         {
         }

         public static InputControllerManager Instance
         {
             get
             {
                 return instance;
             }
         }
        #endregion

        public void ProcessInput(TestGame testGameInstance, GameTime gameTime)
        {
            GetCurrentInputController().ProcessInput(testGameInstance, gameTime);
        }

        private InputController GetCurrentInputController()
        {
            switch(_gameState.CurrentGameScreen)
            {
                case GameState.GameScreen.Menu: 
                    return _inputMenuController;
                case GameState.GameScreen.Game: 
                    return _inputGameController;
                case GameState.GameScreen.GamePopup:
                    return _inputGamePopupController;
                case GameState.GameScreen.EndGame:
                    return _inputGameEndController;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
