/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;
using TMPro;

namespace _Game.Scripts.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class CurrentTimeDisplay : MonoBehaviour
    {
        private TMP_Text _timeDisplayText = null;

        private void Awake() => _timeDisplayText = gameObject.GetComponent<TMP_Text>();
        private void Update() => _timeDisplayText.text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
    }
}
