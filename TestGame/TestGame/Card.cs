using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    class Card
    {
        private readonly string _value;
        private Point _topLeftCorner;
        private readonly Texture2D _coverImage;
        private readonly Texture2D _faceImage;
        private readonly Texture2D _frame;//white by default
        private readonly int _id;
        private readonly int _pairCardId;
        private Rectangle _cardBox;
        private CardAnimation _currentAnimation;
        public CardAnimation CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }

        public Card(int x, int y, Texture2D cover, Texture2D face, Texture2D frame, CardAnimation animation, int id, int pairCardId, string value)
        {
            _topLeftCorner.X = x;
            _topLeftCorner.Y = y;
            _coverImage = cover;
            _faceImage = face;
            _currentAnimation = animation;
            _id = id;
            _pairCardId = pairCardId;
            _frame = frame;
            _value = value;
        }

        public int TopLeftY { get { return _topLeftCorner.Y; } }
        public int TopLeftX { get { return _topLeftCorner.X; } }
        public Texture2D Cover { get { return _coverImage; } }
        public Texture2D Face { get { return _faceImage; } }
        public Texture2D Frame { get { return _frame; } }
        public string Value { get { return _value; } }

        public Card Copy()
        {
            return new Card(_topLeftCorner.X, _topLeftCorner.Y, _coverImage, _faceImage, _frame,
                            Card.CardAnimation.Cover, _id, _pairCardId, _value);
        }
        
        public void DrawCard(SpriteBatch spriteBatch)
        {
            _cardBox = new Rectangle(_topLeftCorner.X - 2, _topLeftCorner.Y - 2, _frame.Width, _frame.Height);
            spriteBatch.Draw(_frame, _cardBox, Color.White);
            switch (_currentAnimation)
            {
                case CardAnimation.Cover: 
                    spriteBatch.Draw(_coverImage, new Rectangle(_topLeftCorner.X, _topLeftCorner.Y, _coverImage.Width, _coverImage.Height), Color.White);
                    break;
                case CardAnimation.Face: 
                    spriteBatch.Draw(_faceImage, new Rectangle(_topLeftCorner.X, _topLeftCorner.Y, _faceImage.Width, _faceImage.Height), Color.White);
                    break;
            }
            
        }

        public bool Disabled { get; set; }
        public int ID { get { return _id; } }
        public int PairCardID { get { return _pairCardId; } }

        public void DisableCard(Color color)
        {
            Disabled = true;
            RedrawFrame(color);
        }

        private void RedrawFrame(Color color)
        {
            var data = new Color[_frame.Height * _frame.Width];
            for (var i = 0; i < data.Length; ++i) data[i] = color;
            _frame.SetData(data);
        }

        public enum CardAnimation
        {
            Cover, Face
        };

        public void Reset()
        {
            Disabled = false;
            RedrawFrame(Color.White);
            _currentAnimation = CardAnimation.Cover;
        }

        public bool IsWithinCard(int x, int y)
        {
            return _cardBox.Contains(x, y);
        }
    }
}
