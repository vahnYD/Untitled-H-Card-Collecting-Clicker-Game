/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;

namespace _Game.Scripts.Extensions
{
    [CreateAssetMenu(fileName ="Modifiable BigDouble Value", menuName ="ScriptableObjects/Values/BigDouble/Modifiable", order = 2)]
    public class ModifiableBigDoubleValue : ScriptableObject, ISavable
    {
        #region Properties
        [SerializeField] private BigDouble _originalValue = new BigDouble();
        public BigDouble OriginalValue
        {
            get => _originalValue;
            private set
            {
                if(_oVStatic) return;
                _originalValue = value;
                _modifiedValue = ((ModificationIsAdditive) ? _originalValue + ModificationAmount : _originalValue * ModificationAmount).Floor();
                OriginalValueChangedEvent?.Invoke(value);
                ModifiedValueChangedEvent?.Invoke(_modifiedValue);
            }
        }

        [SerializeField] private bool _oVStatic;
        public bool OriginalIsStatic => _oVStatic;
        [SerializeField] private bool _strictlyPositive;
        public bool CanBeNegative => !_strictlyPositive;

        [Space]
        [SerializeField] private BigDouble _modifiedValue;
        public BigDouble ModifiedValue
        {
            get => _modifiedValue;
            private set
            {
                _modifiedValue = value;
                if(_originalValue.Equals(0) && !ModificationIsAdditive)
                {
                    ModificationAmount = 0;
                }
                else
                {
                    ModificationAmount = (ModificationIsAdditive) ? (float)_modifiedValue.Subtract(_originalValue).ToDouble() : (float)_modifiedValue.Divide(_originalValue).ToDouble();
                }
                ModifiedValueChangedEvent?.Invoke(value);
                isModified = (_modifiedValue == _originalValue) ? false : true;
            }
        }

        [SerializeField] private bool _isModifiable;
        public bool isModifiable
        {
            get => _isModifiable;
            private set
            {
                _isModifiable = value;
                ModifiabilityStatusChangedEvent?.Invoke(value);
            }
        }

        public bool isModified {get; private set;}
        public float ModificationAmount {get; private set;}
        public bool ModificationIsAdditive {get; private set;}

        public event Action<BigDouble> OriginalValueChangedEvent;
        public event Action<BigDouble> ModifiedValueChangedEvent;
        public event Action<bool> ModifiabilityStatusChangedEvent;
        #endregion

        #region Unity Event Functions
        #endregion
        
        #region Methods
        ///<summary>
        ///Replaces the current Original version of this value with the given <paramref name="newVal">.
        ///Fails if this value is set static.
        ///</summary>
        ///<param name="newVal">New original value.</param>
        public void OverwriteOriginalValue(BigDouble newVal)
        {
            if(_oVStatic) return;

            OriginalValue = newVal;
        }

        ///<summary>
        ///Modifies the original value by the given amount by either adding it or multiplying by it depending on <paramref name="isAdditive">.
        ///Fails if modification has been manually blocked via BlockModification().
        ///</summary>
        ///<param name="modificationValue">Value by which to modify the original value.</param>
        ///<param name="isAdditive">Optional. Sets if the modification is additive or multiplicative. Defaults to false.</param>
        public void ModifyValue(float modificationValue, bool isAdditive = false)
        {
            if(isModifiable is false) return;
            ModificationIsAdditive = isAdditive;
            ModifiedValue = (ModificationIsAdditive) ? OriginalValue + modificationValue : (OriginalValue * modificationValue).Floor();
        }

        ///<summary>
        ///Increases the modified value by the given amount by either adding it directly or adding the modified value multiplied by it depending on <paramref name="isAdditive">.
        ///Fails if the given value is negative or zero, or if the modification has been manually blocked via BlockModification().
        ///</summary>
        ///<param name="modificationValue">Value by which to increase the modified value. Has to be positive.</param>
        ///<param name="isAdditive">Optional. Sets if the increase is additive or multiplicative. Defaults to false.</param>
        public void IncreaseValue(float modificationValue, bool isAdditive = false)
        {
            if(isModifiable is false) return;
            if(modificationValue <= 0) return;
            if(isModified is false) ModificationIsAdditive = isAdditive;
            ModifiedValue = (isAdditive) ? ModifiedValue + modificationValue : (ModifiedValue + (ModifiedValue * modificationValue)).Floor();
        }

        ///<summary>
        ///Decreases the modified value by the given amount by either subtracting it directly or subtracting the modified value multiplied by it depending on <paramref name="isAdditive">.
        ///Fails if the given value is negative or zero, or if the modification has been manually blocked via BlockModification().
        ///</summary>
        ///<param name="modificationValue">Value by which to decrease the modified value. Has to be positive.</param>
        ///<param name="isAdditive">Optional. Sets if the decrease is additive or multiplicative. Defaults to false.</param>
        public void DecreaseValue(float modificationValue, bool isAdditive = false)
        {
            if(isModifiable is false) return;
            if(modificationValue <= 0) return;
            if(isModified is false) ModificationIsAdditive = isAdditive;
            ModifiedValue = (isAdditive) ? ModifiedValue - modificationValue : (ModifiedValue - (ModifiedValue * modificationValue)).Floor();
        }

        ///<summary>
        ///Prevents further changes to the modified value unless the original value is changed.
        ///</summary>
        public void BlockModification() => isModifiable = false;

        ///<summary>
        ///Re-enables changes to the modified value after they were manually blocked.
        ///</summary>
        public void UnblockModification() => isModifiable = true;

        ///<summary>
        ///Sets the modified value back to the original value.
        ///</summary>
        public void ClearModification()
        {
            ModifiedValue = OriginalValue;
        }
        #endregion

        public void Save()
        {
            //TODO
        }

        public void Load()
        {
            //TODO
        }

        #if UNITY_EDITOR
        [ContextMenu("Refresh Modified Value")]
        private void DebugRefreshModifiedValue() => OriginalValue = OriginalValue;
        #endif
    }
}
