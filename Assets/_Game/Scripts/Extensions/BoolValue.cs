/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;

namespace _Game.Scripts.Extensions
{
    [CreateAssetMenu(fileName ="Bool Value", menuName ="ScriptableObjects/Values/Bool")]
    public class BoolValue : ScriptableObject
    {
        [SerializeField] private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }

        public event Action<bool> ValueChangedEvent;
    }
}
