using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestGame.InputControllers
{
    class InputGameEndController : InputController
    {
        private MouseState _oldMouseState;
        private TestGame _testGameInstance;
        private readonly GameState _gameState = GameState.Instance;

        #region singleton pattern
        private static readonly InputGameEndController instance = new InputGameEndController();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
        static InputGameEndController()
        {
        }

        private InputGameEndController()
        {
            _oldMouseState = Mouse.GetState();
        }

        public static InputGameEndController Instance
         {
             get
             {
                 return instance;
             }
         }
        #endregion

        public void ProcessInput(TestGame testGameInstance, GameTime gameTime)
        {
            _testGameInstance = testGameInstance;

            ProcessInputForGamePopup(gameTime);

            _oldMouseState = Mouse.GetState();
        }

        private void ProcessInputForGamePopup(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            
            if (newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton != ButtonState.Pressed)
            {
                ClickOnButton(newMouseState.X, newMouseState.Y);
            }

            ProcessKeyboardInputs(newState);
        }

        private void ClickOnButton(int x, int y)
        {
            if (_gameState.AgainButton.Contains(x, y))
            {
                _gameState.ResetGame();
                _gameState.CurrentGameScreen = GameState.GameScreen.Game;

                //simple trick to avoid when you click (short click, doesn't work when you hold mouse button)
                //on "play again" then card under the button is clicked on also
                //should be refactored
                Thread.Sleep(50);
            }
            else if (_gameState.EnoughButton.Contains(x, y))
            {
                ReturnToMenu();
            }
        }

        private void ProcessKeyboardInputs(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Escape))
            {
                _testGameInstance.Exit();
            }

            if (newState.IsKeyDown(Keys.Back))
            {
                ReturnToMenu();
            }
        }

        private void ReturnToMenu()
        {
            _gameState.CurrentGameScreen = GameState.GameScreen.Menu;
            _testGameInstance.IsMouseVisible = false;
        }
    }
}
