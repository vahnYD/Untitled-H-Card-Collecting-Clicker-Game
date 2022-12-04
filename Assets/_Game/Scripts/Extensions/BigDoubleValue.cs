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
    [CreateAssetMenu(fileName ="BigDouble Value", menuName ="ScriptableObjects/Values/BigDouble/Simple", order = 1)]
    public class BigDoubleValue : ScriptableObject, ISavable
    {
        [SerializeField] private BigDouble _value;
        public BigDouble Value
        {
            get => _value;
            set
            {
                if(isStatic) return;
                _value = value;
                ValueChangedEvent?.Invoke(_value);
            }
        }

        [SerializeField] public bool isStatic {get; private set;}

        public event Action<BigDouble> ValueChangedEvent;

        #region Methods
        public void Save()
        {
            //TODO   
        }

        public void Load()
        {
            //TODO
        }
        #endregion

    }
}
