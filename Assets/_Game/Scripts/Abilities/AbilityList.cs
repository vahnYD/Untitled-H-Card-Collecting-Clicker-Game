/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cards.Abilities
{
    [CreateAssetMenu(fileName ="Ability List", menuName = "ScriptableObjects/Lists/AbilityList")]
    public class AbilityList : ScriptableObject
    {
        #region Properties
        [SerializeField] private AbilityListManager _abilityContainer;
        public AbilityListManager AbilityContainer => _abilityContainer;
        [SerializeField] private List<Ability> _abilityList = new List<Ability>();
        [SerializeField, HideInInspector] private int _level;
        public int Level => _level;
        #endregion

        #region Unity Event Functions
        #endregion
        
        #region Methods
        public Ability FindAbilityById(int id)
        {
            //! needs implementation
            return null;
        }
        #endregion

        #if UNITY_EDITOR

        [ContextMenu("Create Ability")]
        protected void CreateAbility()
        {
            Ability ability = ScriptableObject.CreateInstance<Ability>();

            ability.Initialise(this, "Placeholder name", _abilityContainer.GetNextAbilityId());
            _abilityContainer.IncreaseNextAbilityId();
            _abilityList.Add(ability);

            AssetDatabase.AddObjectToAsset(ability, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(ability);
        }

        [CustomEditor(typeof(AbilityList))]
        public class AbilityListEditor : Editor
        {
            AbilityList _sO;
            private void OnEnable()
            {
                _sO = (AbilityList)target;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                GUILayout.Space(5f);
                if(GUILayout.Button("Crate Ability"))
                {
                    _sO.CreateAbility();
                }
            }
        }
        #endif
    }
}
