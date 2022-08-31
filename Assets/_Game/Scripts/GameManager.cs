/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Cards;
using _Game.Scripts.Extensions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    #region Properties
    [SerializeField] private CardList _cardList;
    [SerializeField] private GameSettingsScriptableObject _gameSettings;

    private int _currentClickCoinAmount;
    private int _currentClickCoinAmountManual;
    private Queue<int> _coinPerClickIncreases = new Queue<int>();
    private Queue<int> _coinPerClickIncreasesManual = new Queue<int>();
    private Action ReduceCoinsPerClickAction;
    private Action ReduceCoinsPerClickManualAction;
    private int _coinAmount;
    public int CoinTotal => _coinAmount;

    private int _currentClickSoulAmount;
    private int _currentClickSoulAmountManual;
    private Queue<int> _soulPerClickIncreases = new Queue<int>();
    private Queue<int> _soulPerClickIncreasesManual = new Queue<int>();
    private Action ReduceSoulsPerClickAction;
    private Action ReduceSoulsPerClickManualAction;
    private int _soulAmount;
    public int SoulTotal => _soulAmount;
    private int _lewdPointAmount;
    public int LewdPointTotal => _lewdPointAmount;
    private int _crystalAmount;
    public int CrystalTotal => _crystalAmount;
    private int _starAmount;
    public int StarTotal => _starAmount;
    private int _devilTearAmount;
    public int DevilTearTotal => _devilTearAmount;
    private PlayerCardInventory _cardInventory;
    private Deck _deck;
    private Deck _hand;
    private Deck _grave;
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

        _currentClickCoinAmount = _gameSettings.BaseCoinGainPerClick;
        _currentClickCoinAmountManual = _gameSettings.BaseCoinGainPerClick;

        ReduceCoinsPerClickAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmount);
        ReduceCoinsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmountManual, true);
        ReduceSoulsPerClickAction = () => ReduceSoulsPerClick(ref _currentClickSoulAmount);
        ReduceSoulsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickSoulAmountManual, true);
    }
	#endregion
	
	#region Methods

    //Clicker Interactions
    public void ManualClick()
    {
        _coinAmount += _currentClickCoinAmountManual;
    }

    public void Click()
    {
        _coinAmount += _currentClickCoinAmount;
    }


    // Currency Interactions
    public void AddCoins(int coins) => _coinAmount += coins;

    public void RemoveCoins(int coins)
    {
        if(_coinAmount - coins <= 0)
        {
            _coinAmount = 0;
            return;
        }
        _coinAmount -= coins;
    }

    public void IncreaseCoinsPerClick(int increase, int duration, bool isManual = false)
    {
        if(!isManual)
        {
            _currentClickCoinAmount += increase;
            _coinPerClickIncreases.Enqueue(increase);
            _currentClickCoinAmountManual += increase;
            _coinPerClickIncreasesManual.Enqueue(increase);
            StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceCoinsPerClickAction, duration));
            StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceCoinsPerClickManualAction, duration));
            return;
        }

        _currentClickCoinAmountManual += increase;
        _coinPerClickIncreasesManual.Enqueue(increase);
        StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceCoinsPerClickManualAction, duration));
    }

    private void ReduceCoinsPerClick(ref int val, bool isManual = false)
    {
        int lowerBound = (_gameSettings.isCoinGainLowerBoundAtBase) ? _gameSettings.BaseCoinGainPerClick : 0;
        if(!isManual)
        {
            int reduction = _coinPerClickIncreases.Dequeue();
            if(val - reduction > lowerBound) val -= reduction;
            else val = lowerBound;
            return;
        }
        int reductionManual = _coinPerClickIncreasesManual.Dequeue();
        if(val - reductionManual > lowerBound) val -= reductionManual;
        else val = lowerBound;
    }

    public void AddSouls(int souls) => _soulAmount += souls;

    public void RemoveSouls(int souls)
    {
        if(_soulAmount - souls <= 0)
        {
            _soulAmount = 0;
            return;
        }
        _soulAmount -= souls;
    }

    public void IncreaseSoulsPerClick(int increase, int duration, bool isManual = false)
    {
        if(!isManual)
        {
            _currentClickSoulAmount += increase;
            _currentClickSoulAmountManual += increase;
            _soulPerClickIncreases.Enqueue(increase);
            _soulPerClickIncreasesManual.Enqueue(increase);
            StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceSoulsPerClickAction, duration));
            StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceSoulsPerClickManualAction, duration));
            return;
        }

        _currentClickSoulAmountManual += increase;
        _soulPerClickIncreasesManual.Enqueue(increase);
        StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(ReduceSoulsPerClickManualAction, duration));
    }

    private void ReduceSoulsPerClick(ref int val, bool isManual = false)
    {
        if(!isManual)
        {
            int reduction = _soulPerClickIncreases.Dequeue();
            if(val - reduction > 0) val -= reduction;
            else val = 0;
            return;
        }
        int reductionManual = _soulPerClickIncreasesManual.Dequeue();
        if(val - reductionManual > 0) val -= reductionManual;
        else val = 0;
        return;
    }

    // Card Interactions
    public void DrawCard(int amount = 1)
    {

    }

    public void MoveCards(CardGameStates moveFrom, CardGameStates moveTo, int amount = 1, bool sendByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
    {

    }

    public void DestroyCards(int amount = 1, bool destroyByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
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

    public enum CardGameStates
    {
        Deck,
        Grave,
        Hand
    }
}
