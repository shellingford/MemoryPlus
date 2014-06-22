using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestGame.InputControllers
{
    class InputMenuController : InputController
    {
        private TestGame _testGameInstance;
        private KeyboardState _oldKeyboardState;
        private readonly GameState _gameState = GameState.Instance;

        #region singleton pattern
        private static readonly InputMenuController instance = new InputMenuController();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
        static InputMenuController()
        {
        }

        private InputMenuController()
        {
            _oldKeyboardState = Keyboard.GetState();
        }

        public static InputMenuController Instance
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

            ProcessInputForMenu();

            _oldKeyboardState = Keyboard.GetState();
        }

        private void ProcessInputForMenu()
        {
            var newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Escape))
            {
                _testGameInstance.Exit();
            }

            if (newState.IsKeyDown(Keys.Down) && !_oldKeyboardState.IsKeyDown(Keys.Down))
            {
                //rotate options
                _gameState.SelectedOption++;
                RotateMenuOptionSelection();
            }

            if (newState.IsKeyDown(Keys.Up) && !_oldKeyboardState.IsKeyDown(Keys.Up))
            {
                _gameState.SelectedOption--;
                RotateMenuOptionSelection();
            }
            if (newState.IsKeyDown(Keys.Enter) && _gameState.SelectedOption == GameState.MenuOption.Start)
            {
                _gameState.CurrentGameScreen = GameState.GameScreen.Game;
                _gameState.ResetGame();
                _testGameInstance.IsMouseVisible = true;
            }
            if (newState.IsKeyDown(Keys.Enter) && _gameState.SelectedOption == GameState.MenuOption.Exit)
            {
                _testGameInstance.Exit();
            }
        }

        private void RotateMenuOptionSelection()
        {
            var numberOfEnums = Enum.GetNames(typeof (GameState.MenuOption)).Length - 1; //enums start at 0
            if ((int)_gameState.SelectedOption > numberOfEnums)
            {
                _gameState.SelectedOption = GameState.MenuOption.Start;
            }
            else if ((int)_gameState.SelectedOption < 0)
            {
                _gameState.SelectedOption = GameState.MenuOption.Exit;
            }
        }
    }
}
