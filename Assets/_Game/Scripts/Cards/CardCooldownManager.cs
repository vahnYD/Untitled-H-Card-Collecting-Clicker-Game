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
        public void StartCooldownForCard(CardInstance card, int remainingCooldown)
        {
            _cardCooldowns.Add(card, remainingCooldown);
        }

        public void RemoveCardFromTracking(CardInstance card)
        {
            _cardCooldowns.Remove(card);
        }

        public void ReduceCooldownForCard(CardInstance card, float reduction, bool isFlat)
        {
            if(!_cardCooldowns.ContainsKey(card)) return;
            int cd = _cardCooldowns[card];
            if(!isFlat) cd = Mathf.FloorToInt(cd - (cd * reduction));
            else cd -= Mathf.RoundToInt(reduction);
            _cardCooldowns[card] = cd;
        }
        #endregion

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
