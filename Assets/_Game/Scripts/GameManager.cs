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
using _Game.Scripts.UI;

[Serializable]
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
    [SerializeField] private int _coinAmount;
    public int CoinTotal => _coinAmount;
    [SerializeField] private long _totalCoinsEarned;

    private int _currentClickSoulAmount = 0;
    private int _currentClickSoulAmountManual = 0;
    private Queue<int> _soulPerClickIncreases = new Queue<int>();
    private Queue<int> _soulPerClickIncreasesManual = new Queue<int>();
    private Action ReduceSoulsPerClickAction;
    private Action ReduceSoulsPerClickManualAction;
    [SerializeField] private long _soulAmount;
    public long SoulTotal => _soulAmount;

    [SerializeField] private int _lewdPointAmount;
    public int LewdPointTotal => _lewdPointAmount;
    [SerializeField] private int _crystalAmount;
    public int CrystalTotal => _crystalAmount;
    [SerializeField] private int _starAmount;
    public int StarTotal => _starAmount;
    [SerializeField] private int _devilTearAmount;
    public int DevilTearTotal => _devilTearAmount;

    private bool _drawIsBlocked = false;
    public bool isDrawingBlocked => _drawIsBlocked;
    private Action UnblockDrawAction;
    [SerializeField] private PlayerCardInventory _cardInventory = null;
    [SerializeField] private Deck _deck = null;
    public int DeckSize => _deck.DeckList.Count;
    [SerializeField] private Deck _hand = null;
    public int HandSize => _hand.DeckList.Count;
    [SerializeField] private Deck _grave = null;
    public int GraveSize => _grave.DeckList.Count;

    [SerializeField] private bool _firstRound = true;
    [SerializeField] private bool _reachedFirstRoundCap = false;
    public bool FirstRoundGachaCapReached => _reachedFirstRoundCap;
    [SerializeField] private bool _tenPullDisabled = true;
    public bool isTenPullDisabled => _tenPullDisabled;
    [SerializeField] private int _totalGachaPullAmount;
    public int TotalGachaPullAmount => _totalGachaPullAmount;
    private int _gachaPullCost;
    private int _gachaPullCost10;
    private bool _gachaCostIsModified = false;
    private bool _gachaCostChangesAreBlocked = false;
    private bool _starWasUsed = false;
    private bool StarWasUsed => _starWasUsed;
    public bool GachaCostChangesAreBlocked => _gachaCostChangesAreBlocked;
	#endregion

    #region Events
    public static event Action<int> GraveSizeChangedEvent;
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

        if(_cardInventory is null) _cardInventory = new PlayerCardInventory();
        if(_deck is null) _deck = new Deck();
        if(_grave is null) _grave = new Deck();
        if(_hand is null) _hand = new Deck();

        _currentClickCoinAmount = _gameSettings.BaseCoinGainPerClick;
        _currentClickCoinAmountManual = _gameSettings.BaseCoinGainPerClick;

        UpdateGachaPullCost();

        ReduceCoinsPerClickAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmount);
        ReduceCoinsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmountManual, true);
        ReduceSoulsPerClickAction = () => ReduceSoulsPerClick(ref _currentClickSoulAmount);
        ReduceSoulsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickSoulAmountManual, true);

        UnblockDrawAction = () => _drawIsBlocked = false;
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
    public void AddCoins(int coins)
    {
        _coinAmount += coins;
        _totalCoinsEarned += coins;
    }

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
        if(amount < 1) return;
        CardInstance[] drawnCards = new CardInstance[amount];
        for(int i = 0; i < amount; i++)
            drawnCards[i] = _deck.Draw();
        //TODO DrawAnimation shenanigans
        _hand.AddMultipleCards(drawnCards);
        _drawIsBlocked = true;
        StartCoroutine(CoroutineExtensions.InvokeActionAfterSeconds(UnblockDrawAction, _gameSettings.DrawCooldownInSec));
    }

    public async void MoveCards(CardGameStates moveFrom, CardGameStates moveTo, int amount = 1, bool movedByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
    {
        if(amount < 1) return;
        if(moveFrom == moveTo) return;
        CardInstance[] cardsToMove = null;
        Deck placeToMoveFrom = null;
        Deck placeToMoveTo = null;


        switch(moveFrom)
        {
            case CardGameStates.Deck:
                placeToMoveFrom = _deck;
                break;
            case CardGameStates.Grave:
                placeToMoveFrom = _grave;
                break;
            case CardGameStates.Hand:
                placeToMoveFrom = _hand;
                break;
        }

        switch(moveTo)
        {
            case CardGameStates.Deck:
                placeToMoveTo = _deck;
                break;
            case CardGameStates.Grave:
                placeToMoveTo = _grave;
                break;
            case CardGameStates.Hand:
                placeToMoveTo = _hand;
                break;
        }
        bool emptySelection = true;

        Time.timeScale = 0;
        if(!movedByProperty)
        {
            cardsToMove = await SelectionWindowManager.Instance.SelectCards(placeToMoveFrom, amount);
        }
        else
        {
            switch(property)
            {
                case Card.SearchableProperties.Name:
                    cardsToMove = await SelectionWindowManager.Instance.SelectCardsByName(placeToMoveFrom, name, amount);
                    break;
                case Card.SearchableProperties.Type:
                    cardsToMove = await SelectionWindowManager.Instance.SelectCardsByType(placeToMoveFrom, type, amount);
                    break;
                case Card.SearchableProperties.Rarity:
                    cardsToMove = await SelectionWindowManager.Instance.SelectCardsByRarity(placeToMoveFrom, rarity, amount);
                    break;
            }
        }
        Time.timeScale = 1;

        if(cardsToMove is null) return;
        for(int i = 0; i < cardsToMove.Length; i++)
            if(cardsToMove[i] != null) emptySelection = false;
        if(emptySelection) return;

        placeToMoveTo?.AddMultipleCards(cardsToMove);
        if(moveTo == CardGameStates.Grave || moveFrom == CardGameStates.Grave) GraveSizeChangedEvent?.Invoke(_grave.DeckList.Count);
    }

    public void MoveSpecificCard(CardInstance card, GameManager.CardGameStates moveFrom, GameManager.CardGameStates moveTo)
    {
        if(moveFrom == moveTo) return;
        Deck placeToMoveFrom = null;
        Deck placeToMoveTo = null;

        switch(moveFrom)
        {
            case CardGameStates.Deck:
                placeToMoveFrom = _deck;
                break;
            case CardGameStates.Grave:
                placeToMoveFrom = _grave;
                break;
            case CardGameStates.Hand:
                placeToMoveFrom = _hand;
                break;
        }

        switch(moveTo)
        {
            case CardGameStates.Deck:
                placeToMoveTo = _deck;
                break;
            case CardGameStates.Grave:
                placeToMoveTo = _grave;
                break;
            case CardGameStates.Hand:
                placeToMoveTo = _hand;
                break;
        }

        if(!placeToMoveFrom.DeckList.Contains(card)) return;
        placeToMoveFrom.RemoveCard(card);
        placeToMoveTo.AddCard(card);
        if(moveTo == CardGameStates.Grave || moveFrom == CardGameStates.Grave) GraveSizeChangedEvent?.Invoke(_grave.DeckList.Count);
    }

    public async void DestroyCards(int amount = 1, bool destroyByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
    {
        if(amount < 1) return;
        CardInstance[] cardsToDestroy = null;

        Time.timeScale = 0;
        if(!destroyByProperty)
        {
            cardsToDestroy = await SelectionWindowManager.Instance.SelectCards(_cardInventory, amount);
        }
        else
        {
            switch(property)
            {
                case Card.SearchableProperties.Name:
                    cardsToDestroy = await SelectionWindowManager.Instance.SelectCardsByName(_cardInventory, name, amount);
                    break;
                case Card.SearchableProperties.Type:
                    cardsToDestroy = await SelectionWindowManager.Instance.SelectCardsByType(_cardInventory, type, amount);
                    break;
                case Card.SearchableProperties.Rarity:
                    cardsToDestroy = await SelectionWindowManager.Instance.SelectCardsByRarity(_cardInventory, rarity, amount);
                    break;
            }
        }
        Time.timeScale = 1;

        if(cardsToDestroy is null) return;
        for(int i = 0; i < cardsToDestroy.Length; i++)
            if(cardsToDestroy[i] != null) _cardInventory?.RemoveCard(cardsToDestroy[i]);
    }

    public async void ReduceCooldownOfCards(float reductionAmount, bool reductionIsFlat, int cardAmount = 1, bool reduceByProperty = false, Card.SearchableProperties property = Card.SearchableProperties.Type, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
    {
        if(cardAmount < 1) return;
        if(reductionAmount is 0) return;
        CardInstance[] cards = null;
        bool emptySelection = true;

        Time.timeScale = 0;
        if(!reduceByProperty)
        {
            cards = await SelectionWindowManager.Instance.SelectCards(_grave, cardAmount, true);
        }
        else
        {
            switch(property)
            {
                case Card.SearchableProperties.Name:
                    cards = await SelectionWindowManager.Instance.SelectCardsByName(_grave, name, cardAmount, true);
                    break;
                case Card.SearchableProperties.Type:
                    cards = await SelectionWindowManager.Instance.SelectCardsByType(_grave, type, cardAmount, true);
                    break;
                case Card.SearchableProperties.Rarity:
                    cards = await SelectionWindowManager.Instance.SelectCardsByRarity(_grave, rarity, cardAmount, true);
                    break;
            }
        }
        Time.timeScale = 1;

        if(cards is null) return;
        for(int i = 0; i < cards.Length; i++)
            if(cards[i] != null) emptySelection = false;
        if(emptySelection) return;

        for(int i = 0; i < cards.Length; i++)
            CardCooldownManager.Instance.ReduceCooldownForCard(cards[i], reductionAmount, reductionIsFlat);
    }

    public void TriggerCooldownEndForCard(CardInstance card)
    {
        if(!_grave.DeckList.Contains(card)) return;
        _grave.RemoveCard(card);
        _deck.AddCard(card);
        GraveSizeChangedEvent?.Invoke(_grave.DeckList.Count);
    }


    // Gacha interactions
    public Dictionary<CardInstance, bool> GachaPull(bool isTenPull = false)
    {
        int amount = (isTenPull) ? 10 : 1;
        RemoveCoins((isTenPull) ? _gachaPullCost10 : _gachaPullCost);
        Dictionary<CardInstance, bool> pulledCards = new Dictionary<CardInstance, bool>();
        for(int i = 0; i < amount; i++)
        {
            CardInstance card = new CardInstance(_cardList.GetRandomCard());
            bool isDupe = _cardInventory.CheckIfCardIsDuplicate(card);
            if(!isDupe) _cardInventory.AddCard(card);
            else AddCrystals(card.CardRef.Rarity);
            pulledCards.Add(card, isDupe);
        }
        _totalGachaPullAmount += amount;
        if(_totalGachaPullAmount > 10) _tenPullDisabled = false;
        if(_firstRound && _totalGachaPullAmount < _gameSettings.GachaPullCostIncreaseReductionUpperBounds[_gameSettings.GachaPullCostIncreaseReductionUpperBounds.Count-1] -10) _tenPullDisabled = true;
        if(_firstRound && _totalGachaPullAmount >= _gameSettings.GachaPullCostIncreaseReductionUpperBounds[_gameSettings.GachaPullCostIncreaseReductionUpperBounds.Count-1]) _reachedFirstRoundCap = true;
        UpdateGachaPullCost();
        return pulledCards;
    }

    private void UpdateGachaPullCost()
    {
        List<int> bounds = _gameSettings.GachaPullCostIncreaseReductionUpperBounds;
        List<float> mults = _gameSettings.GachaPullCostIncreaseReductions;

        _gachaPullCost = CalcGachaCostRecursive(_totalGachaPullAmount, bounds, mults);

        _gachaPullCost10 = 0;
        for(int i = 0; i < 10; i++)
            _gachaPullCost10 += CalcGachaCostRecursive(_totalGachaPullAmount + i, bounds, mults);
    }

    private int CalcGachaCostRecursive(int counter, List<int> bounds, List<float> mults)
    {
        if(counter <= 0)
            return _gameSettings.BaseGachaPullCost;

        float incrementMult = 1f;
        bool multSet = false;
        for(int i = 0; i < mults.Count - 1; i++)
        {
            if(i >= bounds.Count) break;
            if(counter < bounds[i])
            {
                incrementMult = mults[i];
                multSet = true;
                break;
            }
        }
        if(!multSet && _firstRound) incrementMult = mults[mults.Count - 1];

        return Mathf.FloorToInt(CalcGachaCostRecursive(counter - 1, bounds, mults) + (counter * (_gameSettings.BaseGachaPullIncrement * incrementMult)));
    }

    public void ModifyGachaCost(float amount, bool isFlat = false, bool isBlocking = false, bool starWasUsed = false)
    {
        if(starWasUsed is true && _starWasUsed is true) return;
        if(_gachaCostChangesAreBlocked) return;
        if(!isFlat && !starWasUsed) _gachaPullCost = Mathf.RoundToInt(_gachaPullCost + (_gachaPullCost * amount));
        else if(!starWasUsed) _gachaPullCost += Mathf.RoundToInt(amount);
        if(isBlocking) _gachaCostChangesAreBlocked = true;
        if(starWasUsed is true)
        {
            _starWasUsed = true;
            _gachaPullCost10 = Mathf.RoundToInt(_gachaPullCost10 * 0.5f);
        }
    }

    public void AddCrystals(Card.CardRarity rarity)
    {
        switch(rarity)
        {
            case Card.CardRarity.Common:
                _crystalAmount += _gameSettings.CardDismantleCrystalValues[0];
                break;
            case Card.CardRarity.Rare:
                _crystalAmount += _gameSettings.CardDismantleCrystalValues[1];
                break;
            case Card.CardRarity.VeryRare:
                _crystalAmount += _gameSettings.CardDismantleCrystalValues[2];
                break;
            case Card.CardRarity.Special:
                _crystalAmount += _gameSettings.CardDismantleCrystalValues[3];
                break;
        }

        if(_crystalAmount > _gameSettings.CrystalsPerStar)
        {
            _crystalAmount -= _gameSettings.CrystalsPerStar;
            AddStar();
        }
    }

    public void AddStar(int amount = 1)
    {
        _starAmount += amount;
    }

    public void RemoveStar(int amount = 1)
    {
        _starAmount -= amount;
        if(_starAmount < 0) _starAmount = 0;
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
