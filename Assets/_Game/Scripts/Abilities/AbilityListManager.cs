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

namespace _Game.Scripts.Abilities
{
    [CreateAssetMenu(fileName = "Ability List Manager", menuName = "ScriptableObjects/Lists/AbilityListManager")]
    public class AbilityListManager : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<AbilityList> _abilityLevels = new List<AbilityList>();
        [SerializeField, HideInInspector] private int _nextAbilityId = 0;
        #endregion

        
        
        #region Methods
        public int GetNextAbilityId() => _nextAbilityId;
        public void IncreaseNextAbilityId() => _nextAbilityId++;

        public Ability FindAbilityById(int id)
        {
            //! needs implementation
            return null;
        }
        #endregion

        #if UNITY_EDITOR
        [ContextMenu("Increase Next Ability ID")]
        private void DebugIncreaseNextAbilityId() => _nextAbilityId++;

        [CustomEditor(typeof(AbilityListManager))]
        public class AbilityListManagerEditor : Editor
        {
            SerializedProperty _spNextAbilityId;

            private void OnEnable()
            {
                _spNextAbilityId = serializedObject.FindProperty("_nextAbilityId");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(5f);
                GUILayout.Label("Next Ability ID: "+_spNextAbilityId.intValue.ToString());
            }
        }
        #endif
    }
}