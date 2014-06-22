using Microsoft.Xna.Framework;

namespace TestGame
{
    class QuestionMessage
    {
        private const string CorrectAnswerMessage = "Correct! :)";
        private const string IncorrectAnswerMessage = "Nope, sorry! :(";

        public Color MessageColor { get; set; }
        public string Message { get; set; }

        public QuestionMessage()
        {
            Reset();
        }

        public void AnsweredCorrectly()
        {
            Message = CorrectAnswerMessage;
            MessageColor = Color.DarkGreen;
        }

        public void AnsweredIncorrectly()
        {
            Message = IncorrectAnswerMessage;
            MessageColor = Color.DarkRed;
        }

        internal void Reset()
        {
            Message = "";
        }
    }
}
