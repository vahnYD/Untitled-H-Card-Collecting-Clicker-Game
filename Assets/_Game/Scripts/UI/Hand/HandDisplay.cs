/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class HandDisplay : MonoBehaviour
    {
        #region Properties
        [SerializeField] private List<Transform> _cardSpotTransforms = new List<Transform>();
        private List<Transform> _displayedCards = new List<Transform>();
        private GameManager _gameManager;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }
        #endregion
        
        #region Methods
        private void Populate()
        {
            //TODO
        }


        #endregion
    }
}
