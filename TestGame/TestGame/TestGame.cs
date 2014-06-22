using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestGame.InputControllers;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Game
    {
        private const string Title = "MEMORY+";

        readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _coverImage;
        private readonly Color _gameBackgroundColor;

        private Texture2D _pixel;

        private SpriteFont _smallMenuFont;
        private SpriteFont _bigMenuFont;
        private SpriteFont _titleFont;

        private readonly GameState _gameState = GameState.Instance;
        private readonly InputControllerManager _inputControllerManager = InputControllerManager.Instance;

        public TestGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.PreferredBackBufferWidth = 1000;
            Window.Title = "Memory+";
            _gameBackgroundColor = Color.Aquamarine;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            //_inputControllerManager.SetTestGameInstance(this);

            _gameState.Cards = new Card[_gameState.MemoryMatrixSizeHeight][];
            for (int i = 0; i < _gameState.MemoryMatrixSizeHeight; i++)
            {
                _gameState.Cards[i] = new Card[_gameState.MemoryMatrixSizeWidth];
            }

            _gameState.Player1 = new Player("Player One", Color.Blue) {CurrentMove = true};
            _gameState.Player2 = new Player("Player Two", Color.Red);

            LoadGroups();
        }

        public void LoadGroups()
        {
            var cardsCollection = new CardsCollection();
            cardsCollection.LoadGroups(Content, _coverImage, _graphics, _gameState);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _coverImage = Content.Load<Texture2D>("cover");

            _smallMenuFont = Content.Load<SpriteFont>("MenuFontSmall");
            _bigMenuFont = Content.Load<SpriteFont>("MenuFontBig");
            _titleFont = Content.Load<SpriteFont>("Title2");
            
            _pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _inputControllerManager.ProcessInput(this, gameTime);
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //_inputControllerManager.SetGameTime(gameTime);
            switch (_gameState.CurrentGameScreen)
            {
                case GameState.GameScreen.Menu: DrawMenu(); break;
                case GameState.GameScreen.Game: DrawGame(); break;
                case GameState.GameScreen.GamePopup: DrawGameWithPopup(); break;
                case GameState.GameScreen.EndGame: DrawGameEnd(); break;
            }
            base.Draw(gameTime);
        }

        private bool _write = true;

        private void DrawMenu()
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            int y = GraphicsDevice.Viewport.Height;
            int x = GraphicsDevice.Viewport.Width;

            int offsetX = (x - 200) / 2;
            int offsetY = (y - 380) / 2;

            if (_write)
            {
                _write = false;
            }

            _spriteBatch.DrawString(_titleFont, Title, new Vector2(220, 80), Color.Blue);

            _spriteBatch.DrawString(_gameState.SelectedOption == GameState.MenuOption.Start ? _bigMenuFont : _smallMenuFont, GameState.MenuOption.Start.ToString(), new Vector2(offsetX, offsetY + 100),
                _gameState.SelectedOption == GameState.MenuOption.Start ? Color.Red : Color.DarkBlue);

            _spriteBatch.DrawString(_gameState.SelectedOption == GameState.MenuOption.Options ? _bigMenuFont : _smallMenuFont, GameState.MenuOption.Options.ToString(), new Vector2(offsetX, offsetY + 200),
                _gameState.SelectedOption == GameState.MenuOption.Options ? Color.Red : Color.DarkBlue);

            _spriteBatch.DrawString(_gameState.SelectedOption == GameState.MenuOption.Exit ? _bigMenuFont : _smallMenuFont, GameState.MenuOption.Exit.ToString(), new Vector2(offsetX, offsetY + 300),
                _gameState.SelectedOption == GameState.MenuOption.Exit ? Color.Red : Color.DarkBlue);

            _spriteBatch.End();
        }

        private void DrawGame()
        {
            GraphicsDevice.Clear(_gameBackgroundColor);

            _spriteBatch.Begin();

            DrawMemory(_spriteBatch);

            _spriteBatch.End();
        }

        private void DrawGameWithPopup()
        {
            GraphicsDevice.Clear(_gameBackgroundColor);

            _spriteBatch.Begin();

            DrawMemory(_spriteBatch);

            DisplayQuestion("Question", "Do they equal?", _spriteBatch);

            _spriteBatch.End();
        }

        private void DrawGameEnd()
        {
            GraphicsDevice.Clear(_gameBackgroundColor);

            _spriteBatch.Begin();

            DrawMemory(_spriteBatch);

            string title = "";
            string message = "";
            if (_gameState.HasCurrentPlayerMorePoints())
            {
                title = "Game won!";
                message = "Congrats! You won :)";
            }
            else if(_gameState.Player1.Points == _gameState.Player2.Points)
            {
                title = "Game over";
                message = "It's a draw!";
            }
            else
            {
                title = "Game over";
                message = "Sorry, you lose :(";
            }

            DisplayGameOverScreen(title, message, _spriteBatch);

            _spriteBatch.End();
        }

        private void DrawMemory(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < _gameState.MemoryMatrixSizeHeight; i++)
            {
                for (var j = 0; j < _gameState.MemoryMatrixSizeWidth; j++)
                {
                    _gameState.Cards[i][j].DrawCard(spriteBatch);
                }
            }

            var x = GraphicsDevice.Viewport.Width;

            var nameSize = _smallMenuFont.MeasureString(_gameState.Player1.Name);
            DrawPlayerName(spriteBatch, _gameState.Player1, 30, 15, 35, _smallMenuFont);
            DrawPlayerName(spriteBatch, _gameState.Player2, 30 + (int)nameSize.X + 50, 15, 35, _smallMenuFont);


            var esc = "ESC - exit game";
            var back = "BACK - return to menu";
            var backSize = _smallMenuFont.MeasureString(esc);
            
            spriteBatch.DrawString(_smallMenuFont, esc, new Vector2(x - backSize.X - 100, 15), Color.Black);
            spriteBatch.DrawString(_smallMenuFont, back, new Vector2(x - backSize.X - 100, 35), Color.Black);
        }

        private void DrawPlayerName(SpriteBatch spriteBatch, Player player, int x, int nameOffset, int pointsOffset, SpriteFont font)
        {
            if(player.CurrentMove)
            {
                var playerNameSize = _smallMenuFont.MeasureString(player.Name);
                var playerPointsSize = _smallMenuFont.MeasureString(player.Points + "");

                var width = (int)(playerNameSize.X > playerPointsSize.X ? playerNameSize.X : playerPointsSize.X) + 10;
                var height = (int)(playerNameSize.Y + playerPointsSize.Y);

                var currentPlayerBox = CreateBox(width, height, x - 5, nameOffset - 5);

                var fillText = new Texture2D(GraphicsDevice, 1, 1);
                fillText.SetData(new[] { _gameBackgroundColor });
                DrawBorder(spriteBatch, currentPlayerBox, Color.MediumVioletRed, 2);
            }

            spriteBatch.DrawString(font, player.Name, new Vector2(x, nameOffset), player.Color);
            spriteBatch.DrawString(font, player.Points + "", new Vector2(x, pointsOffset), player.Color);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle box, Color borderColor, int thickness)
        {
            spriteBatch.Draw(_pixel, new Rectangle(box.X, box.Y, box.Width, thickness), borderColor); //top
            spriteBatch.Draw(_pixel, new Rectangle((box.X + box.Width - thickness), box.Y, //right
                                            thickness, box.Height), borderColor);

            spriteBatch.Draw(_pixel, new Rectangle(box.X, box.Y + box.Height - thickness, //bottom
                                            box.Width, thickness), borderColor);
            spriteBatch.Draw(_pixel, new Rectangle(box.X, box.Y, thickness, box.Height), borderColor); //left
        }

        public void DisplayQuestion(string title, string text, SpriteBatch batch)
        {
            var fillText = new Texture2D(GraphicsDevice, 1, 1);
            fillText.SetData(new[] { Color.White });
            //Draw rectangle, center screen,
            const int mainBoxWidth = 270;
            Rectangle mainBox = CreateBox(mainBoxWidth, 250, Window.ClientBounds.Width - 50 - mainBoxWidth,
                                _gameState.Cards[0][0].TopLeftY);

            //Draw Title
            const int titleBoxWidth = 220;
            var padding = mainBox.Width / 2 - titleBoxWidth / 2;
            Rectangle titleBox = CreateBox(titleBoxWidth, (int)_smallMenuFont.MeasureString(title).Y, padding + mainBox.X,
                                padding + mainBox.Y);

            //Draw Line Between Title and TextBox
            Rectangle textSeperator = CreateBox(mainBox.Width - padding * 2, 1, mainBox.X + padding,
                                titleBox.Y + (int)(padding * 1.2));
            
            //Draw TextBody
            Rectangle textBody = CreateBox(mainBox.Width - (padding * 2), mainBox.Height - (padding * 3) - titleBox.Height,
                titleBox.X, titleBox.Y + titleBox.Height + padding);

            //Draw MainBox
            batch.Draw(fillText, mainBox, Color.Gray);
            
            //Draw TitleBox
            batch.DrawString(_smallMenuFont, title, new Vector2(titleBox.X, titleBox.Y), Color.Blue);

            //Draw Line Between Title And TextBody
            batch.Draw(fillText, textSeperator, Color.Gray);

            //Draw TextBody
            int lineNumber = 0;
            foreach (string line in WrapText(text, textBody.Width))
            {
                batch.DrawString(_smallMenuFont, line, new Vector2(textBody.X, textBody.Y + (lineNumber * _smallMenuFont.MeasureString(line).Y)), Color.Black);
                lineNumber++;
            }

            const int buttonWidth = 55;
            _gameState.YesButton = DrawButton(buttonWidth, textBody.X, textBody.Y + 50, "Yes", batch);
            _gameState.NoButton = DrawButton(buttonWidth, textBody.X + 100, textBody.Y + 50, "No", batch);

            if(!string.IsNullOrEmpty(_gameState.Message.Message))
            {
                var msgSize = _smallMenuFont.MeasureString(_gameState.Message.Message);
                batch.DrawString(_smallMenuFont, _gameState.Message.Message, new Vector2(textBody.X, _gameState.YesButton.Y + msgSize.Y + 30), _gameState.Message.MessageColor);
            }
        }

        public void DisplayGameOverScreen(string title, string text, SpriteBatch batch)
        {
            var fillText = new Texture2D(GraphicsDevice, 1, 1);
            fillText.SetData(new[] { Color.White });
            //Draw rectangle, center screen,
            const int mainBoxWidth = 290;
            const int mainBoxHeight = 220;
            Rectangle mainBox = CreateBox(mainBoxWidth, mainBoxHeight, Window.ClientBounds.Width / 2 - mainBoxHeight / 2,
                                Window.ClientBounds.Height / 2 - mainBoxWidth / 2);

            //Draw Title
            const int titleBoxWidth = 220;
            int padding = 20;
            Rectangle titleBox = CreateBox(titleBoxWidth, (int)_smallMenuFont.MeasureString(title).Y, padding + mainBox.X,
                                padding + mainBox.Y);

            //Draw Line Between Title and TextBox
            Rectangle textSeperator = CreateBox(mainBox.Width - padding * 2, 1, mainBox.X + padding,
                                titleBox.Y + (int)(padding * 1.2));

            //Draw TextBody
            Rectangle textBody = CreateBox(mainBox.Width - (padding * 2), mainBox.Height - (padding * 3) - titleBox.Height,
                titleBox.X, titleBox.Y + titleBox.Height + padding);

            //Draw MainBox
            batch.Draw(fillText, mainBox, Color.Gray);

            //Draw TitleBox
            batch.DrawString(_smallMenuFont, title, new Vector2(titleBox.X, titleBox.Y), Color.Blue);

            //Draw Line Between Title And TextBody
            batch.Draw(fillText, textSeperator, Color.Gray);

            //Draw TextBody
            int lineNumber = 0;
            foreach (string line in WrapText(text, textBody.Width))
            {
                batch.DrawString(_smallMenuFont, line, new Vector2(textBody.X, textBody.Y + (lineNumber * _smallMenuFont.MeasureString(line).Y)), Color.Black);
                lineNumber++;
            }

            _gameState.AgainButton = DrawButton(75, textBody.X, textBody.Y + 60, "Again", batch);
            _gameState.EnoughButton = DrawButton(90, textBody.X + 100, textBody.Y + 60, "Enough", batch);
        }

        private Rectangle CreateBox(int width, int height, int x, int y)
        {
            Rectangle box;
            box.Width = width;
            box.Height = height;
            box.X = x;
            box.Y = y;
            return box;
        }

        private Rectangle DrawButton(int width, int x, int y, string text, SpriteBatch batch)
        {
            var fillText = new Texture2D(GraphicsDevice, 1, 1);
            fillText.SetData(new Color[] { Color.White });
            const int buttonHeight = 50;
            Rectangle button = CreateBox(width, buttonHeight, x, y);
            batch.Draw(fillText, button, Color.Red);
            Vector2 textSize = _smallMenuFont.MeasureString(text);
            batch.DrawString(_smallMenuFont, text, new Vector2(button.X + 5, button.Y + (buttonHeight - textSize.Y) / 2 + 5), Color.Black);
            return button;
        }

        private IEnumerable<object> WrapText(string text, float length)
        {
            string[] words = text.Split(' ');
            var lines = new ArrayList();
            float linewidth = 0f;
            float spaceWidth = _smallMenuFont.MeasureString(" ").X;
            int curLine = 0;
            lines.Add(string.Empty);
            foreach (string word in words)
            {
                Vector2 size = _smallMenuFont.MeasureString(word);
                if (linewidth + size.X < length)
                {
                    lines[curLine] += word + " ";
                    linewidth += size.X + spaceWidth;
                }
                else
                {
                    lines.Add(word + " ");
                    linewidth = size.X + spaceWidth;
                    curLine++;
                }
            }
            return lines.ToArray();
        }

    }
}
