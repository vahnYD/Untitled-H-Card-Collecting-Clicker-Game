/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    #region Properties
    [SerializeField] private CardList _cardList;
    [SerializeField, Min(0)] private int _clickCoinAmountDefault;
    private int _currentClickCoinAmount;
    private int _coinAmount;
    public int CoinTotal => _coinAmount;
    private int _soulAmount;
    public int SoulTotal => _soulAmount;
    private int _medalAmount;
    private PlayerCardInventory _cardInventory;
    private Deck _deck;
	#endregion

    #region Unity Event Functions
    private void Awake()
    {
        //singleton pattern check
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _currentClickCoinAmount = _clickCoinAmountDefault;
    }
	#endregion
	
	#region Methods
    public void ManualClick()
    {
        _coinAmount += _currentClickCoinAmount;
    }

    public void Click()
    {

    }

    public int GetCurrentCoinTotal()
    {
        return 0;
    }
    public void RemoveCoins(int coins)
    {

    }
	#endregion

    #if UNITY_EDITOR
    [ContextMenu("Debug/Add Coins/1k")]
    private void DebugAddCoins1000() => _coinAmount += 1000;

    [ContextMenu("Debug/Add Coins/10k")]
    private void DebugAddCoins10000() => _coinAmount += 10000;

    [ContextMenu("Debug/Add Coins/100k")]
    private void DebugAddCoins100000() => _coinAmount += 100000;

    [ContextMenu("Debug/Add Souls/1k")]
    private void DebugAddSouls1000() => _soulAmount += 1000;

    [ContextMenu("Debug/Add Souls/10k")]
    private void DebugAddSouls10000() => _soulAmount += 10000;

    [ContextMenu("Debug/Add Souls/100k")]
    private void DebugAddSouls100000() => _soulAmount += 100000;
    #endif
}
