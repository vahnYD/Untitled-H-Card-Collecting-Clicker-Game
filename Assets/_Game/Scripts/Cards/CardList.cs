/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.Cards
{
    [CreateAssetMenu(fileName = "Card List", menuName = "ScriptableObjects/Lists/CardList")]
    public class CardList : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<Card> _cards = new List<Card>();
        [SerializeField, HideInInspector] private  int _nextCardId = 0;
        protected int NextCardId
        {
            get
            {
                int carry = _nextCardId;
                _nextCardId = carry + 1;
                return carry;
            }
        }
        #endregion
        
        #region Methods
        ///<summary>
        ///Searches for the Card with the given ID.
        ///</summary>
        ///<param name="id">Card ID to search for.</param>
        ///<returns>Returns a Card if available, otherwise returns null.</returns>
        public Card SearchCardById(int id)
        {
            if(_cards.Any(card => card.Id == id))
            {
                Card output = _cards.Where(card => card.Id == id).FirstOrDefault();
                return output;
            }
            return null;
        }

        ///<summary>
        ///Searches for Cards with a given Name.
        ///</summary>
        ///<param name="name">Name of the cards to search for as a string.</param>
        ///<returns>Returns a List of Cards with the given name if available, otherwise returns null.</returns>
        public List<Card> SearchCardsByName(string name)
        {
            if(_cards.Any(card => card.Name == name))
            {
                return _cards.Where(card => card.Name == name).ToList();
            }
            return null;
        }

        ///<summary>
        ///Searches for Cards with a given type.
        ///</summary>
        ///<param name="type">Card.CardType enum value for the type to search by.</param>
        ///<returns>Returns a List of Cards with the given type if available, otherwise returns null.</returns>
        public List<Card> SearchCardsByType(Card.CardType type)
        {
            if(_cards.Any(card => card.Type == type))
            {
                return _cards.Where(card => card.Type == type).ToList();
            }
            return null;
        }

        ///<summary>
        ///Searches for Cards with a given Rarity.
        ///</summary>
        ///<param name="cardRarity">Card.CardRarity Enum value for the rarity to search by.</param>
        ///<returns>Returns a List of Cards with the given rarity if available, otherwise returns null.</returns>
        public List<Card> GetCardsByRarity(Card.CardRarity cardRarity)
        {
            if(_cards.Any(card => card.Rarity == cardRarity))
            {
                return _cards.Where(card => card.Rarity == cardRarity).ToList();
            }
            return null;
        }

        ///<summary>
        ///Returns a randomly chosen Card from the list.
        ///</summary>
        public Card GetRandomCard()
        {
            return _cards[Random.Range(0, _cards.Count)];
        }

        ///<summary>
        ///Returns randomly chosen Card from the list, weighted by rarity.
        ///</summary>
        ///<param name="gameSettings">GameSettingsScriptableObject to read rarity weights from.</param>
        public Card RollWeightedCard(GameSettingsScriptableObject gameSettings)
        {
            int roll = Random.Range(0, 100);
            Card.CardRarity pulledRarity = Card.CardRarity.Common;

            if(roll >= 100 - gameSettings.GachaPullWeights[0])
            {
                pulledRarity = Card.CardRarity.Common;
            }
            else if(roll >= 100 - gameSettings.GachaPullWeights[1] - gameSettings.GachaPullWeights[0])
            {
                pulledRarity = Card.CardRarity.Rare;
            }
            else if(roll >= 100 - gameSettings.GachaPullWeights[2] - gameSettings.GachaPullWeights[1] - gameSettings.GachaPullWeights[0])
            {
                pulledRarity = Card.CardRarity.VeryRare;
            }
            else
            {
                pulledRarity = Card.CardRarity.Special;
            }

            List<Card> cardsOfPulledRarity = _cards.Where(x => x.Rarity == pulledRarity).ToList();

            return cardsOfPulledRarity[Random.Range(0, cardsOfPulledRarity.Count)];
        }
        #endregion

        #if UNITY_EDITOR
        [ContextMenu("Create Card")]
        private void DebugCreateCard() => CreateCard("Placeholder Name");

        [ContextMenu("Increase Next Card ID")]
        private void DebugIncreaseNextCardId() => _nextCardId++;

        public void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }

        protected void CreateCard(string cardName)
        {
            Card card = ScriptableObject.CreateInstance<Card>();
            card.Initialise(this, cardName, NextCardId);
            _cards.Add(card);

            AssetDatabase.AddObjectToAsset(card, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(card);
        }

        [CustomEditor(typeof(CardList))]
        public class CardListEditor : Editor
        {
            CardList _sO;
            private bool _showCardName;
            private string _cardName;
            SerializedProperty _spNextId;

            private void OnEnable()
            {
                _sO = (CardList) target;
                _showCardName = false;
                _cardName = "Placeholder Name";
                _spNextId = serializedObject.FindProperty("_nextCardId");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                GUILayout.Label("Next Card ID: "+_spNextId.intValue.ToString());

                GUILayout.Space(5f);
                if(GUILayout.Button("Create Card"))
                {
                    _showCardName = !_showCardName;
                }

                if(_showCardName)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name:");
                    _cardName = EditorGUILayout.TextField(_cardName);

                    if(GUILayout.Button("Confirm", GUILayout.MaxWidth(80f)))
                    {
                        _sO.CreateCard(_cardName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        #endif
    }
}