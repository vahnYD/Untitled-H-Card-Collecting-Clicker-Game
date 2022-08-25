/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using UnityEngine;

namespace Cards
{
    [Serializable]
    public class CardInstance
    {
        private Card _card;
        private string _name;
        private bool _hasAbility;
        private bool _onCooldown;
        private int _level;
        private int _nextUpgradeCost;
        private string _abilityText;
        
        public CardInstance(Card card)
        {
            _card = card;
            _name = card.Name;
            _hasAbility = card.HasAbility;
            _nextUpgradeCost = card.Ability.BaseUpgradeCost;
            _level = 1;
        }

        public void ActivateAbility()
        {
            if(!_hasAbility) return;
            _card.Ability.ActivateAbility(_level);
        }

        public void Upgrade()
        {
            if(_nextUpgradeCost > GameManager.Instance.GetCurrentCoinTotal()) return;
            _level++;
            UpdateAbilityText();
            GameManager.Instance.RemoveCoins(_nextUpgradeCost);
            _nextUpgradeCost = _card.Ability.GetUpgradeCostForLevel(_level+1);
        }

        private void UpdateAbilityText()
        {
            _abilityText = _card.Ability.GetUpdatedAbilityTextForLevel(_level);
        }
    }
}