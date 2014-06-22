using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestGame.InputControllers
{
    class InputGamePopupController : InputController
    {
        private MouseState _oldMouseState;
        private TestGame _testGameInstance;
        private readonly GameState _gameState = GameState.Instance;
        private readonly Stopwatch _messageStopwatch;

        private const long ClosePopupAfterAnswerInSec = 1;

        #region singleton pattern
        private static readonly InputGamePopupController instance = new InputGamePopupController();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
        static InputGamePopupController()
        {
        }

        private InputGamePopupController()
        {
            _messageStopwatch = new Stopwatch();
            _oldMouseState = Mouse.GetState();
        }

        public static InputGamePopupController Instance
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

            if (_messageStopwatch.Elapsed.TotalSeconds > 0)
            {
                if (_messageStopwatch.Elapsed.TotalSeconds >= ClosePopupAfterAnswerInSec)
                {
                    StopAndResetCountdown();
                    if (_gameState.GameHasEnded())
                    {
                        _gameState.CurrentGameScreen = GameState.GameScreen.EndGame;
                    }
                    else
                    {
                        _gameState.ResetFromPopupToGame();
                    }
                }
            }

            if (newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton != ButtonState.Pressed)
            {
                ClickOnButton(newMouseState.X, newMouseState.Y);
            }

            ProcessKeyboardInputs(newState);
        }

        private void ClickOnButton(int x, int y)
        {
            if (ClickedOnButton(x, y))
            {
                _messageStopwatch.Start();
            }

            ProcessAnswerCorrectness(x, y);

            if ((_gameState.YesButton.Contains(x, y) || _gameState.NoButton.Contains(x, y)) && 
                BothCardsOpened() && !CardPairFound())
            {
                CloseCards();
            }
        }

        private void ProcessAnswerCorrectness(int x, int y)
        {
            if (_gameState.YesButton.Contains(x, y) && CardPairFound())
            {
                _gameState.PlayerAnsweredCorrectly();
                CloseCards();
            }
            else if (_gameState.NoButton.Contains(x, y) && CardPairFound())
            {
                _gameState.PlayerOnlyFoundPair();
                CloseCards();
            }
            else if (_gameState.YesButton.Contains(x, y) && !CardPairFound())
            {
                _gameState.PlayerAnsweredIncorrectly();
            }
            else if (_gameState.NoButton.Contains(x, y) && !CardPairFound())
            {
                _gameState.PlayerAnsweredCorrectlyWithoutPair();
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
                _gameState.CurrentGameScreen = GameState.GameScreen.Menu;
                _testGameInstance.IsMouseVisible = false;
            }
        }

        private void CloseCards()
        {
            //if they don't match then close them
            if (!CardPairFound())
            {
                _gameState.Player1.CurrentMove = !_gameState.Player1.CurrentMove;
                _gameState.Player2.CurrentMove = !_gameState.Player2.CurrentMove;
            }

            _gameState.CloseCards();
        }

        private bool BothCardsOpened()
        {
            return _gameState.OpenCard1 != null && _gameState.OpenCard2 != null;
        }

        private bool ClickedOnButton(int x, int y)
        {
            return _gameState.NoButton.Contains(x, y) || _gameState.YesButton.Contains(x, y);
        }

        private bool CardPairFound()
        {
            return BothCardsOpened() && _gameState.OpenCard2.PairCardID == _gameState.OpenCard1.ID;
        }

        private void StopAndResetCountdown()
        {
            _messageStopwatch.Stop();
            _messageStopwatch.Reset();
        }
    }
}
