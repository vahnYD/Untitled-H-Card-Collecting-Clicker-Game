/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class CardDropPoint : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            CardObject_Hand obj = eventData.pointerDrag.GetComponent<CardObject_Hand>();
            obj?.AttemptSkillActivation();
        }
    }
}
