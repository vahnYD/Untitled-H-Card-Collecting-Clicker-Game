/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName ="Game Settings Container",menuName ="ScriptableObjects/Game Settings Container", order =2)]
public class GameSettingsScriptableObject : ScriptableObject
{
    //Starting resources
    [SerializeField, Min(0)] private int _startingCoins;
    public int StartingCoins => _startingCoins;
    [SerializeField, Min(0)] private int _startingSouls;
    public int StartingSouls => _startingSouls;
    [SerializeField] private long _maxSoulsBase;
    public long MaxSoulsAtBase => _maxSoulsBase;

    //Lewd Point Calculation related Settings
    [SerializeField] private List<float> _soulMultipliers = new List<float>();
    public List<float> SoulMultipliers => _soulMultipliers;
    [SerializeField] private List<int> _soulMultiplierUpperBounds = new List<int>();
    public List<int> SoulMultiplierUpperBounds => _soulMultiplierUpperBounds;
    [SerializeField] private List<float> _cardTypeMultipliers = new List<float>();
    public List<float> CardTypeMultipliers => _cardTypeMultipliers;
    [SerializeField] private List<int> _cardTypeMultiplierUpperBounds = new List<int>();
    public List<int> CardTypeMultiplierUpperBounds => _cardTypeMultiplierUpperBounds;

    //Click Interaction Settings
    [SerializeField, Min(0)] private int _baseCoinGainPerClick;
    public int BaseCoinGainPerClick => _baseCoinGainPerClick;
    [SerializeField, Tooltip("If yes: caps at base coin gain;\nOtherwise: caps at 0")] private bool _capCoinsPerClickAtBase;
    public bool isCoinGainLowerBoundAtBase => _capCoinsPerClickAtBase;
    [SerializeField, Min(1)] private int _autoclickerIntervalLp;
    public int AutoClickerLewdpointInterval => _autoclickerIntervalLp;
    [SerializeField, Min(1)] private int _autoclickerIntervalAbility;
    public int AutoclickerAbilityInterval => _autoclickerIntervalAbility;

    //Gacha Settings
    [SerializeField] private List<float> _gachaPullWeights;
    public List<float> GachaPullWeights => _gachaPullWeights;
    [SerializeField, Min(0)] private int _baseGachaPullCost;
    public int BaseGachaPullCost => _baseGachaPullCost;
    [SerializeField, Min(0)] private float _baseGachaPullIncrement;
    public float BaseGachaPullIncrement => _baseGachaPullIncrement;
    [SerializeField] private List<int> _gachaPullCostIncreaseReductionUpperBounds = new List<int>();
    public List<int> GachaPullCostIncreaseReductionUpperBounds => _gachaPullCostIncreaseReductionUpperBounds;
    [SerializeField] private List<float> _gachaPullCostIncreaseReductions = new List<float>();
    public List<float> GachaPullCostIncreaseReductions => _gachaPullCostIncreaseReductions;
    [SerializeField, Tooltip("In order from top to bottom: Common, Rare, Very Rare, Special.")] private List<int> _cardDismantleCrystalValues = new List<int>(4);
    public List<int> CardDismantleCrystalValues => _cardDismantleCrystalValues;
    [SerializeField, Min(0)] private int _crystalsPerStar;
    public int CrystalsPerStar => _crystalsPerStar;

    //Card related Settings
    [SerializeField] private int _drawCooldownInSec;
    public int DrawCooldownInSec => _drawCooldownInSec;
    [SerializeField, Min(1)] private int _maxDupesAllowedInDeck;
    public int AmountOfDupesAllowedInDeck => _maxDupesAllowedInDeck;
    [SerializeField, Min(1)] private int _defaultDeckSize;
    public int DefaultDeckSize => _defaultDeckSize;
    [SerializeField] private int _deckSizeIncreasePerVeryRare;
    public int DeckSizeIncreasePerVeryRare => _deckSizeIncreasePerVeryRare;
    [SerializeField, Min(0)] private int _maxVRareCardsAllowedInDeckBase;
    public int MaximumVRareCardsAllowedInDeckAtBase => _maxVRareCardsAllowedInDeckBase;
    [SerializeField] private int _deckSizeIncreasePerSpecial;
    public int DeckSizeIncreasePerSpecial => _deckSizeIncreasePerSpecial;
    [SerializeField, Min(0)] private int _maxSpecialCardsAllowedInDeckBase;
    public int MaximumSpecialCardsAllowedInDeckAtBase => _maxSpecialCardsAllowedInDeckBase;

    #if UNITY_EDITOR
    [CustomEditor(typeof(GameSettingsScriptableObject))]
    public class GameSettingsScriptableObjectEditor : Editor
    {
        #region Serialized Properties
        SerializedProperty _spStartingCoins;
        SerializedProperty _spStartingSouls;
        SerializedProperty _spMaxSoulsBase;

        //Lewd point cal
        SerializedProperty _spSoulMultipliers;
        SerializedProperty _spSoulMultiplierUpperBounds;
        SerializedProperty _spCardTypeMultipliers;
        SerializedProperty _spCardTypeMultiplierUpperBounds;

        //Click Interaction
        SerializedProperty _spBaseCoinGainPerClick;
        SerializedProperty _spCapCoinsPerClickAtBase;
        SerializedProperty _spAutoclickerIntervalLp;
        SerializedProperty _spAutoclickerIntervalAbility;

        //Gacha
        SerializedProperty _spGachaPullWeights;
        SerializedProperty _spBaseGachaPullCost;
        SerializedProperty _spBaseGachaPullIncrement;
        SerializedProperty _spGachaPullCostIncreaseReductions;
        SerializedProperty _spGachaPullCostIncreaseReductionUpperBounds;
        SerializedProperty _spCardDismantleCrystalValues;
        SerializedProperty _spCrystalsPerStar;

        //Card stuff
        SerializedProperty _spDrawCooldownInSec;
        SerializedProperty _spMaxDupesAllowedInDeck;
        SerializedProperty _spDefaultDeckSize;
        SerializedProperty _spDeckSizeIncreasePerVeryRare;
        SerializedProperty _spMaxVRareCardsAllowedInDeckBase;
        SerializedProperty _spDeckSizeIncreasePerSpecial;
        SerializedProperty _spMaxSpecialCardsAllowedInDeckBase;
        #endregion

        private bool _showLewdPointCalcSettings, _showClickInteractionSettings, _showGachaSettings, _showCardSettings = false;

        private void OnEnable()
        {
            _spStartingCoins = serializedObject.FindProperty("_startingCoins");
            _spStartingSouls = serializedObject.FindProperty("_startingSouls");
            _spMaxSoulsBase = serializedObject.FindProperty("_maxSoulsBase");

            //Lewd Point Calc
            _spSoulMultipliers = serializedObject.FindProperty("_soulMultipliers");
            _spSoulMultiplierUpperBounds = serializedObject.FindProperty("_soulMultiplierUpperBounds");
            _spCardTypeMultipliers = serializedObject.FindProperty("_cardTypeMultipliers");
            _spCardTypeMultiplierUpperBounds = serializedObject.FindProperty("_cardTypeMultiplierUpperBounds");

            //Click Interaction
            _spBaseCoinGainPerClick = serializedObject.FindProperty("_baseCoinGainPerClick");
            _spCapCoinsPerClickAtBase = serializedObject.FindProperty("_capCoinsPerClickAtBase");
            _spAutoclickerIntervalLp = serializedObject.FindProperty("_autoclickerIntervalLp");
            _spAutoclickerIntervalAbility = serializedObject.FindProperty("_autoclickerIntervalAbility");

            //Gacha
            _spGachaPullWeights = serializedObject.FindProperty("_gachaPullWeights");
            _spBaseGachaPullCost = serializedObject.FindProperty("_baseGachaPullCost");
            _spBaseGachaPullIncrement = serializedObject.FindProperty("_baseGachaPullIncrement");
            _spGachaPullCostIncreaseReductions = serializedObject.FindProperty("_gachaPullCostIncreaseReductions");
            _spGachaPullCostIncreaseReductionUpperBounds = serializedObject.FindProperty("_gachaPullCostIncreaseReductionUpperBounds");
            _spCardDismantleCrystalValues = serializedObject.FindProperty("_cardDismantleCrystalValues");
            _spCrystalsPerStar = serializedObject.FindProperty("_crystalsPerStar");

            //Cards
            _spDrawCooldownInSec = serializedObject.FindProperty("_drawCooldownInSec");
            _spMaxDupesAllowedInDeck = serializedObject.FindProperty("_maxDupesAllowedInDeck");
            _spDefaultDeckSize = serializedObject.FindProperty("_defaultDeckSize");
            _spDeckSizeIncreasePerVeryRare = serializedObject.FindProperty("_deckSizeIncreasePerVeryRare");
            _spMaxVRareCardsAllowedInDeckBase = serializedObject.FindProperty("_maxVRareCardsAllowedInDeckBase");
            _spDeckSizeIncreasePerSpecial = serializedObject.FindProperty("_deckSizeIncreasePerSpecial");
            _spMaxSpecialCardsAllowedInDeckBase = serializedObject.FindProperty("_maxSpecialCardsAllowedInDeckBase");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Game Settings", EditorStyles.boldLabel);

            GUILayout.Space(5f);

            EditorGUILayout.PropertyField(_spStartingCoins);
            EditorGUILayout.PropertyField(_spStartingSouls);
            EditorGUILayout.PropertyField(_spMaxSoulsBase);

            GUILayout.Space(5f);

            _showClickInteractionSettings = EditorGUILayout.Foldout(_showClickInteractionSettings, "Click Interaction Settings");
            if(_showClickInteractionSettings)
            {
                EditorGUILayout.PropertyField(_spBaseCoinGainPerClick);
                EditorGUILayout.PropertyField(_spCapCoinsPerClickAtBase);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spAutoclickerIntervalLp);
                EditorGUILayout.PropertyField(_spAutoclickerIntervalAbility);
            }

            GUILayout.Space(5f);

            _showCardSettings = EditorGUILayout.Foldout(_showCardSettings, "Card Settings");
            if(_showCardSettings)
            {
                EditorGUILayout.PropertyField(_spDrawCooldownInSec);
                EditorGUILayout.PropertyField(_spDefaultDeckSize);
                EditorGUILayout.PropertyField(_spMaxDupesAllowedInDeck);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spMaxVRareCardsAllowedInDeckBase);
                EditorGUILayout.PropertyField(_spDeckSizeIncreasePerVeryRare);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spMaxSpecialCardsAllowedInDeckBase);
                EditorGUILayout.PropertyField(_spDeckSizeIncreasePerSpecial);
            }

            GUILayout.Space(5f);

            _showGachaSettings = EditorGUILayout.Foldout(_showGachaSettings, "Gacha Settings");
            if(_showGachaSettings)
            {
                EditorGUILayout.PropertyField(_spGachaPullWeights);

                GUILayout.Space(3f);
                
                EditorGUILayout.PropertyField(_spBaseGachaPullCost);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spBaseGachaPullIncrement);
                EditorGUILayout.PropertyField(_spGachaPullCostIncreaseReductions);
                EditorGUILayout.PropertyField(_spGachaPullCostIncreaseReductionUpperBounds);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spCardDismantleCrystalValues);
                EditorGUILayout.PropertyField(_spCrystalsPerStar);
            }

            GUILayout.Space(5f);

            _showLewdPointCalcSettings = EditorGUILayout.Foldout(_showLewdPointCalcSettings, "Lewd Point Calculation Settings");
            if(_showLewdPointCalcSettings)
            {
                EditorGUILayout.PropertyField(_spSoulMultipliers);
                EditorGUILayout.PropertyField(_spSoulMultiplierUpperBounds);

                GUILayout.Space(3f);

                EditorGUILayout.PropertyField(_spCardTypeMultipliers);
                EditorGUILayout.PropertyField(_spCardTypeMultiplierUpperBounds);
            }
        }
    }
    #endif
}
