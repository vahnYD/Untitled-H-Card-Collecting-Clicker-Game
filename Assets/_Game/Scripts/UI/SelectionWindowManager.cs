/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class SelectionWindowManager : MonoBehaviour
    {
        public static SelectionWindowManager Instance {get; private set;}
        #region Properties
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion
        
        #region Methods
        public async Task<CardInstance[]> SelectCards(ICardList cardPool, int amount = 1, bool cdReduction = false)
        {
            //TODO
            return null;
        }

        public async Task<CardInstance[]> SelectCardsByName(ICardList cardPool, string name, int amount = 1, bool cdReduction = false)
        {
            //TODO
            return null;
        }

        public async Task<CardInstance[]> SelectCardsByType(ICardList cardPool, Card.CardType type, int amount = 1, bool cdReduction = false)
        {
            //TODO
            return null;
        }

        public async Task<CardInstance[]> SelectCardsByRarity(ICardList cardPool, Card.CardRarity rarity, int amount = 1, bool cdReduction = false)
        {
            //TODO
            return null;
        }


        #endregion

        //! selection methods await coroutine finish. Coroutine runs till the Confirm button in the selection window is pressed
    }
}
