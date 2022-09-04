/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;

namespace _Game.Scripts.Extensions
{
    [CreateAssetMenu(fileName ="Int Value", menuName ="ScriptableObjects/Values/Int")]
    public class IntValue : ScriptableObject
    {
        [SerializeField] private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }
        
        public event Action<int> ValueChangedEvent;
    }
}
