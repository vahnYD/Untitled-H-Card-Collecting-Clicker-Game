/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Cards;

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
            CatchHandSizeChange(0);
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {

        }
        #endregion
        
        #region Methods
        private void CatchHandSizeChange(int newVal)
        {
            if(_gameManager is null) return;
            Populate();
        }
        private void Populate()
        {
            //TODO
        }


        #endregion
    }
}
