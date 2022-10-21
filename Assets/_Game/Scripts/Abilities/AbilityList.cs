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

namespace _Game.Scripts.Abilities
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
        ///<summary>
        ///Searches for the Ability with the corresponding ID.
        ///</summary>
        ///<param name="id">Ability ID to search for.</param>
        ///<returns>Returns an Ability if available, otherwise returns null.</returns>
        public Ability FindAbilityById(int id)
        {
            if(_abilityList.Any(ability => ability.AbilityID == id))
            {
                Ability output = _abilityList.Where(x=>x.AbilityID == id).FirstOrDefault();
                return output;
            }
            return null;
        }
        #endregion

        #if UNITY_EDITOR

        [ContextMenu("Create Ability")]
        private void DebugCreatAbility() => CreateAbility("Placeholder Name");

        protected void CreateAbility(string abilityName)
        {
            Ability ability = ScriptableObject.CreateInstance<Ability>();

            ability.Initialise(this, abilityName, _abilityContainer.GetNextAbilityId());
            _abilityContainer.IncreaseNextAbilityId();
            _abilityList.Add(ability);

            AssetDatabase.AddObjectToAsset(ability, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(ability);
        }

        public void RemoveAbility(Ability ability)
        {
            _abilityList.Remove(ability);
        }

        [CustomEditor(typeof(AbilityList))]
        public class AbilityListEditor : Editor
        {
            AbilityList _sO;
            private bool _showAbilityName;
            private string _abilityName;
            private void OnEnable()
            {
                _sO = (AbilityList)target;
                _showAbilityName = false;
                _abilityName = "Placeholder Name";
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                GUILayout.Space(5f);
                if(GUILayout.Button("Create Ability"))
                {
                    _showAbilityName = !_showAbilityName;
                }

                if(_showAbilityName)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name:");
                    _abilityName = EditorGUILayout.TextField(_abilityName);

                    if(GUILayout.Button("Confirm", GUILayout.MaxWidth(80f)))
                    {
                        _sO.CreateAbility(_abilityName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        #endif
    }
}
