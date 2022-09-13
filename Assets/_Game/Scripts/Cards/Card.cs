/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Abilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.Cards
{
    public class Card : ScriptableObject
    {
        #region Properties
        [SerializeField, HideInInspector] private CardList _cardList;
        public CardList CardListRef => _cardList;
        [SerializeField, HideInInspector] private int _id;
        public int Id => _id;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private CardRarity _rarity;
        public CardRarity Rarity => _rarity;
        [SerializeField, Min(0)] private int _soulValue;
        public int SoulValue => _soulValue;
        [SerializeField] private CardType _type;
        public CardType Type => _type;
        [SerializeField, Min(0)] private int _strength;
        public int Strength => _strength;
        [SerializeField, TextArea(4, 10)] private string _flavourText;
        public string FlavourText => _flavourText;
        [SerializeField] private bool _hasAbility;
        public bool HasAbility => _hasAbility;
        [SerializeField] private List<Ability> _abilities;
        public List<Ability> Abilities => _abilities;
        [SerializeField] private List<Sprite> _cardArt = new List<Sprite>();
        public List<Sprite> CardArt => _cardArt;
        #endregion

        #region Methods
        public Sprite GetRandomCardArt()
        {
            if(_cardArt.Count == 0) return Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

            int index = (_cardArt.Count == 1) ? 0 : Random.Range(0, _cardArt.Count);

            return _cardArt[index];
        }
        #endregion

        #if UNITY_EDITOR
        protected void DeleteCard()
        {
            _cardList.RemoveCard(this);
            Destroy(this);
        }

        public void Initialise(CardList cardList, string name, int id)
        {
            _cardList = cardList;
            _id = id;
            _name = name;
            this.name = _name;
        }

        [CustomEditor(typeof(Card))]
        public class CardEditor : Editor
        {
            #region Serialized Properties
            Card _sO;
            SerializedProperty _spCardList;
            SerializedProperty _spId;
            SerializedProperty _spName;
            SerializedProperty _spRarity;
            SerializedProperty _spSoulValue;
            SerializedProperty _spType;
            SerializedProperty _spStrength;
            SerializedProperty _spFlavourText;
            SerializedProperty _spHasAbility;
            SerializedProperty _spAbility;
            SerializedProperty _spCardArt;
            #endregion

            private bool _showDeleteButton = false;

            private void OnEnable()
            {
                _sO = (Card) target;
                _spCardList = serializedObject.FindProperty("_cardList");
                _spId = serializedObject.FindProperty("_id");
                _spName = serializedObject.FindProperty("_name");
                _spRarity = serializedObject.FindProperty("_rarity");
                _spSoulValue = serializedObject.FindProperty("_soulValue");
                _spType = serializedObject.FindProperty("_type");
                _spStrength = serializedObject.FindProperty("_strength");
                _spFlavourText = serializedObject.FindProperty("_flavourText");
                _spHasAbility = serializedObject.FindProperty("_hasAbility");
                _spAbility = serializedObject.FindProperty("_ability");
                _spCardArt = serializedObject.FindProperty("_cardArt");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.LabelField("Card", EditorStyles.boldLabel);
                GUILayout.Label("Card ID: " + _spId.intValue);
                EditorGUILayout.PropertyField(_spName);
                EditorGUILayout.PropertyField(_spType);
                EditorGUILayout.PropertyField(_spRarity);
                EditorGUILayout.PropertyField(_spStrength);
                EditorGUILayout.PropertyField(_spSoulValue);
                EditorGUILayout.PropertyField(_spHasAbility);
                if(_spHasAbility.boolValue)
                    EditorGUILayout.PropertyField(_spAbility);
                EditorGUILayout.PropertyField(_spCardArt);
                EditorGUILayout.PropertyField(_spFlavourText);

                GUILayout.Space(15f);
                if(GUILayout.Button("Delete Card"))
                {
                    _showDeleteButton = !_showDeleteButton;
                }
                if(_showDeleteButton)
                {
                    if(GUILayout.Button("Are you sure?"))
                    {
                        _sO.DeleteCard();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
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
