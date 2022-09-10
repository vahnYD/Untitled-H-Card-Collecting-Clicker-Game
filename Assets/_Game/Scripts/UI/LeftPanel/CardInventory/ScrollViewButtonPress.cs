/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.UI
{
    public class ScrollViewButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<bool> PressStateChangedEvent;
        public void OnPointerDown(PointerEventData data) => PressStateChangedEvent?.Invoke(true);
        public void OnPointerUp(PointerEventData data) => PressStateChangedEvent?.Invoke(false);

        private void OnDisable() => PressStateChangedEvent?.Invoke(false);
    }
}
