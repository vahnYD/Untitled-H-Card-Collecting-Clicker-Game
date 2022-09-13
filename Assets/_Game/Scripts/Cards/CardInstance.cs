/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    [Serializable]
    public class CardInstance
    {
        [SerializeField] private Card _card;
        public Card CardRef => _card;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private Sprite _cardArt;
        public Sprite CardArt => _cardArt;
        [SerializeField] private bool _hasAbility;
        public bool HasAbility => _hasAbility;
        public List<Abilities.Ability> CardAbilities => _card.Abilities;
        [SerializeField] private bool _onCooldown;
        public bool OnCooldown => _onCooldown;
        [SerializeField] private int _level;
        public int Level => _level;
        [SerializeField] private int _nextUpgradeCost = 0;
        public int NextUpgradeCost => _nextUpgradeCost;
        [SerializeField, TextArea(1, 4)] private string _abilityText;
        public string AbilityText => _abilityText;
        public event Action AbilityUpgradeEvent;
        
        public CardInstance(Card card)
        {
            _card = card;
            _name = card.Name;
            _cardArt = card.GetRandomCardArt();
            _hasAbility = card.HasAbility;
            if(_hasAbility)
                foreach(Abilities.Ability ability in card.Abilities)
                {
                    _nextUpgradeCost += ability.GetUpgradeCostForLevel(2);
                }
            _level = (_hasAbility && _card.Abilities.Where(x=>x.MaxLevel >=2).Count() >= 1) ? 1 : 0;
        }

        public void ActivateAbility()
        {
            if(!_hasAbility) return;
            if(_onCooldown) return;
            foreach(Abilities.Ability ability in _card.Abilities)
                ability.ActivateAbility(_level);
            _onCooldown = true;
            int cooldown = 0;
                foreach(Abilities.Ability ability in _card.Abilities)
                cooldown += ability.CooldownInSec;
            CardCooldownManager.Instance.StartCooldownForCard(this, cooldown);
            GameManager.Instance.MoveSpecificCard(this, GameManager.CardGameStates.Hand, GameManager.CardGameStates.Grave);
        }

        public void OffCooldown() => _onCooldown = false;

        public bool AttemptUpgrade()
        {
            if(_nextUpgradeCost > GameManager.Instance.CoinTotal) return false;
            if(_level == 0) return false;
            if(_level == _card.Abilities.Max(x=>x.MaxLevel)) return false;
            Upgrade();
            return true;
        }

        private void Upgrade()
        {
            _level++;
            UpdateAbilityText();
            GameManager.Instance.RemoveCoins(_nextUpgradeCost);
            _nextUpgradeCost = 0;
            foreach(Abilities.Ability ability in _card.Abilities)
            {
                if(_level !>= ability.MaxLevel)
                    _nextUpgradeCost += ability.GetUpgradeCostForLevel(_level);
            }
            AbilityUpgradeEvent?.Invoke();
        }

        private void UpdateAbilityText()
        {
            _abilityText = "";
            foreach(Abilities.Ability ability in _card.Abilities)
            {
                if(_level !>= ability.MaxLevel) 
                {
                    _abilityText += ability.GetUpdatedAbilityText(_level) + " ";
                } 
                else 
                {
                    _abilityText += ability.GetUpdatedAbilityText(ability.MaxLevel) + " ";
                }
            }
        }
    }
}