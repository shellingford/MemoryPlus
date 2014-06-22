using Microsoft.Xna.Framework;

namespace TestGame
{
    class Player
    {
        public Player(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public string Name { get; set; }

        public Color Color { get; set; }

        public int Points { get; set; }

        public bool CurrentMove { get; set; }
    }
}
