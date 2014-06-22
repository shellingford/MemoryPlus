using Microsoft.Xna.Framework;

namespace TestGame.InputControllers
{
    interface InputController
    {
        void ProcessInput(TestGame testGameInstance, GameTime gameTime);
    }
}
