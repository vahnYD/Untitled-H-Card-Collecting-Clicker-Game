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
        ///<summary>
        ///Returns true if the list is empty, else returns false.
        ///</summary>
        bool isEmpty();

        ///<summary>
        ///Adds a singular Card to the card list.
        ///</summary>
        ///<param name="card">CardInstance to add to the card list.</param>
        void AddCard(CardInstance card);

        ///<summary>
        ///Removes a singular Card from the card list.
        ///</summary>
        ///<param name="card">CardInstance to remove from the card list.</param>
        void RemoveCard(CardInstance card);

        List<CardInstance> GetCardList();

        ///<summary>
        ///Gets Cards from the card list that have the name given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="name">Name to get cards for as string.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        List<CardInstance> GetCardsByName(string name);

        ///<summary>
        ///Gets Cards from the card list that are of the type given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="type">Card.CardType enum value for the type to grab cards of.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        List<CardInstance> GetCardsByType(Card.CardType type);

        ///<summary>
        ///Gets Cards from the card list that are of the rarity given as parameter.
        ///Doesn't remove.
        ///</summary>
        ///<param name="rarity">Card.CardRarity enum value for the rarity to grab cards of.</param>
        ///<returns>Returns a List of CardInstance Objects.</returns>
        List<CardInstance> GetCardsByRarity(Card.CardRarity rarity);
    }
}
