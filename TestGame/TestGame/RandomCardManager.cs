using System;
using System.Collections.Generic;
using System.Linq;

namespace TestGame
{
    public class RandomCardManager
    {
        private readonly List<RandomCardPlace> _randomCardPlaces = new List<RandomCardPlace>();

        private int _currentIndex;

        public RandomCardManager(int height, int width)
        {
            for(int k = 0; k < height * width; k++)
            {
                int i = k / width;
                int j = k % width;

                _randomCardPlaces.Add(new RandomCardPlace{I = i, J = j});
            }
            _randomCardPlaces = _randomCardPlaces.OrderBy(a => Guid.NewGuid()).ToList();
        }

        public RandomCardPlace GetNext()
        {
            if(_currentIndex >= _randomCardPlaces.Count)
            {
                throw new ArgumentException();
            }
            return _randomCardPlaces[_currentIndex++];
        }

        public class RandomCardPlace
        {
            public int I { get; set; }
            public int J { get; set; }
        }
    }
}
