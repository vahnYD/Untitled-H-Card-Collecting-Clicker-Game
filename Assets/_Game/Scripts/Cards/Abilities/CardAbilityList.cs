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

namespace Cards.Abilities
{
    [CreateAssetMenu(fileName = "Card Ability List", menuName = "ScriptableObjects/Lists/CardAbilityList")]
    public class CardAbilityList : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<CardAbility> _abilities = new List<CardAbility>();
        #endregion

        
        
        #region Methods
        #endregion

        #if UNITY_EDITOR
        [ContextMenu("Create Ability")]
        private void CreateAbility()
        {
            CardAbility ability = ScriptableObject.CreateInstance<CardAbility>();
            ability.Initialise(this, "Placeholder name");
            _abilities.Add(ability);

            AssetDatabase.AddObjectToAsset(ability, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(ability);
        }
        #endif
    }
}