/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerCardInventory
    {
        #region Properties
        [SerializeField] private List<CardInstance> _cardInventory = new List<CardInstance>();
        public List<CardInstance> CardInventory => _cardInventory;
        #endregion

        #region Methods
        public void AddCard(CardInstance card)
        {
            _cardInventory.Add(card);
        }

        public void RemoveCard(CardInstance card)
        {
            _cardInventory.Remove(card);
        }
        #endregion
    }
}
