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
    public class PlayerCardInventory : ICardList
    {
        #region Properties
        [SerializeField] private List<CardInstance> _cardInventory = new List<CardInstance>();
        public List<CardInstance> CardInventory => _cardInventory;
        private int _totalStrength = 0;
        public int TotalStrength => _totalStrength;
        private Dictionary<Card.CardType, int> _ownedTypes = new Dictionary<Card.CardType, int>();
        private float _typeMultiplier = 0f;
        public float TypeMultiplier => _typeMultiplier;
        #endregion

        #region Methods
        public PlayerCardInventory()
        {
            this._totalStrength = 0;
            this._typeMultiplier = 0;
        }
        public List<CardInstance> GetCardList() => _cardInventory;
        public List<CardInstance> GetCardListOfType(Card.CardType type) => _cardInventory.FindAll((CardInstance card) => card.CardRef.Type == type);
        public void AddCard(CardInstance card)
        {
            Card cardRef = card.CardRef;
            _cardInventory.Add(card);
            _totalStrength += cardRef.Strength;
            UpdateCardType(cardRef.Type, cardRef.Strength);
            UpdateTypeMultiplier();
        }

        public void RemoveCard(CardInstance card)
        {
            Card cardRef = card.CardRef;
            _cardInventory.Remove(card);
            _totalStrength -= cardRef.Strength;
            UpdateCardType(cardRef.Type, cardRef.Strength,false);
        }

        private void UpdateCardType(Card.CardType type, int strength, bool isAdding = true)
        {
            if(_ownedTypes.Count == 0)
            {
                if(!isAdding) return;
                _ownedTypes.Add(type, strength);
                return;
            }

            Dictionary<Card.CardType, int> copy = _ownedTypes;

            foreach( KeyValuePair<Card.CardType, int> pair in copy)
            {
                if(pair.Key == type)
                {
                    if(isAdding)
                    {
                        _ownedTypes[pair.Key] = pair.Value + strength;
                        return;
                    }

                    if(pair.Value - strength > 0)
                    {
                        _ownedTypes[pair.Key] = pair.Value - strength;
                        return;
                    }

                    _ownedTypes.Remove(pair.Key);
                    return;
                }
            }
        }

        private void UpdateTypeMultiplier()
        {
            GameSettingsScriptableObject settings = GameManager.Instance.GameSettingsRef;
            List<float> multipliers = settings.CardTypeMultipliers;
            List<int> upperBounds = settings.CardTypeMultiplierUpperBounds;

            float output = 1;

            foreach(KeyValuePair<Card.CardType, int> pair in _ownedTypes)
            {
                for(int i = 0; i < multipliers.Count; i++)
                {
                    if(i >= upperBounds.Count)
                    {
                        output += multipliers[multipliers.Count - 1];
                        break;
                    }
                    if(pair.Value < upperBounds[i])
                    {
                        output += multipliers[i];
                        break;
                    }
                }
            }

            _typeMultiplier = output;
        }

        public bool CheckIfCardIsDuplicate(CardInstance card)
        {
            for(int i = 0; i < _cardInventory.Count; i++)
            {
                if(_cardInventory[i].CardArt == card.CardArt) return true;
            }
            return false;
        }
        #endregion
    }
}
