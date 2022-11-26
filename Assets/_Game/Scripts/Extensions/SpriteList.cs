/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Extensions
{
    [CreateAssetMenu(fileName ="Sprite List", menuName = "ScriptableObjects/Lists/Sprite List/Generic", order =2)]
    public class SpriteList : ScriptableObject
    {
        [SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
        public List<Sprite> Sprites => _spriteList;
        public event Action<int> ValueChangedAtIndexEvent;
        public Sprite this[int index]
        {
            get
            {
                if(index <= _spriteList.Count - 1 && index >= 0) return _spriteList[index];
                else return null;
            }
            set
            {
                if(index <= _spriteList.Count - 1 && index >= 0) 
                {
                    _spriteList[index] = value;
                    ValueChangedAtIndexEvent?.Invoke(index);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.name);
                }
            }
        }
    }
}
