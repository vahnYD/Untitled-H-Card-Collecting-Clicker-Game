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

namespace _Game.Scripts.Cards
{
    [CreateAssetMenu(fileName = "Card List", menuName = "ScriptableObjects/Lists/CardList")]
    public class CardList : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<Card> _cards = new List<Card>();
        private static int _nextCardId = 0;
        #endregion
        
        #region Methods
        public Card SearchCardById(int id)
        {
            //! needs implementation
            return null;
        }

        public List<Card> GetCardsByRarity(Card.CardRarity cardRarity)
        {
            //! needs implementation
            return null;
        }
        #endregion

        #if UNITY_EDITOR
        [ContextMenu("Create Card")]
        private void DebugCreateCard() => CreateCard("Placeholder Name");

        protected void CreateCard(string cardName)
        {
            Card card = ScriptableObject.CreateInstance<Card>();
            card.Initialise(this, cardName, _nextCardId);
            _nextCardId++;
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

            private void OnEnable()
            {
                _sO = (CardList) target;
                _showCardName = false;
                _cardName = "Placeholder Name";
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(5f);
                if(GUILayout.Button("Create Card"))
                {
                    _showCardName = !_showCardName;
                }

                if(_showCardName)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name:");
                    EditorGUILayout.TextField(_cardName);

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