/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    [Serializable]
    public class CardInstance
    {
        private Card _card;
        public Card CardRef => _card;
        private string _name;
        public string Name => _name;
        private Sprite _cardArt;
        public Sprite CardArt => _cardArt;
        private bool _hasAbility;
        public bool HasAbility;
        public Abilities.Ability CardAbility => _card.Ability;
        private bool _onCooldown;
        public bool OnCooldown => _onCooldown;
        private int _remainingCooldown;
        public int RemainingCooldown => _remainingCooldown;
        private int _level;
        public int Level => _level;
        private int _nextUpgradeCost;
        public int NextUpgradeCost => _nextUpgradeCost;
        private string _abilityText;
        public string AbilityText => _abilityText;
        
        public CardInstance(Card card)
        {
            _card = card;
            _name = card.Name;
            _cardArt = card.GetRandomCardArt();
            _hasAbility = card.HasAbility;
            _nextUpgradeCost = card.Ability.BaseUpgradeCost;
            _level = (_card.Ability.MaxLevel < 2) ? 0 : 1;
        }

        public void ActivateAbility()
        {
            if(!_hasAbility) return;
            _card.Ability.ActivateAbility(_level);
        }

        public bool AttemptUpgrade()
        {
            if(_nextUpgradeCost > GameManager.Instance.CoinTotal) return false;
            if(_level == 0) return false;
            if(_level == _card.Ability.MaxLevel) return false;
            Upgrade();
            return true;
        }

        private void Upgrade()
        {
            _level++;
            UpdateAbilityText();
            GameManager.Instance.RemoveCoins(_nextUpgradeCost);
            _nextUpgradeCost = _card.Ability.GetUpgradeCostForLevel(_level+1);
        }

        private void UpdateAbilityText()
        {
            _abilityText = _card.Ability.GetUpdatedAbilityText(_level);
        }
    }
}