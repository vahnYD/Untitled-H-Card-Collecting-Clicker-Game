/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using UnityEngine;
using Cards.Abilities;

namespace Cards
{
    public class Card : ScriptableObject
    {
        #region Properties
        [SerializeField] private CardList _cardList;
        public CardList CardListRef => _cardList;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private CardRarity _rarity;
        public CardRarity Rarity => _rarity;
        [SerializeField, Min(0)] private int _corruptionCost;
        public int CorruptionCost => _corruptionCost;
        [SerializeField] private CardType _type;
        public CardType Type => _type;
        [SerializeField, Min(0)] private int _strength;
        public int Strength => _strength;
        [SerializeField, TextArea(4, 10)] private string _flavourText;
        public string FlavourText => _flavourText;
        [SerializeField] private bool _hasAbility;
        public bool HasAbility => _hasAbility;
        [SerializeField] private CardAbility _ability;
        public CardAbility Ability => _ability;
        [SerializeField] private Sprite _cardArt;
        public Sprite CardArt => _cardArt;
        #endregion

        #region Methods
        #endregion

        #if UNITY_EDITOR
        public void Initialise(CardList cardList, string name)
        {
            _cardList = cardList;
            _name = name;
            this.name = _name;
        }
        #endif

        public enum CardRarity
        {
            Common,
            Rare,
            VeryRare,
            Special
        }

        public enum CardType
        {
            Submission,
            BodyMod,
            Exhibitionism,
            Prostitution,
            Showoff,
            EvilCorruption,
            Allsexual,
            Sluttification,
            CockWorship
        }

        public enum SearchableProperties
        {
           Name,
           Type,
           Rarity
        }
    }
}
