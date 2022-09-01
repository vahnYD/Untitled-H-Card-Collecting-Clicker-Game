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
    public GameSettingsScriptableObject GameSettingsRef => _gameSettings;

    private int _runningAutoClickerDuration = 0;

    private int _currentClickCoinAmount;
    private int _currentClickCoinAmountManual;
    private Queue<int> _coinPerClickIncreases = new Queue<int>();
    private Queue<int> _coinPerClickIncreasesManual = new Queue<int>();
    private Action ReduceCoinsPerClickAction;
    private Action ReduceCoinsPerClickManualAction;
    private int _coinAmount;
    public int CoinTotal => _coinAmount;

    private int _currentClickSoulAmount = 0;
    private int _currentClickSoulAmountManual = 0;
    private Queue<int> _soulPerClickIncreases = new Queue<int>();
    private Queue<int> _soulPerClickIncreasesManual = new Queue<int>();
    private Action ReduceSoulsPerClickAction;
    private Action ReduceSoulsPerClickManualAction;
    private long _soulAmount;
    public long SoulTotal => _soulAmount;

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
    public int DeckSize => _deck.DeckList.Count;
    private Deck _hand;
    public int HandSize => _hand.DeckList.Count;
    private Deck _grave;
    public int GraveSize => _grave.DeckList.Count;
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

    private void OnEnable()
    {
        StartCoroutine(AutoclickerGlobalTiming());
    }

    private void OnDisable()
    {
        StopCoroutine(AutoclickerGlobalTiming());
    }
	#endregion
	
	#region Methods

    //Clicker Interactions
    public void ManualClick()
    {
        AddCoins(_currentClickCoinAmountManual);
        AddSouls(_currentClickSoulAmountManual);
    }

    public void Click()
    {
        AddCoins(_currentClickCoinAmount);
        AddSouls(_currentClickSoulAmount);
    }

    public void GainAutoclicker(int duration)
    {
        _runningAutoClickerDuration += duration;
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

    public void AddSouls(int souls)
    {
        if(_soulAmount > _gameSettings.MaxSoulsAtBase) return;
        _soulAmount += souls;
        if(_soulAmount > _gameSettings.MaxSoulsAtBase) _soulAmount = _gameSettings.MaxSoulsAtBase;
    }

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

    private void UpdateLewdPoints()
    {
        _lewdPointAmount = Mathf.FloorToInt(_cardInventory.TotalStrength * _cardInventory.TypeMultiplier * GetSoulMult());

        _currentClickCoinAmount = _gameSettings.BaseCoinGainPerClick + Mathf.FloorToInt(_lewdPointAmount / 10);
        Queue<int> autoIncreases = _coinPerClickIncreases;
        for(int i = 0; i < autoIncreases.Count; i++)
        {
            _currentClickCoinAmount += autoIncreases.Dequeue();
        }

        _currentClickCoinAmountManual = _gameSettings.BaseCoinGainPerClick + _lewdPointAmount;
        Queue<int> manualIncreases = _coinPerClickIncreasesManual;
        for(int i = 0; i < manualIncreases.Count; i++)
        {
            _currentClickCoinAmountManual += manualIncreases.Dequeue();
        }
    }

    private float GetSoulMult()
    {
        List<float> soulMults = _gameSettings.SoulMultipliers;
        List<int> soulMultBounds = _gameSettings.SoulMultiplierUpperBounds;

        for(int i = 0; i < soulMults.Count; i++)
        {
            if(i >= soulMultBounds.Count) break;
            if(_soulAmount <= soulMultBounds[i]) return soulMults[i];
        }
        return soulMults[soulMults.Count-1];
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

    public void ReduceCooldownOfCards(float reductionAmount, int cardAmount = 1, bool reduceByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
    {

    }


    // Gacha interactions
    public void ModifyGachaCost(float amount, bool isFlat = false, bool isBlocking = false)
    {

    }

    public void AddStar(int amount = 1)
    {
        _starAmount += amount;
    }
	#endregion

    #region Coroutines
    private IEnumerator AutoclickerGlobalTiming()
    {
        int counter = 0;
        int lpInterval = _gameSettings.AutoClickerLewdpointInterval;
        int abilityInterval = _gameSettings.AutoclickerAbilityInterval;
        int counterRest = (lpInterval > abilityInterval) ? lpInterval : abilityInterval;
        for(;;)
        {
            yield return new WaitForSeconds(1f);
            counter = counter++ % counterRest;
            if(_runningAutoClickerDuration > 0 && counter % abilityInterval == 0) Click();
            if(counter % lpInterval == 0) Click();
            if(_runningAutoClickerDuration > 0) _runningAutoClickerDuration--;
        }
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

    [ContextMenu("Debug/Gain Autoclicker/30sec")]
    private void DebugAddAutoclicker30() => GainAutoclicker(30);

    [ContextMenu("Debug/Gain Autoclicker/1min")]
    private void DebugAddAutoclicker60() => GainAutoclicker(60);

    [ContextMenu("Debug/Gain Autoclicker/10min")]
    private void DebugAddAutoclicker600() => GainAutoclicker(600);
    #endif

    public enum CardGameStates
    {
        Deck,
        Grave,
        Hand
    }
}
