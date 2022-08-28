/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Abilities
{
    [CreateAssetMenu(fileName = "Ability List Manager", menuName = "ScriptableObjects/Lists/AbilityListManager")]
    public class AbilityListManager : ScriptableObject
    {
        #region Properties
        [SerializeField] private List<AbilityList> _abilityLevels = new List<AbilityList>();
        private static int _nextAbilityId = 0;
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
    }
}