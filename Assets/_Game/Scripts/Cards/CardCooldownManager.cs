/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    [Serializable]
    public class CardCooldownManager : MonoBehaviour
    {
        // CardCooldownManager tracks the ability cooldown of cards that were signed up and triggers their end of cooldown upon completion
        public static CardCooldownManager Instance {get; private set;}
        #region Properties
        [SerializeField] private Dictionary<CardInstance, int> _cardCooldowns = new Dictionary<CardInstance, int>();
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            if(_cardCooldowns.Count > 0)
            {
                StopAllCoroutines();
                foreach(KeyValuePair<CardInstance, int> pair in _cardCooldowns)
                    StartCoroutine(CooldownCoroutine(pair.Key));
            }
        }
        #endregion
        
        #region Methods
        ///<summary>
        ///Starts cooldown tracking for the given card.
        ///</summary>
        ///<param name="card">CardInstance to sign up for cooldown tracking.</param>
        ///<param name="remainingCooldown">Cooldown for the given CardInstance as int.</param>
        public void StartCooldownForCard(CardInstance card, int remainingCooldown)
        {
            _cardCooldowns.Add(card, remainingCooldown);
            StartCoroutine(CooldownCoroutine(card));
        }

        ///<summary>
        ///Removed card from cooldown tracking without triggering end of cooldown.
        ///</summary>
        ///<param name="card">CardInstance to remove from cooldown tracking.</param>
        public void RemoveCardFromTracking(CardInstance card)
        {
            _cardCooldowns.Remove(card);
        }

        ///<summary>
        ///Reduces cooldown for the given card by the given amount if the card is being tracked.
        ///Non-flat Formula: CD=Floor(CD - (CD * reduction))
        ///</summary>
        ///<param name="card">CardInstance to reduce cooldown for.</param>
        ///<param name="reduction">Amount to reduce the cooldown by as float.</param>
        ///<param name="isFlat">Bool value: true if cooldown is reduced by a flat amount, false if the reduction is multiplicative.</param>
        public void ReduceCooldownForCard(CardInstance card, float reduction, bool isFlat)
        {
            if(!_cardCooldowns.ContainsKey(card)) return;
            int cd = _cardCooldowns[card];
            if(!isFlat) cd = Mathf.FloorToInt(cd - (cd * reduction));
            else cd -= Mathf.RoundToInt(reduction);
            _cardCooldowns[card] = cd;
        }

        ///<summary>
        ///Returns the remaining cooldown for a given card if available.
        ///</summary>
        ///<param name="card">CardInstance to obtain cooldown for.</param>
        ///<returns>Returns the remaining cooldown if available, otherwise returns null.</returns>
        public float? GetRemainingCooldownForCard(CardInstance card)
        {
            if(!_cardCooldowns.ContainsKey(card)) return null;

            return _cardCooldowns[card];
        }
        #endregion

        ///<summary>
        ///Reduces the cooldown of the card by 1 seconds every in-game second.
        ///When hitting 0 triggers the end of cooldown for the card and removes it from tracking.
        ///[WaitForSeconds]
        ///</summary>
        ///<param name="card">CardInstance for which the cooldown is being tracked.</param>
        private IEnumerator CooldownCoroutine(CardInstance card)
        {
            for(;;)
            {
                yield return new WaitForSeconds(1f);
                if(!_cardCooldowns.ContainsKey(card)) break;
                _cardCooldowns[card]--;
                if(_cardCooldowns[card] < 1) break; 
            }
            if(_cardCooldowns.ContainsKey(card))
            {
                GameManager.Instance.TriggerCooldownEndForCard(card);
                card.OffCooldown();
                _cardCooldowns.Remove(card);
            }
        }
    }
}
