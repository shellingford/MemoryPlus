using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    class CardsCollection
    {
        private const string RootContentDir = "\\Cards\\Math\\Integrals\\";

        public void LoadGroups(ContentManager content, Texture2D coverImage, GraphicsDeviceManager graphics, GameState gameState)
        {
            // load our manifest so we know what files we have
            var dir = new DirectoryInfo(content.RootDirectory + RootContentDir);
            LoadGroupCards(dir, content, coverImage, graphics, gameState);
        }

        public void LoadGroupCards(DirectoryInfo dir, ContentManager content, Texture2D coverImage, GraphicsDeviceManager graphics, GameState gameState)
        {
            var randomCardManager = new RandomCardManager(gameState.MemoryMatrixSizeHeight, gameState.MemoryMatrixSizeWidth);

            int counter = 0;
            foreach(FileInfo fileInfo in dir.GetFiles())
            {
                counter++;
                var cardTexture = content.Load<Texture2D>("Cards/Math/Integrals/" + fileInfo.Name.Replace(".xnb", ""));
                var rect = new Texture2D(graphics.GraphicsDevice, coverImage.Width + 6, coverImage.Height + 4);
                var data = new Color[rect.Height * rect.Width];
                for (int k = 0; k < data.Length; ++k) data[k] = Color.White;
                rect.SetData(data);

                gameState.PlaceRandomCard(counter, counter % 2 == 0 ? (counter - 1) : (counter + 1), randomCardManager, coverImage, cardTexture, rect,
                                fileInfo.Name);
            }
        }
    }
}
