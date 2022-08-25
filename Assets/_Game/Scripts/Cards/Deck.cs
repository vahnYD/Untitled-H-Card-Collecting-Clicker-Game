/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class Deck
    {
        #region Properties
        [SerializeField] private List<CardInstance> _decklist = new List<CardInstance>();
        public List<CardInstance> DeckList => _decklist;
        #endregion

        #region Methods
        private void AddCard(CardInstance card)
        {
            _decklist.Add(card);
        }

        private void RemoveCard(CardInstance card)
        {
            _decklist.Remove(card);
        }
        #endregion
    }
}
