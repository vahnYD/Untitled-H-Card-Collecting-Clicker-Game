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
    public class Deck : ICardList
    {
        //! Cards randomly get removed from hand despite remove/removeMultiple/draw not getting triggered and the list being private
        #region Properties
        [SerializeField] private List<CardInstance> _decklist;
        public List<CardInstance> DeckList
        {
            get{return _decklist;}
            private set{}
        }

        public Deck()
        {
            _decklist = new List<CardInstance>();
        }

        ///<summary>
        ///Gets Invoked whenever the amount of cards in the list goes up or down.
        ///</summary>
        public event Action<int> DeckSizeChangedEvent;
        #endregion

        #region Methods
        ///<summary>
        ///Returns true if the list is empty, else returns false.
        ///</summary>
        public bool isEmpty()
        {
            if(_decklist.Count is 0) return true;
            return false;
        }

        public List<CardInstance> GetCardList() => _decklist;

        ///<summary>
        ///Adds a singular Card to the card list.
        ///Invokes the DeckSizeChangedEvent.
        ///</summary>
        ///<param name="card">CardInstance to add to the card list.</param>
        public void AddCard(CardInstance card)
        {
            _decklist.Add(card);
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        ///<summary>
        ///Adds multiple Cards to the card list.
        ///Invokes the DeckSizeChangedEvent.
        ///</summary>
        ///<param name="cards">Array of CardInstance Objects to add to the card list.</param>
        public void AddMultipleCards(CardInstance[] cards)
        {
            if(cards.Length is 0) return;
            for(int i = 0; i < cards.Length; i++)
            {
                if(cards[i] != null)_decklist.Add(cards[i]);
            }
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        ///<summary>
        ///Removes a singluar Card from the card list.
        ///Invokes the DeckSizeChangedEvent.
        ///</summary>
        ///<param name="card">CardInstance to remove from the card list.</param>
        public void RemoveCard(CardInstance card)
        {
            _decklist.Remove(card);;
            DeckSizeChangedEvent.Invoke(_decklist.Count);
        }

        ///<summary>
        ///Removes multiple Cards from the card list.
        ///Invokes the DeckSizeChangedEvent.
        ///</summary>
        ///<param name="cards">Array of CardInstance Objects to remove from the card list.</param>
        public void RemoveMultipleCards(CardInstance[] cards)
        {
            if(cards.Length is 0) return;
            foreach(CardInstance card in cards)
            {
                _decklist.Remove(card);
            }
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        ///<summary>
        ///Gets Cards from the card list that have the name given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="name">Name to get cards for as string.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByName(string name) => _decklist.FindAll((CardInstance card) => card.Name == name);

        ///<summary>
        ///Gets Cards from the card list that are of the type given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="type">Card.CardType enum value for the type to grab cards of.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByType(Card.CardType type) => _decklist.FindAll((CardInstance card) => card.CardRef.Type == type);

        ///<summary>
        ///Gets Cards from the card list that are of the rarity given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="rarity">Card.CardRarity enum value for the rarity to grab cards of.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        public List<CardInstance> GetCardsByRarity(Card.CardRarity rarity) => _decklist.FindAll((CardInstance card) => card.CardRef.Rarity == rarity);

        ///<summary>
        ///Gets the CardInstance Object at index 0 of the card list, removes it from the card list, and invokes the DeckSizeChangedEvent.
        ///</summary>
        ///</returns>Returns the CardInstance Object.</returns>
        public CardInstance Draw()
        {
            if(_decklist.Count is 0) return null;
            CardInstance card = _decklist[0];
            _decklist.RemoveAt(0);
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
            return card;
        }

        ///<summary>
        ///Shuffles the index positions of the CardInstanc Objects in the card list.
        ///</summary>
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

        ///<summary>
        ///Replaces the current card list with the card list given as parameter.
        ///</summary>
        ///<param name="cards">List of CardInstance Objects.</param>
        public void OverwriteDecklist(List<CardInstance> cards)
        {
            _decklist = cards;
        }
        #endregion
    }
}
