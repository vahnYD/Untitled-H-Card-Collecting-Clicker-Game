/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;

namespace _Game.Scripts.Extensions
{
    [CreateAssetMenu(fileName ="Long Value", menuName ="ScriptableObjects/Values/Long")]
    public class LongValue : ScriptableObject
    {
        [SerializeField] private long _value;
        //large amount of event invokes might cause lag
        //will have to see if implementing a cooldown on that is needed
        public long Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }

        public event Action<long> ValueChangedEvent;
    }
}
