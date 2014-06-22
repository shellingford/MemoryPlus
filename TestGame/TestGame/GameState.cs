using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    class GameState
    {
        #region singleton pattern
        private static readonly GameState instance = new GameState();

         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit
        static GameState()
         {
         }

        private GameState()
        {
            Message = new QuestionMessage();
        }

        public static GameState Instance
         {
             get
             {
                 return instance;
             }
         }
        #endregion
        
        public enum GameScreen
        {
            Menu, Game, GamePopup, EndGame
        };

        public enum MenuOption
        {
            Start, Options, Exit
        }

        private const int CardOffX = 80;
        private const int CardOffY = 150;

        private const int AnsweredCorrectlyPoints = 3;
        private const int FoundPairPoints = 1;

        public MenuOption SelectedOption { get; set; }

        private int _memoryMatrixSizeHeight = 4;
        private int _memoryMatrixSizeWidth = 4;
        public int MemoryMatrixSizeHeight
        {
            get { return _memoryMatrixSizeHeight; }
            set { _memoryMatrixSizeHeight = value; }
        }
        public int MemoryMatrixSizeWidth
        {
            get { return _memoryMatrixSizeWidth; }
            set { _memoryMatrixSizeWidth = value; }
        }

        public Card[][] Cards { get; set; }

        private GameState.GameScreen _currentGameScreen = GameState.GameScreen.Menu;
        public GameState.GameScreen CurrentGameScreen
        {
            get { return _currentGameScreen; }
            set { _currentGameScreen = value; }
        }

        public Card OpenCard1 { get; set; }
        public Card OpenCard2 { get; set; }

        public Rectangle YesButton { get; set; }
        public Rectangle NoButton { get; set; }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public QuestionMessage Message { get; set; }

        private int _foundPairCards = 0;

        public Card CloseCard(Card card)
        {
            if (!card.Disabled)
            {
                card.CurrentAnimation = Card.CardAnimation.Cover;
            }
            else
            {
                _foundPairCards++;
            }
            return null;
        }

        public void CloseCards()
        {
            if(OpenCard1 != null && OpenCard2 != null)
            {
                OpenCard1 = CloseCard(OpenCard1);
                OpenCard2 = CloseCard(OpenCard2);
            }
        }

        public void PlayerOnlyFoundPair()
        {
            AwardPlayerWithPoints(FoundPairPoints);
            if(GetCurrentPlayerColor() == Color.Blue)
            {
                DisableCards(Color.Teal);
            }
            else if (GetCurrentPlayerColor() == Color.Red)
            {
                DisableCards(Color.Orange);
            }
            Message.AnsweredIncorrectly();
        }

        public void PlayerAnsweredCorrectly()
        {
            AwardPlayerWithPoints(AnsweredCorrectlyPoints);
            DisableCards(GetCurrentPlayerColor());
            Message.AnsweredCorrectly();
        }

        public void PlayerAnsweredCorrectlyWithoutPair()
        {
            Message.AnsweredCorrectly();
        }

        public void PlayerAnsweredIncorrectly()
        {
            Message.AnsweredIncorrectly();
        }

        private void DisableCards(Color color)
        {
            OpenCard1.DisableCard(color);
            OpenCard2.DisableCard(color);
        }

        private void AwardPlayerWithPoints(int points)
        {
            if(Player1.CurrentMove)
            {
                Player1.Points += points;
            }
            if (Player2.CurrentMove)
            {
                Player2.Points += points;
            }
        }

        public Color GetCurrentPlayerColor()
        {
            if (Player1.CurrentMove)
            {
                return Player1.Color;
            }
            if (Player2.CurrentMove)
            {
                return Player2.Color;
            }

            throw new InvalidOperationException("None of the players currently playing");
        }

        public void ResetFromPopupToGame()
        {
            CurrentGameScreen = GameState.GameScreen.Game;
            Message.Reset();
        }

        public void ResetGame()
        {
            //reset player points
            Player1.Points = 0;
            Player2.Points = 0;

            Card[][] copyCards = CopyCards();

            //reset cards
            var randomCardManager = new RandomCardManager(_memoryMatrixSizeHeight, _memoryMatrixSizeWidth);
            for(int i = 0; i < _memoryMatrixSizeHeight; i++)
            {
                for(int j = 0; j < _memoryMatrixSizeWidth; j++)
                {
                    copyCards[i][j].Reset();
                    PlaceRandomCard(copyCards[i][j].ID, copyCards[i][j].PairCardID, randomCardManager, copyCards[i][j].Cover, copyCards[i][j].Face, copyCards[i][j].Frame, copyCards[i][j].Value);
                }
            }

            //reset open cards
            OpenCard1 = null;
            OpenCard2 = null;

            //reset question message
            Message.Message = "";
        }

        private Card[][] CopyCards()
        {
            var copy = new Card[Cards.Length][];

            for (int i = 0; i < _memoryMatrixSizeHeight; i++)
            {
                copy[i] = new Card[_memoryMatrixSizeWidth];
                for (int j = 0; j < _memoryMatrixSizeWidth; j++)
                {
                    copy[i][j] = Cards[i][j].Copy();
                }
            }

            return copy;
        }
        
        public void PlaceRandomCard(int id, int pairId, RandomCardManager randomCardManager, Texture2D coverImage, Texture2D card, Texture2D rect, string value)
        {
            RandomCardManager.RandomCardPlace randomCardPlace = randomCardManager.GetNext();
            int offsetY = randomCardPlace.J * (coverImage.Height + 10) + CardOffY;
            int offsetX = randomCardPlace.I * (coverImage.Width + 23) + CardOffX;

            Cards[randomCardPlace.I][randomCardPlace.J] =
                new Card(offsetX, offsetY, coverImage, card, rect, Card.CardAnimation.Cover, id, pairId, value);
        }

        public bool HasCurrentPlayerMorePoints()
        {
            if (Player1.CurrentMove)
            {
                return Player1.Points > Player2.Points;
            }
            if (Player2.CurrentMove)
            {
                return Player2.Points > Player1.Points;
            }

            throw new InvalidOperationException("None of the players currently playing");
        }

        public bool GameHasEnded()
        {
            return _foundPairCards == _memoryMatrixSizeHeight*_memoryMatrixSizeWidth;
        }

        public Rectangle AgainButton { get; set; }

        public Rectangle EnoughButton { get; set; }
    }
}
