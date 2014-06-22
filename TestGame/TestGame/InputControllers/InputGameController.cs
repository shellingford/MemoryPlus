using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestGame.InputControllers
{
    class InputGameController : InputController
    {
        private MouseState _oldMouseState;
        private TestGame _testGameInstance;
        private readonly GameState _gameState = GameState.Instance;

        #region singleton pattern
        private static readonly InputGameController instance = new InputGameController();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
        static InputGameController()
        {
        }

        private InputGameController()
        {
            _oldMouseState = Mouse.GetState();
         }

         public static InputGameController Instance
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

            ProcessInputForGame();

            _oldMouseState = Mouse.GetState();
        }

        private void ProcessInputForGame()
        {
            KeyboardState newState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            if (newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton != ButtonState.Pressed)
            {
                ClickOnCard(newMouseState.X, newMouseState.Y);
            }

            ProcessKeyboardInputs(newState);
        }

        private void ProcessKeyboardInputs(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Escape))
            {
                _testGameInstance.Exit();
            }

            if (newState.IsKeyDown(Keys.Back))
            {
                _gameState.CurrentGameScreen = GameState.GameScreen.Menu;
                _testGameInstance.IsMouseVisible = false;
            }
        }

        private void ClickOnCard(int x, int y)
        {
            if (_gameState.OpenCard1 != null && _gameState.OpenCard2 != null)
            {
                return;
            }
            for (int i = 0; i < _gameState.MemoryMatrixSizeHeight; i++)
            {
                bool endLoop = false;
                for (int j = 0; j < _gameState.MemoryMatrixSizeWidth; j++)
                {
                    if (_gameState.Cards[i][j].IsWithinCard(x, y) &&
                        !_gameState.Cards[i][j].Disabled)
                    {
                        if (_gameState.OpenCard1 == null)
                        {
                            _gameState.OpenCard1 = _gameState.Cards[i][j];
                        }
                        else if (_gameState.OpenCard2 == null && !_gameState.OpenCard1.Equals(_gameState.Cards[i][j]))
                        {
                            _gameState.OpenCard2 = _gameState.Cards[i][j];
                            _gameState.CurrentGameScreen = GameState.GameScreen.GamePopup;
                        }
                        else if (_gameState.OpenCard2 == null && _gameState.OpenCard1.Equals(_gameState.Cards[i][j]))
                        {
                            endLoop = true;
                            break;
                        }

                        switch (_gameState.Cards[i][j].CurrentAnimation)
                        {
                            case Card.CardAnimation.Cover:
                                _gameState.Cards[i][j].CurrentAnimation = Card.CardAnimation.Face;
                                break;
                            case Card.CardAnimation.Face:
                                _gameState.Cards[i][j].CurrentAnimation = Card.CardAnimation.Cover;
                                break;
                        }
                        endLoop = true;
                        break;
                    }
                    if (_gameState.Cards[i][j].IsWithinCard(x, y) &&
                        _gameState.Cards[i][j].Disabled)
                    {
                        endLoop = true;
                        break;
                    }
                }
                if(endLoop) break;
            }
        }
    }
}
