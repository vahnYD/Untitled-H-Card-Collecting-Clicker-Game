/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using System.Linq;
using System.Threading.Tasks;
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
        
        ///<summary>
        ///Creates a CardInstance Object for the given card.
        ///<summary>
        ///<param name="card">Card object to use as base for the CardInstance.</param>
        public CardInstance(Card card)
        {
            _card = card;
            _name = card.Name;
            _cardArt = card.GetRandomCardArt();
            _hasAbility = card.HasAbility;
            if(_hasAbility)
            {
                foreach(Abilities.Ability ability in card.Abilities)
                {
                    _nextUpgradeCost += ability.GetUpgradeCostForLevel(2);
                }
                UpdateAbilityText();
            }
            _level = (_hasAbility && _card.Abilities.Where(x=>x.MaxLevel >=2).Count() >= 1) ? 1 : 0;
        }

        ///<summary>
        ///Triggers the abilities this card posesses, start cooldown, and moves it from the hand to the grave yard after.
        ///Will not trigger if on cooldown or there is no ability.
        ///</summary>
        public bool ActivateAbility(ref int cooldownInSec)
        {
            if(!_hasAbility) return false;
            if(_onCooldown) return false;

            Queue<Action> abilities = new Queue<Action>();
            Queue<Action> carry;

            foreach(Abilities.Ability ability in _card.Abilities)
            {
                carry = ability.GetAbilityActions(_level);
                if(carry is null) return false;
                for(int i = 0; i < carry.Count; i++)
                    abilities.Enqueue(carry.Dequeue());
            }

            //! Presently causes issues if multiple abilities that use the selection window get activated quickly.

            for(int i = 0; i < abilities.Count; i++)
                abilities.Dequeue().Invoke();

            _onCooldown = true;
            foreach(Abilities.Ability ability in _card.Abilities)
                cooldownInSec += ability.CooldownInSec;
            
            return true;
        }

        public void OffCooldown() => _onCooldown = false;

        ///<summary>
        ///Attempts to Upgrade the ability of this card.
        ///Fails if there isnt enough coins to afford the update, if this card isnt upgradable, or if this card is already at max level.
        ///</summary>
        ///<returns>Returns true if the card was upgraded, false if it failed.</returns>
        public bool AttemptUpgrade()
        {
            if(_nextUpgradeCost > GameManager.Instance.CoinTotal) return false;
            if(_level == 0) return false;
            if(_level == _card.Abilities.Max(x=>x.MaxLevel)) return false;
            Upgrade();
            return true;
        }

        ///<summary>
        ///Increases the level of the card, updates the ability text, removes the cost of the upgrade from the game manager,
        ///and calculates the next upgrade cost before invoking the an AbilityUpgradeEvent.
        ///</summary>

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

        ///<summary>
        ///Composits the ability text for this card by concatenating the ability text of all it's abilities in order.
        ///</summary>
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