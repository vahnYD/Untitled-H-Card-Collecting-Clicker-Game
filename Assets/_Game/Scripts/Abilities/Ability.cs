/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using UnityEngine;
using _Game.Scripts.Cards;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.Abilities
{
    public class Ability : ScriptableObject
    {

        #region Properties
        [SerializeField, HideInInspector] private AbilityList _abilityLevelList;
        public AbilityList AbilityLevelListRef => _abilityLevelList;

        //Basic properties
        [SerializeField, HideInInspector] private int _id;
        public int AbilityID => _id;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField, TextArea(4, 10), Tooltip("{0} = Flat Coin Gain Amount\n{1} = Coin Per Click Increase Amount\n{2} = Coin Per Click Increase (Manual Only) Amount\n\n{3} = Flat Soul Gain Amount\n{4} = Soul Per Click Gain Amount\n{5} = Soul Per Click Gain (Manual Only) Amount\n\n{6} = Draw Cards Amount\n{7} = Search in Deck Amount\n{8} = Search in Deck Property\n{9} = Search in Deck Name\n{10} = Search in Deck Type\n{11} = Search in Deck Rarity\n\n{12} = Search in Grave Amount\n{13} = Search in Grave Property\n{14} = Search in Grave Name\n{15} = Search in Grave Type\n{16} = Search in Grave Rarity\n\n{17} = Return from Hand Amount\n{18} = Return from Hand Property\n{19} = Return from Hand Name\n{20} = Return from Hand Type\n{21} = Return from Hand Rarity\n\n{22} = Return from Grave Amount\n{23} = Return from Grave Property\n{24} = Return from Grave Name\n{25} = Return from Grave Type\n{26} = Return from Grave Rarity\n\n{27} = Send Cards from Hand Amount\n{28} = Send Cards from Hand Property\n{29} = Send Cards from Hand Name\n{30} = Send Cards from Hand Type\n{31} = Send Cards from Hand Rarity\n\n{32} = Send Cards from Deck Amount\n{33} = Send Cards from Deck Property\n{34} = Send Cards from Deck Name\n{35} = Send Cards from Deck Type\n{36} = Send Cards from Deck Rarity\n\n{37} = Cooldown Reduction Card Amount\n{38} = Cooldown Reduction Reduction Amount\n{39} = Cooldown Reduction Property\n{40} = Cooldown Reduction Name\n{41} = Cooldown Reduction Type\n{42} = Cooldown Reduction Rarity\n\n{43} = Gacha Pull Cost Change\n\n{44} = Destroy Cards Amount\n{45} = Destroy Cards Property\n{46} = Destroy Cards Name\n{47} = Destroy Cards Type\n{48} = Destroy Cards Rarity\n\n{49} = Gain Stars Amount\n\n{50} = Autoclicker Duration")] 
        private string _abilityText; 
        [SerializeField, Min(1), Tooltip("Max Level < 2 => Un-upgradable")] private int _maxLevel;
        public int MaxLevel => _maxLevel;
        [SerializeField, Min(0)] private int _cooldownInSec;
        public int CooldownInSec;
        
        //Upgrade
        [SerializeField, Min(0)] private int _baseUpgradeCost;
        public int BaseUpgradeCost => _baseUpgradeCost;
        [SerializeField] private float _upgradeCostMultAdditivePart;
        [SerializeField] private float _upgradeCostMultMultiplicativePart;
        [SerializeField] private float _effectMultPerLevel;

        //Possible Ability Effects
        //Coin Gain
        [SerializeField] private bool _coinGainEffect;
        [SerializeField, Min(0)] private int _coinGain;
        [SerializeField, Min(0)] private float _coinPerClickIncrease;
        [SerializeField, Min(0)] private float _coinPerClickIncreaseManual;

        //Soul Gain
        [SerializeField] private bool _soulGainEffect;
        [SerializeField, Min(0)] private int _soulGain;
        [SerializeField, Min(0)] private float _soulPerClickGain;
        [SerializeField, Min(0)] private float _soulPerClickGainManual;

        //Draw Cards
        [SerializeField] private bool _drawCards;
        [SerializeField, Min(0)] private int _drawCardsAmount;

        //Search Cards
        [SerializeField] private bool _deckSearch; 
        [SerializeField, Min(0)] private int _deckSearchAmount;
        [SerializeField] private bool _deckSearchByProperty;
        [SerializeField] private Card.SearchableProperties _propertyToSearchDeck;
        [SerializeField] private string _nameToSearchDeck;
        [SerializeField] private Card.CardType _typeToSearchDeck;
        [SerializeField] private Card.CardRarity _rarityToSearchDeck;
        [SerializeField] private bool _graveSearch;
        [SerializeField, Min(0)] private int _graveSearchAmount;
        [SerializeField] private bool _graveSearchByProperty;
        [SerializeField] private Card.SearchableProperties _propertyToSearchGrave;
        [SerializeField] private string _nameToSearchGrave;
        [SerializeField] private Card.CardType _typeToSearchGrave;
        [SerializeField] private Card.CardRarity _rarityToSearchGrave;

        //Return Cards to Deck
        [SerializeField] private bool _returnCardsFromHand;
        [SerializeField, Min(0)] private int _amountOfCardsToReturnHand;
        [SerializeField] private bool _returnCardsByPropertyHand;
        [SerializeField] private Card.SearchableProperties _propertyReturnCardsHand;
        [SerializeField] private string _nameReturnCardsHand;
        [SerializeField] private Card.CardType _typeReturnCardsHand;
        [SerializeField] private Card.CardRarity _rarityReturnCardsHand;
        [SerializeField] private bool _returnCardsFromGrave;
        [SerializeField, Min(0)] private int _amountOfCardsToReturnGrave;
        [SerializeField] private bool _returnCardsByPropertyGrave;
        [SerializeField] private Card.SearchableProperties _propertyReturnCardsGrave;
        [SerializeField] private string _nameReturnCardsGrave;
        [SerializeField] private Card.CardType _typeReturnCardsGrave;
        [SerializeField] private Card.CardRarity _rarityReturnCardsGrave;

        //Send Cards to Grave
        [SerializeField] private bool _sendGraveHand;
        [SerializeField, Min(0)] private int _amountToSendGraveHand;
        [SerializeField] private bool _sendGraveByPropertyHand;
        [SerializeField] private Card.SearchableProperties _propertyToSendGraveHand;
        [SerializeField] private string _nameToSendGraveHand;
        [SerializeField] private Card.CardType _typeToSentGraveHand;
        [SerializeField] private Card.CardRarity _rarityToSentGraveHand;
        [SerializeField] private bool _sendGraveDeck;
        [SerializeField, Min(0)] private int _amountToSendGraveDeck;
        [SerializeField] private bool _sendGraveByPropertyDeck;
        [SerializeField] private Card.SearchableProperties _propertyToSendGraveDeck;
        [SerializeField] private string _nameToSendGraveDeck;
        [SerializeField] private Card.CardType _typeToSendGraveDeck;
        [SerializeField] private Card.CardRarity _rarityToSendGraveDeck;

        //Card Cooldown Reduction
        [SerializeField] private bool _cooldownReduction;
        [SerializeField] private int _amountCardsCooldownReduction;
        [SerializeField] private float _amountReductionCooldownReduction;
        [SerializeField] private bool _flatCooldownReduction;
        [SerializeField] private bool _cooldownReductionByProperty;
        [SerializeField] private Card.SearchableProperties _propertyCooldownReduction;
        [SerializeField] private string _nameCooldownReduction;
        [SerializeField] private Card.CardType _typeCooldownReduction;
        [SerializeField] private Card.CardRarity _rarityCooldownReduction;

        //Gacha Pull Cost Manipulation
        [SerializeField] private bool _manipulateGachaPullCost;
        [SerializeField] private float _gachaPullCostChange;
        [SerializeField] private bool _blockOtherGachaCostChanges;

        //Removing Cards from the Players Inventory
        [SerializeField] private bool _destroyCards;
        [SerializeField, Min(0)] private int _amountOfCardsToDestroy;
        [SerializeField] private bool _destroyCardsByProperty;
        [SerializeField] private Card.SearchableProperties _propertyDestroyCards;
        [SerializeField] private string _nameDestroyCards;
        [SerializeField] private Card.CardType _typeDestroyCards;
        [SerializeField] private Card.CardRarity _rarityDestroyCards;

        //Gain stars
        [SerializeField] private bool _gainStars;
        [SerializeField, Min(0)] private int _gainStarsAmount;

        //Get Autoclicker
        [SerializeField] private bool _gainAutoclicker;
        [SerializeField, Min(0)] private int _autoclickerDuration;
        #endregion

        #region Methods
        public void ActivateAbility(int level = 1)
        {
            GameManager gameManager = GameManager.Instance;

            if(_sendGraveHand)
            {
                int amountToSendPostMult = (_maxLevel < 2) ? _amountToSendGraveHand : CalculateEffectForLevel(_amountToSendGraveHand, level);

                if(!_sendGraveByPropertyHand)
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Hand, GameManager.CardGameStates.Grave, amountToSendPostMult);
                }
                else
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Hand, GameManager.CardGameStates.Grave, amountToSendPostMult, true, _propertyToSendGraveHand, _nameToSendGraveHand, _typeToSentGraveHand, _rarityToSentGraveHand);
                }
            }

            if(_sendGraveDeck)
            {
                int amountToSendPostMult = (_maxLevel < 2) ? _amountToSendGraveDeck : CalculateEffectForLevel(_amountToSendGraveDeck, level);

                if(!_sendGraveByPropertyDeck)
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Deck, GameManager.CardGameStates.Grave ,amountToSendPostMult);
                }
                else
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Deck, GameManager.CardGameStates.Grave, amountToSendPostMult, true, _propertyToSendGraveDeck, _nameToSendGraveDeck, _typeToSendGraveDeck, _rarityToSendGraveDeck);
                }
            }

            if(_returnCardsFromHand)
            {
                int amountToReturnPostMult = (_maxLevel < 2) ? _amountOfCardsToReturnHand : CalculateEffectForLevel(_amountOfCardsToReturnHand, level);

                if(!_returnCardsByPropertyHand)
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Hand, GameManager.CardGameStates.Deck, amountToReturnPostMult);
                }
                else
                {
                    gameManager.MoveCards(GameManager.CardGameStates.Hand, GameManager.CardGameStates.Deck, amountToReturnPostMult, true, _propertyReturnCardsHand, _nameReturnCardsHand, _typeReturnCardsHand, _rarityReturnCardsHand);
                }
            }

            if(_destroyCards)
            {
                int amountToDestroyPostMult = (_maxLevel < 2) ? _amountOfCardsToDestroy : CalculateEffectForLevel(_amountOfCardsToDestroy, level);

                if(!_destroyCardsByProperty)
                {
                    gameManager.DestroyCards(amountToDestroyPostMult);
                }
                else
                {
                    gameManager.DestroyCards(amountToDestroyPostMult, true, _propertyDestroyCards, _nameDestroyCards, _typeDestroyCards, _rarityDestroyCards);
                }
            }

            if(_coinGainEffect)
            {
                if(_coinGain > 0)
                {
                    int coinGainPostMult = (_maxLevel < 2) ? _coinGain : CalculateEffectForLevel(_coinGain, level);
                    gameManager.AddCoins(coinGainPostMult);
                }
            }

            if(_soulGainEffect)
            {

            }

            if(_drawCards)
            {

            }

            if(_deckSearch)
            {

            }

            if(_graveSearch)
            {

            }

            if(_returnCardsFromGrave)
            {

            }

            if(_cooldownReduction)
            {

            }

            if(_manipulateGachaPullCost)
            {

            }

            if(_gainStars)
            {

            }

            if(_gainAutoclicker)
            {

            }
        }

        public int GetUpgradeCostForLevel(int level)
        {
            return (int)Mathf.Floor(_baseUpgradeCost * ((_upgradeCostMultAdditivePart + (_upgradeCostMultMultiplicativePart * level))/100));
        }

        // if the ability is upgradable calculates the effect multipliers for the given level, then replaces the numbers in the ability text and returns the string
        public string GetUpdatedAbilityText(int level = 1)
        {
            int coinGainPostMult = (_maxLevel < 2) ? _coinGain : CalculateEffectForLevel(_coinGain, level);
            float coinPerClickIncreasePostMult = (_maxLevel < 2) ? _coinPerClickIncrease : CalculateEffectForLevel(_coinPerClickIncrease, level);
            float coinPerClickIncreaseManualPostMult = (_maxLevel < 2) ? _coinPerClickIncreaseManual : CalculateEffectForLevel(_coinPerClickIncreaseManual, level);

            int soulGainPostMult = (_maxLevel < 2) ? _soulGain : CalculateEffectForLevel(_soulGain, level);
            float soulPerClickGainPostMult = (_maxLevel < 2) ? _soulPerClickGain : CalculateEffectForLevel(_soulPerClickGain, level);
            float soulPerClickGainManualPostMult = (_maxLevel < 2) ? _soulPerClickGainManual : CalculateEffectForLevel(_soulPerClickGainManual, level);

            int drawCardsPostMult = (_maxLevel < 2) ? _drawCardsAmount : CalculateEffectForLevel(_drawCardsAmount, level);
            int deckSearchAmountPostMult = (_maxLevel < 2) ? _deckSearchAmount : CalculateEffectForLevel(_deckSearchAmount, level);
            int graveSearchAmountPostMult = (_maxLevel < 2) ? _graveSearchAmount : CalculateEffectForLevel(_graveSearchAmount, level);
            int amountOfCardsToReturnHandPostMult = (_maxLevel < 2) ? _amountOfCardsToReturnHand : CalculateEffectForLevel(_amountOfCardsToReturnHand, level);
            int amountOfCardsToReturnGravePostMult = (_maxLevel < 2) ? _amountOfCardsToReturnGrave : CalculateEffectForLevel(_amountOfCardsToReturnGrave, level);
            int amountToSendGraveHandPostMult = (_maxLevel < 2) ? _amountToSendGraveHand : CalculateEffectForLevel(_amountToSendGraveHand, level);
            int amountToSendGraveDeckPostMult = (_maxLevel < 2) ? _amountToSendGraveDeck : CalculateEffectForLevel(_amountToSendGraveDeck, level);

            int cooldownReductionCardAmountPostMult = (_maxLevel < 2) ? _amountCardsCooldownReduction : CalculateEffectForLevel(_amountCardsCooldownReduction, level);
            float cooldownReductionReductionAmountPostMult = (_maxLevel < 2) ? _amountReductionCooldownReduction : CalculateEffectForLevel (_amountReductionCooldownReduction, level);

            float gachaPullCostChangePostMult = (_maxLevel < 2) ? _gachaPullCostChange : CalculateEffectForLevel(_gachaPullCostChange, level);

            int amountOfCardsToDestroyPostMult = (_maxLevel < 2) ? _amountOfCardsToDestroy : CalculateEffectForLevel(_amountOfCardsToDestroy, level);
            int gainStarsAmountPostMult = (_maxLevel < 2) ? _gainStarsAmount : CalculateEffectForLevel(_gainStarsAmount, level);
            int autoclickerDurationPostMult = (_maxLevel < 2) ? _autoclickerDuration : CalculateEffectForLevel(_autoclickerDuration, level);

            return string.Format(
                _abilityText,
                coinGainPostMult, coinPerClickIncreasePostMult, coinPerClickIncreaseManualPostMult,
                soulGainPostMult, soulPerClickGainPostMult, soulPerClickGainManualPostMult,
                drawCardsPostMult,
                deckSearchAmountPostMult, _propertyToSearchDeck, _nameToSearchDeck, _typeToSearchDeck, _rarityToSearchDeck,
                graveSearchAmountPostMult, _propertyToSearchGrave, _nameToSearchGrave, _typeToSearchGrave, _rarityToSearchGrave,
                amountOfCardsToReturnHandPostMult, _propertyReturnCardsHand, _nameReturnCardsHand, _typeReturnCardsHand, _rarityReturnCardsHand,
                amountOfCardsToReturnGravePostMult, _propertyReturnCardsGrave, _nameReturnCardsGrave, _typeReturnCardsGrave, _rarityReturnCardsGrave,
                amountToSendGraveHandPostMult, _propertyToSendGraveHand, _nameToSendGraveHand, _typeToSentGraveHand, _rarityToSentGraveHand,
                amountToSendGraveDeckPostMult, _propertyToSendGraveDeck, _nameToSendGraveDeck, _typeToSendGraveDeck, _rarityToSendGraveDeck, 
                cooldownReductionCardAmountPostMult, cooldownReductionReductionAmountPostMult,
                _propertyCooldownReduction, _nameCooldownReduction, _typeCooldownReduction, _rarityCooldownReduction,
                gachaPullCostChangePostMult,
                amountOfCardsToDestroyPostMult, _propertyDestroyCards, _nameDestroyCards, _typeDestroyCards, _rarityDestroyCards,
                gainStarsAmountPostMult,
                autoclickerDurationPostMult
                );
        }

        private float CalculateEffectForLevel(float effectVal, int level)
        {
            return effectVal * (_effectMultPerLevel * level);
        }

        private int CalculateEffectForLevel(int effectVal, int level)
        {
            return (int)Mathf.Floor(effectVal * (_effectMultPerLevel * level));
        }
        #endregion


        #if UNITY_EDITOR
        public void Initialise(AbilityList abilityList, string name, int id)
        {
            _abilityLevelList = abilityList;
            _id = id;
            _name = name;
            this.name = _name;
        }

        [CustomEditor(typeof(Ability))]
        public class AbilityEditor : Editor
        {
            #region Serialized Properties
            //Basic Properties
            SerializedProperty _spAbilityLevelList;
            SerializedProperty _spId;
            SerializedProperty _spName;
            SerializedProperty _spAbilityText;
            SerializedProperty _spMaxLevel;
            SerializedProperty _spCooldownInSec;

            //Upgrade
            SerializedProperty _spBaseUpgradeCost;
            SerializedProperty _spUpgradeCostMultAdditivePart;
            SerializedProperty _spUpgradeCostMultMultiplicativePart;
            SerializedProperty _spEffectMultPerLevel;

            //Possible Ability Effects
            //Coin Gain
            SerializedProperty _spCoinGainEffect;
            SerializedProperty _spCoinGain;
            SerializedProperty _spCoinPerClickIncrease;
            SerializedProperty _spCoinPerClickIncreaseManual;

            //Soul Gain
            SerializedProperty _spSoulGainEffect;
            SerializedProperty _spSoulGain;
            SerializedProperty _spSoulPerClickGain;
            SerializedProperty _spSoulPerClickGainManual;

            //Draw Cards
            SerializedProperty _spDrawCards;
            SerializedProperty _spDrawCardsAmount;

            //Search Cards
            SerializedProperty _spDeckSearch;
            SerializedProperty _spDeckSearchAmount;
            SerializedProperty _spDeckSearchByProperty;
            SerializedProperty _spPropertyToSearchDeck;
            SerializedProperty _spNameToSearchDeck;
            SerializedProperty _spTypeToSearchDeck;
            SerializedProperty _spRarityToSearchDeck;
            SerializedProperty _spGraveSearch;
            SerializedProperty _spGraveSearchAmount;
            SerializedProperty _spGraveSearchByProperty;
            SerializedProperty _spPropertyToSearchGrave;
            SerializedProperty _spNameToSearchGrave;
            SerializedProperty _spTypeToSearchGrave;
            SerializedProperty _spRarityToSearchGrave;

            //Return Cards to Deck
            SerializedProperty _spReturnCardsFromHand;
            SerializedProperty _spAmountOfCardsToReturnHand;
            SerializedProperty _spReturnCardsByPropertyHand;
            SerializedProperty _spPropertyReturnCardsHand;
            SerializedProperty _spNameReturnCardsHand;
            SerializedProperty _spTypeReturnCardsHand;
            SerializedProperty _spRarityReturnCardsHand;
            SerializedProperty _spReturnCardsFromGrave;
            SerializedProperty _spAmountOfCardsToReturnGrave;
            SerializedProperty _spReturnCardsByPropertyGrave;
            SerializedProperty _spPropertyReturnCardsGrave;
            SerializedProperty _spNameReturnCardsGrave;
            SerializedProperty _spTypeReturnCardsGrave;
            SerializedProperty _spRarityReturnCardsGrave;

            //Send Cards to Grave
            SerializedProperty _spSendGraveHand;
            SerializedProperty _spAmountToSendGraveHand;
            SerializedProperty _spSendGraveByPropertyHand;
            SerializedProperty _spPropertyToSendGraveHand;
            SerializedProperty _spNameToSendGraveHand;
            SerializedProperty _spTypeToSendGraveHand;
            SerializedProperty _spRarityToSendGraveHand;
            SerializedProperty _spSendGraveDeck;
            SerializedProperty _spAmountToSendGraveDeck;
            SerializedProperty _spSendGraveByPropertyDeck;
            SerializedProperty _spPropertyToSendGraveDeck;
            SerializedProperty _spNameToSendGraveDeck;
            SerializedProperty _spTypeToSendGraveDeck;
            SerializedProperty _spRarityToSendGraveDeck;

            //Card Cooldown Reduction
            SerializedProperty _spCooldownReduction;
            SerializedProperty _spAmountCardsCooldownReduction;
            SerializedProperty _spAmountReductionCooldownReduction;
            SerializedProperty _spFlatCooldownReduction;
            SerializedProperty _spCooldownReductionByProperty;
            SerializedProperty _spPropertyCooldownReduction;
            SerializedProperty _spNameCooldownReduction;
            SerializedProperty _spTypeCooldownReduction;
            SerializedProperty _spRarityCooldownReduction;

            //Gacha Pull Cost Manipulation
            SerializedProperty _spManipulateGachaPullCost;
            SerializedProperty _spGachaPullCostChange;
            SerializedProperty _spBlockOtherGachaCostChanges;


            //Removing Cards from the Players Inventory
            SerializedProperty _spDestroyCards;
            SerializedProperty _spAmountOfCardsToDestroy;
            SerializedProperty _spDestroyCardsByProperty;
            SerializedProperty _spPropertyDestroyCards;
            SerializedProperty _spNameDestroyCards;
            SerializedProperty _spTypeDestroyCards;
            SerializedProperty _spRarityDestroyCards;

            //Gain Stars
            SerializedProperty _spGainStars;
            SerializedProperty _spGainStarsAmount;

            //Gain Autoclicker
            SerializedProperty _spGainAutoclicker;
            SerializedProperty _spAutoclickerDuration;
            #endregion

            private bool _showUpgrades, _showAbilities = false;

            private void OnEnable()
            {
                //Basic Properties
                _spAbilityLevelList = serializedObject.FindProperty("_abilityLevelList");
                _spId = serializedObject.FindProperty("_id");
                _spName = serializedObject.FindProperty("_name");
                _spAbilityText = serializedObject.FindProperty("_abilityText");
                _spMaxLevel = serializedObject.FindProperty("_maxLevel");
                _spCooldownInSec = serializedObject.FindProperty("_cooldownInSec");

                //Upgrade
                _spBaseUpgradeCost = serializedObject.FindProperty("_baseUpgradeCost");
                _spUpgradeCostMultAdditivePart = serializedObject.FindProperty("_upgradeCostMultAdditivePart");
                _spUpgradeCostMultMultiplicativePart = serializedObject.FindProperty("_upgradeCostMultMultiplicativePart");
                _spEffectMultPerLevel = serializedObject.FindProperty("_effectMultPerLevel");

                //Possible Ability Effects
                //Coin Gain
                _spCoinGainEffect = serializedObject.FindProperty("_coinGainEffect");
                _spCoinGain = serializedObject.FindProperty("_coinGain");
                _spCoinPerClickIncrease = serializedObject.FindProperty("_coinPerClickIncrease");
                _spCoinPerClickIncreaseManual = serializedObject.FindProperty("_coinPerClickIncreaseManual");

                //Soul Gain
                _spSoulGainEffect = serializedObject.FindProperty("_soulGainEffect");
                _spSoulGain = serializedObject.FindProperty("_soulGain");
                _spSoulPerClickGain = serializedObject.FindProperty("_soulPerClickGain");
                _spSoulPerClickGainManual = serializedObject.FindProperty("_soulPerClickGainManual");

                //Draw Cards
                _spDrawCards = serializedObject.FindProperty("_drawCards");
                _spDrawCardsAmount = serializedObject.FindProperty("_drawCardsAmount");

                //Search Cards
                _spDeckSearch = serializedObject.FindProperty("_deckSearch");
                _spDeckSearchAmount = serializedObject.FindProperty("_deckSearchAmount");
                _spDeckSearchByProperty = serializedObject.FindProperty("_deckSearchByProperty");
                _spPropertyToSearchDeck = serializedObject.FindProperty("_propertyToSearchDeck");
                _spNameToSearchDeck = serializedObject.FindProperty("_nameToSearchDeck");
                _spTypeToSearchDeck = serializedObject.FindProperty("_typeToSearchDeck");
                _spRarityToSearchDeck = serializedObject.FindProperty("_rarityToSearchDeck");
                _spGraveSearch = serializedObject.FindProperty("_graveSearch");
                _spGraveSearchAmount = serializedObject.FindProperty("_graveSearchAmount");
                _spGraveSearchByProperty = serializedObject.FindProperty("_graveSearchByProperty");
                _spPropertyToSearchGrave = serializedObject.FindProperty("_propertyToSearchGrave");
                _spNameToSearchGrave = serializedObject.FindProperty("_nameToSearchGrave");
                _spTypeToSearchGrave = serializedObject.FindProperty("_typeToSearchGrave");
                _spRarityToSearchGrave = serializedObject.FindProperty("_rarityToSearchGrave");

                //Return Cards to Deck
                _spReturnCardsFromHand = serializedObject.FindProperty("_returnCardsFromHand");
                _spAmountOfCardsToReturnHand = serializedObject.FindProperty("_amountOfCardsToReturnHand");
                _spReturnCardsByPropertyHand = serializedObject.FindProperty("_returnCardsByPropertyHand");
                _spPropertyReturnCardsHand = serializedObject.FindProperty("_propertyReturnCardsHand");
                _spNameReturnCardsHand = serializedObject.FindProperty("_nameReturnCardsHand");
                _spTypeReturnCardsHand = serializedObject.FindProperty("_typeReturnCardsHand");
                _spRarityReturnCardsHand = serializedObject.FindProperty("_rarityReturnCardsHand");
                _spReturnCardsFromGrave = serializedObject.FindProperty("_returnCardsFromGrave");
                _spAmountOfCardsToReturnGrave = serializedObject.FindProperty("_amountOfCardsToReturnGrave");
                _spReturnCardsByPropertyGrave = serializedObject.FindProperty("_returnCardsByPropertyGrave");
                _spPropertyReturnCardsGrave = serializedObject.FindProperty("_propertyReturnCardsGrave");
                _spNameReturnCardsGrave = serializedObject.FindProperty("_nameReturnCardsGrave");
                _spTypeReturnCardsGrave = serializedObject.FindProperty("_typeReturnCardsGrave");
                _spRarityReturnCardsGrave = serializedObject.FindProperty("_rarityReturnCardsGrave");

                //Send Cards to Grave
                _spSendGraveHand = serializedObject.FindProperty("_sendGraveHand");
                _spAmountToSendGraveHand = serializedObject.FindProperty("_amountToSendGraveHand");
                _spSendGraveByPropertyHand = serializedObject.FindProperty("_sendGraveByPropertyHand");
                _spPropertyToSendGraveHand = serializedObject.FindProperty("_propertyToSendGraveHand");
                _spNameToSendGraveHand = serializedObject.FindProperty("_nameToSendGraveHand");
                _spTypeToSendGraveHand = serializedObject.FindProperty("_typeToSendGraveHand");
                _spRarityToSendGraveHand = serializedObject.FindProperty("_rarityToSendGraveHand");
                _spSendGraveDeck = serializedObject.FindProperty("_sendGraveDeck");
                _spAmountToSendGraveDeck = serializedObject.FindProperty("_amountToSendGraveDeck");
                _spSendGraveByPropertyDeck = serializedObject.FindProperty("_sendGraveByPropertyDeck");
                _spPropertyToSendGraveDeck = serializedObject.FindProperty("_propertyToSendGraveDeck");
                _spNameToSendGraveDeck = serializedObject.FindProperty("_nameToSendGraveDeck");
                _spTypeToSendGraveDeck = serializedObject.FindProperty("_typeToSendGraveDeck");
                _spRarityToSendGraveDeck = serializedObject.FindProperty("_rarityToSendGraveDeck");

                //Card Cooldown Reduction
                _spCooldownReduction = serializedObject.FindProperty("_cooldownReduction");
                _spAmountCardsCooldownReduction = serializedObject.FindProperty("_amountCardsCooldownReduction");
                _spAmountReductionCooldownReduction = serializedObject.FindProperty("_amountReductionCooldownReduction");
                _spFlatCooldownReduction = serializedObject.FindProperty("_flatCooldownReduction");
                _spCooldownReductionByProperty = serializedObject.FindProperty("_cooldownReductionByProperty");
                _spPropertyCooldownReduction = serializedObject.FindProperty("_propertyCooldownReduction");
                _spNameCooldownReduction = serializedObject.FindProperty("_nameCooldownReduction");
                _spTypeCooldownReduction = serializedObject.FindProperty("_typeCooldownReduction");
                _spRarityCooldownReduction = serializedObject.FindProperty("_rarityCooldownReduction");

                //Gacha Pull Cost Manipulation
                _spManipulateGachaPullCost = serializedObject.FindProperty("_manipulateGachaPullCost");
                _spGachaPullCostChange = serializedObject.FindProperty("_gachaPullCostChange");
                _spBlockOtherGachaCostChanges = serializedObject.FindProperty("_blockOtherGachaCostChanges");

                //Remove Cards from the Players Inventory
                _spDestroyCards = serializedObject.FindProperty("_destroyCards");
                _spAmountOfCardsToDestroy = serializedObject.FindProperty("_amountOfCardsToDestroy");
                _spDestroyCardsByProperty = serializedObject.FindProperty("_destroyCardsByProperty");
                _spPropertyDestroyCards = serializedObject.FindProperty("_propertyDestroyCards");
                _spNameDestroyCards = serializedObject.FindProperty("_nameDestroyCards");
                _spTypeDestroyCards = serializedObject.FindProperty("_typeDestroyCards");
                _spRarityDestroyCards = serializedObject.FindProperty("_rarityDestroyCards");

                //Gain Stars
                _spGainStars = serializedObject.FindProperty("_gainStars");
                _spGainStarsAmount = serializedObject.FindProperty("_gainStarsAmount");
                
                //Gain Autoclicker
                _spGainAutoclicker = serializedObject.FindProperty("_gainAutoclicker");
                _spAutoclickerDuration = serializedObject.FindProperty("_autoclickerDuration");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.LabelField("Ability", EditorStyles.boldLabel);
                GUILayout.Label("Ability ID: " + _spId.intValue.ToString());
                EditorGUILayout.PropertyField(_spName);
                EditorGUILayout.PropertyField(_spMaxLevel);
                EditorGUILayout.PropertyField(_spCooldownInSec);
                EditorGUILayout.PropertyField(_spAbilityText);
                GUILayout.Space(5f);

                _showUpgrades = EditorGUILayout.BeginFoldoutHeaderGroup(_showUpgrades, "Upgrade Details");
                if(_showUpgrades)
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    GUILayout.Label("Effect Multiplier (Per Level)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spEffectMultPerLevel, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    GUILayout.Label("Base Upgrade Cost");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spBaseUpgradeCost, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    GUILayout.Label("Cost Mult = (");
                    EditorGUILayout.PropertyField(_spUpgradeCostMultAdditivePart, GUIContent.none, GUILayout.MaxWidth(35f));
                    GUILayout.Label(" + (");
                    EditorGUILayout.PropertyField(_spUpgradeCostMultMultiplicativePart, GUIContent.none, GUILayout.MaxWidth(35f));
                    GUILayout.Label(" x [Current Level]))%");
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.Space(5f);

                _showAbilities = EditorGUILayout.BeginFoldoutHeaderGroup(_showAbilities, "Ability Details");
                if(_showAbilities)
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));          
                    EditorGUILayout.LabelField("Coin Gain Effect", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spCoinGainEffect, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spCoinGainEffect.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Flat Coin Gain");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spCoinGain, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Coin Per Click Increase");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spCoinPerClickIncrease, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Coin Per Click Increase (Manual Only)");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spCoinPerClickIncreaseManual, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Soul Gain Effect", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spSoulGainEffect, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spSoulGainEffect.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Flat Soul Gain");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spSoulGain, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Soul Per Click Gain");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spSoulPerClickGain, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Soul Per Click Gain (Manual Only)");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spSoulPerClickGainManual, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Draw Cards", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spDrawCards, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spDrawCards.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spDrawCardsAmount, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Search in Deck", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spDeckSearch,GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spDeckSearch.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spDeckSearchAmount, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Search By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spDeckSearchByProperty, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spDeckSearchByProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyToSearchDeck,GUIContent.none);
                            switch((Card.SearchableProperties)_spPropertyToSearchDeck.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameToSearchDeck,GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeToSearchDeck,GUIContent.none);
                                    break;
                                
                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityToSearchDeck,GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Search in Grave", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spGraveSearch, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spGraveSearch.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spGraveSearchAmount, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Search By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spGraveSearchByProperty, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spGraveSearchByProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyToSearchGrave,GUIContent.none);
                            switch((Card.SearchableProperties) _spPropertyToSearchGrave.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameToSearchGrave, GUIContent.none);
                                    break;
                                
                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeToSearchGrave, GUIContent.none);
                                    break;
                                
                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityToSearchGrave, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Return Card/s from Hand to Deck", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spReturnCardsFromHand, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spReturnCardsFromHand.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountOfCardsToReturnHand, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Return By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spReturnCardsByPropertyHand, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spReturnCardsByPropertyHand.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyReturnCardsHand, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyReturnCardsHand.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameReturnCardsHand, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeReturnCardsHand, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityReturnCardsHand, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Return Card/s from Grave to Deck", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spReturnCardsFromGrave, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spReturnCardsFromGrave.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountOfCardsToReturnGrave, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Return By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spReturnCardsByPropertyGrave, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spReturnCardsByPropertyGrave.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyReturnCardsGrave, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyReturnCardsGrave.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameReturnCardsGrave, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeReturnCardsGrave, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityReturnCardsGrave, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Send Card/s from Hand to Grave", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spSendGraveHand,GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spSendGraveHand.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountToSendGraveHand, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Send By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spSendGraveByPropertyHand, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spSendGraveByPropertyHand.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyToSendGraveHand, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyToSendGraveHand.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameToSendGraveHand, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeToSendGraveHand, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityToSendGraveHand, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Send Card/s from Deck to Grave", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spSendGraveDeck, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spSendGraveDeck.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountToSendGraveDeck, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Send By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spSendGraveByPropertyDeck, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spSendGraveByPropertyDeck.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyToSendGraveDeck, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyToSendGraveDeck.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameToSendGraveDeck, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeToSendGraveDeck, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityToSendGraveDeck, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Reduce Cooldown of Exhausted Card/s", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spCooldownReduction,GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spCooldownReduction.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount of Cards");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountCardsCooldownReduction, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount of Reduction");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountReductionCooldownReduction, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Is the reduction flat?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spFlatCooldownReduction, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Limit to Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spCooldownReductionByProperty, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spCooldownReductionByProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyCooldownReduction, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyCooldownReduction.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameCooldownReduction, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeCooldownReduction, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityCooldownReduction, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Manipulate Gacha Pull Cost", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spManipulateGachaPullCost, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spManipulateGachaPullCost.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Gacha Cost Change (Multiplier)");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spGachaPullCostChange, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Block Other Cost Changes?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spBlockOtherGachaCostChanges, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Destroy Card/s", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spDestroyCards, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spDestroyCards.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAmountOfCardsToDestroy, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Destroy By Property?");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spDestroyCardsByProperty, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();

                        if(_spDestroyCardsByProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_spPropertyDestroyCards, GUIContent.none);

                            switch((Card.SearchableProperties) _spPropertyDestroyCards.enumValueIndex)
                            {
                                case Card.SearchableProperties.Name:
                                    EditorGUILayout.PropertyField(_spNameDestroyCards, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Type:
                                    EditorGUILayout.PropertyField(_spTypeDestroyCards, GUIContent.none);
                                    break;

                                case Card.SearchableProperties.Rarity:
                                    EditorGUILayout.PropertyField(_spRarityDestroyCards, GUIContent.none);
                                    break;
                            }
                        }
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Gain Stars", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spGainStars, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spGainStars.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Amount");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spGainStarsAmount, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Gain Autoclicker", EditorStyles.boldLabel, GUILayout.Width(250f));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(_spGainAutoclicker, GUIContent.none, GUILayout.MaxWidth(50f));
                    EditorGUILayout.EndHorizontal();
                    if(_spGainAutoclicker.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300f), GUILayout.ExpandWidth(false));
                        GUILayout.Label("Duration");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(_spAutoclickerDuration, GUIContent.none, GUILayout.MaxWidth(50f));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                serializedObject.ApplyModifiedProperties();
            }
        }
        #endif
    }
}