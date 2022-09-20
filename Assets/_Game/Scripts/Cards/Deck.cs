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
        #region Properties
        [SerializeField] private List<CardInstance> _decklist = new List<CardInstance>();
        public List<CardInstance> DeckList => _decklist;
        public event Action<int> DeckSizeChangedEvent;
        #endregion

        #region Methods
        public List<CardInstance> GetCardList() => _decklist;
        public void AddCard(CardInstance card)
        {
            _decklist.Add(card);
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        public void AddMultipleCards(CardInstance[] cards)
        {
            if(cards.Length is 0) return;
            for(int i = 0; i < cards.Length; i++)
            {
                if(cards[i] != null)_decklist.Add(cards[i]);
            }
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        public void RemoveCard(CardInstance card)
        {
            _decklist.Remove(card);;
            DeckSizeChangedEvent.Invoke(_decklist.Count);
        }

        public void RemoveMultipleCards(CardInstance[] cards)
        {
            if(cards.Length is 0) return;
            foreach(CardInstance card in cards)
            {
                _decklist.Remove(card);
            }
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
        }

        public List<CardInstance> GetCardsByName(string name) => _decklist.FindAll((CardInstance card) => card.Name == name);
        public List<CardInstance> GetCardsByType(Card.CardType type) => _decklist.FindAll((CardInstance card) => card.CardRef.Type == type);
        public List<CardInstance> GetCardsByRarity(Card.CardRarity rarity) => _decklist.FindAll((CardInstance card) => card.CardRef.Rarity == rarity);

        public CardInstance Draw()
        {
            if(_decklist.Count is 0) return null;
            CardInstance card = _decklist[0];
            _decklist.RemoveAt(0);
            DeckSizeChangedEvent?.Invoke(_decklist.Count);
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

        public void OverwriteDecklist(List<CardInstance> cards)
        {
            _decklist = cards;
        }
        #endregion
    }
}
