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

        ///<summary>
        ///Returns true if the list is empty, else returns false.
        ///</summary>
        public bool isEmpty()
        {
            if(_cardInventory.Count is 0) return true;
            return false;
        }

        public List<CardInstance> GetCardList() => _cardInventory;

        ///<summary>
        ///Gets Cards from the card list that have the name given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="name">Name to get cards for as string.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByName(string name) => _cardInventory.FindAll(card => card.Name == name);

        ///<summary>
        ///Gets Cards from the card list of the type given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="type">Card.CardType enum value for the type to grab cards of.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByType(Card.CardType type) => _cardInventory.FindAll(card => card.CardRef.Type == type);

        ///<summary>
        ///Gets Cards from the card list of the rarity given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="rarity">Card.CardRarity enum value for the rarity to grab cards of.</param>
        ///</returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByRarity(Card.CardRarity rarity) => _cardInventory.FindAll(card => card.CardRef.Rarity == rarity);

        ///<summary>
        ///Adds a singular Card to the card list and updates the internal total strength value.
        ///Triggers Card Type Multiplier recalculation for lewd point calculations.
        ///</summary>
        ///<param name="card">CardInstance to add to the card list.</param>
        public void AddCard(CardInstance card)
        {
            Card cardRef = card.CardRef;
            _cardInventory.Add(card);
            _totalStrength += cardRef.Strength;
            UpdateCardType(cardRef.Type, cardRef.Strength);
            UpdateTypeMultiplier();
        }

        ///<summary>
        ///Removes a singular Card from the card list and updates the internal total strength value.
        ///Triggers Card Type Multiplier recalculation for lewd point calculations.
        ///</summary>
        ///<param name="card">CardInstance to remove from the card list.</param>
        public void RemoveCard(CardInstance card)
        {
            Card cardRef = card.CardRef;
            _cardInventory.Remove(card);
            _totalStrength -= cardRef.Strength;
            UpdateCardType(cardRef.Type, cardRef.Strength,false);
            UpdateTypeMultiplier();
        }

        ///<summary>
        ///Updates the tracked card types and their strength values.
        ///Adds the type if it isn't already being tracked and the strength value is positive.
        ///Removes the type if it's strength value falls to or below 0.
        ///</summary>
        ///<param name="type">Card.CardType enum value for the type to update.</param>
        ///<param name="strength">Strength by which to change the types strength value if already tracked.</param>
        ///<param name="isAdding">Optional. Bool flag to indicate if the strength change is an increase or a reduction. Defaults to true.</param> 
        private void UpdateCardType(Card.CardType type, int strength, bool isAdding = true)
        {
            if(!_ownedTypes.ContainsKey(type))
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

        ///<summary>
        ///Updates the type multiplier that's needed for lewd point calculations based on the tracked card types and their strength values.
        ///</summary>
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

        ///<summary>
        ///Checks if a given CardInstance is a duplicate of a already present CardInstance in the inventory.
        ///</summary>
        ///<param name="card">CardInstance that's being checked against.</param>
        ///<returns>Returns true if the CardInstance is a duplicate, otherwise returns false.</returns>
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
