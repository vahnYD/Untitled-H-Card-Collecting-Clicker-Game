/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    [Serializable]
    public class Deck
    {
        #region Properties
        [SerializeField] private List<CardInstance> _decklist = new List<CardInstance>();
        public List<CardInstance> DeckList => _decklist;
        #endregion

        #region Methods
        public void AddCard(CardInstance card)
        {
            _decklist.Add(card);
        }

        public void RemoveCard(CardInstance card)
        {
            _decklist.Remove(card);;
        }

        public CardInstance Draw()
        {
            CardInstance card = _decklist[0];
            _decklist.RemoveAt(0);
            return card;
        }

        public void Shuffle()
        {
            System.Random random = new System.Random();
            int n = _decklist.Count;
            while(n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                CardInstance card = _decklist[k];
                _decklist[k] = _decklist[n];
                _decklist[n] = card;
            }
        }
        #endregion
    }
}
