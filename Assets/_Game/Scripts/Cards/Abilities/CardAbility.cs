/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cards.Abilities
{
    public class CardAbility : ScriptableObject
    {

        #region Properties
        [SerializeField] private CardAbilityList _abilityList;
        public CardAbilityList AbilityListRef => _abilityList;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField, TextArea(4, 10)] private string _abilityText; 
        [SerializeField, Min(1)] private int _maxLevel;
        public int MaxLevel => _maxLevel;
        [SerializeField, Min(0)] private int _baseUpgradeCost;
        public int BaseUpgradeCost => _baseUpgradeCost;
        [SerializeField] private int _upgradeCostMultAdditivePart;
        [SerializeField] private int _upgradeCostMultMultiplicativePart;
        [SerializeField] private float _effectMultPerLevel;
        [SerializeField, Min(0)] private int _amountOfEffects;
        [SerializeField, Min(0)] private int _coinGain;
        [SerializeField, Min(0)] private int _coinPerClickIncrease;
        [SerializeField, Min(0)] private int _coinPerClickIncreaseManual;
        [SerializeField, Min(0)] private int _soulGain;
        [SerializeField, Min(0)] private int _soulPerClickIncrease;
        [SerializeField, Min(0)] private int _soulPerClickIncreaseManual;
        [SerializeField, Min(0)] private int _drawCardsAmount;
        [SerializeField] private bool _deckSearch; 
        [SerializeField] private bool _deckSearchByProperty;
        [SerializeField] private Card.SearchableProperties _propertyToSearchDeck;
        [SerializeField] private string _nameToSearchDeck;
        [SerializeField] private Card.CardType _typeToSearchDeck;
        [SerializeField] private Card.CardRarity _rarityToSearchDeck;
        [SerializeField] private bool _graveSearch;
        [SerializeField] private bool _graveSearchByProperty;
        [SerializeField] private Card.SearchableProperties _propertyToSearchGrave;
        [SerializeField] private string _nameToSearchGrave;
        [SerializeField] private Card.CardType _typeToSearchGrave;
        [SerializeField] private Card.CardRarity _rarityToSearchGrave;
        [SerializeField] private bool _returnCardsFromHand;
        [SerializeField, Min(0)] private int _amountOfCardsToReturnHand;
        [SerializeField] private bool _returnCardsFromGrave;
        [SerializeField, Min(0)] private int _amountOfCardsToReturnGrave;
        [SerializeField] private bool _manipulateGachaPullCost;
        [SerializeField] private int _gachaPullCostChange;
        [SerializeField] private bool _destroyCards;
        [SerializeField, Min(0)] private int _amountOfCardsToDestroy;
        [SerializeField, Min(0)] private int _gainStars;
        [SerializeField] private bool _gainAutoclicker;
        [SerializeField, Min(0)] private int _autoclickerDuration;
        #endregion

        #region Methods
        public void ActivateAbility(int level)
        {

        }

        public int GetUpgradeCostForLevel(int level)
        {
            return _baseUpgradeCost * ((_upgradeCostMultAdditivePart + (_upgradeCostMultMultiplicativePart * level))/100);
        }

        public string GetUpdatedAbilityTextForLevel(int level)
        {
            return "";
        }
        #endregion


        #if UNITY_EDITOR
        public void Initialise(CardAbilityList abilityList, string name)
        {
            _abilityList = abilityList;
            _name = name;
            this.name = _name;
        }

        [CustomEditor(typeof(CardAbility))]
        public class CardAbilityEditor : Editor
        {
            #region Serialized Properties
            SerializedProperty _spAbilityList;
            SerializedProperty _spName;
            SerializedProperty _spAbilityText;
            SerializedProperty _spMaxLevel;
            SerializedProperty _spBaseUpgradeCost;
            SerializedProperty _spUpgradeCostMultAdditivePart;
            SerializedProperty _spUpgradeCostMultMultiplicativePart;
            SerializedProperty _spEffectMultPerLevel;
            SerializedProperty _spAmountOfEffects;
            SerializedProperty _spCoinGain;
            SerializedProperty _spCoinPerClickIncrease;
            SerializedProperty _spCoinPerClickIncreaseManual;
            SerializedProperty _spSoulGain;
            SerializedProperty _spSoulPerClickIncrease;
            SerializedProperty _spSoulPerClickIncreaseManual;
            SerializedProperty _spDrawCardsAmount;
            SerializedProperty _spDeckSearch;
            SerializedProperty _spDeckSearchByProperty;
            SerializedProperty _spPropertyToSearchDeck;
            SerializedProperty _spNameToSearchDeck;
            SerializedProperty _spTypeToSearchDeck;
            SerializedProperty _spRarityToSearchDeck;
            SerializedProperty _spGraveSearch;
            SerializedProperty _spGraveSearchByProperty;
            SerializedProperty _spPropertyToSearchGrave;
            SerializedProperty _spNameToSearchGrave;
            SerializedProperty _spTypeToSearchGrave;
            SerializedProperty _spRarityToSearchGrave;
            SerializedProperty _spReturnCardsFromHand;
            SerializedProperty _spAmountOfCardsToReturnHand;
            SerializedProperty _spReturnCardsFromGrave;
            SerializedProperty _spAmountOfCardsToReturnGrave;
            SerializedProperty _spManipulateGachaPullCost;
            SerializedProperty _spGachaPullCostChange;
            SerializedProperty _spDestroyCards;
            SerializedProperty _spAmountOfCardsToDestroy;
            SerializedProperty _spGainStars;
            SerializedProperty _spGainAutoclicker;
            SerializedProperty _spAutoclickerDuration;
            #endregion
        }
        #endif
    }
}