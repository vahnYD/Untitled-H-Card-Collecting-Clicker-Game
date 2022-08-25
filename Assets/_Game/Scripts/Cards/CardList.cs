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

namespace Cards
{
    [CreateAssetMenu(fileName = "Card List", menuName = "ScriptableObjects/Lists/CardList")]
    public class CardList : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<Card> _cards = new List<Card>();
        #endregion
        
        #region Methods
        #endregion

        #if UNITY_EDITOR
        [ContextMenu("Create Card")]
        private void CreateCard()
        {
            Card card = ScriptableObject.CreateInstance<Card>();
            card.Initialise(this, "Placeholder Name");
            _cards.Add(card);

            AssetDatabase.AddObjectToAsset(card, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(card);
        }
        #endif
    }
}