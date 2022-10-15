/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    public interface ICardList
    {
        bool isEmpty();
        void AddCard(CardInstance card);
        void RemoveCard(CardInstance card);
        List<CardInstance> GetCardList();
    }
}
