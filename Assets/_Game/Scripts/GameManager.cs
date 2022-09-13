/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    [SerializeField] private LongValue _coinAmount;
    public long CoinTotal => _coinAmount.Value;
    [SerializeField] private long _totalCoinsEarned;

    private int _currentClickSoulAmount = 0;
    private int _currentClickSoulAmountManual = 0;
    private Queue<int> _soulPerClickIncreases = new Queue<int>();
    private Queue<int> _soulPerClickIncreasesManual = new Queue<int>();
    private Action ReduceSoulsPerClickAction;
    private Action ReduceSoulsPerClickManualAction;
    [SerializeField] private LongValue _soulAmount;
    public long SoulTotal => _soulAmount.Value;

    [SerializeField] private IntValue _lewdPointAmount;
    public int LewdPointTotal => _lewdPointAmount.Value;
    public float LewdPointTypeMultiplier => _cardInventory.TypeMultiplier;
    public int LewdPointStrength => _cardInventory.TotalStrength;
    public float LewdPointSoulMultiplier => GetSoulMult();
    [SerializeField] private IntValue _crystalAmount;
    public int CrystalTotal => _crystalAmount.Value;
    [SerializeField] private IntValue _starAmount;
    public int StarTotal => _starAmount.Value;
    [SerializeField] private IntValue _devilTearAmount;
    public int DevilTearTotal => _devilTearAmount.Value;

    [SerializeField] private BoolValue _drawIsBlocked;
    public bool isDrawingBlocked => _drawIsBlocked.Value;
    private Action UnblockDrawAction;
    [SerializeField] private PlayerCardInventory _cardInventory = null;
    [SerializeField] private Deck _deck = null;
    public int DeckSize => _deck.DeckList.Count;
    [SerializeField] private Deck _hand = null;
    public int HandSize => _hand.DeckList.Count;
    [SerializeField] private Deck _grave = null;
    public int GraveSize => _grave.DeckList.Count;

    [SerializeField] private BoolValue _firstRound;
    [SerializeField] private BoolValue _reachedFirstRoundCap;
    public bool FirstRoundGachaCapReached => _reachedFirstRoundCap.Value;
    [SerializeField] private BoolValue _tenPullDisabled;
    public bool isTenPullDisabled => _tenPullDisabled.Value;
    [SerializeField] private int _totalGachaPullAmount;
    public int TotalGachaPullAmount => _totalGachaPullAmount;
    [SerializeField] private IntValue _gachaPullCost;
    [SerializeField] private IntValue _gachaPullCost10;
    [SerializeField] private BoolValue _gachaCostIsModified;
    [SerializeField] private BoolValue _gachaCostChangesAreBlocked;
    [SerializeField] private BoolValue _starWasUsed;
    private bool StarWasUsed => _starWasUsed.Value;
    public bool GachaCostChangesAreBlocked => _gachaCostChangesAreBlocked.Value;
	#endregion

    #region Events
    public static event Action<int> GraveSizeChangedEvent;
    public static event Action<int> DeckSizeChangedEvent;
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

        #if UNITY_EDITOR
        if(_cardList is null || _gameSettings is null || _coinAmount is null || _soulAmount is null || _lewdPointAmount is null || _crystalAmount is null || _starAmount is null || _devilTearAmount is null || _drawIsBlocked is null || _firstRound is null || _reachedFirstRoundCap is null || _tenPullDisabled is null || _gachaPullCost is null || _gachaPullCost10 is null || _gachaCostIsModified is null || _gachaCostChangesAreBlocked is null || _starWasUsed is null)
            Debug.LogWarning("Game Manager is missing Object References");
        #endif

        if(_cardInventory is null) _cardInventory = new PlayerCardInventory();
        if(_deck is null) _deck = new Deck();
        if(_grave is null) _grave = new Deck();
        if(_hand is null) _hand = new Deck();

        _currentClickCoinAmount = 0;
        _currentClickCoinAmountManual = _gameSettings.BaseCoinGainPerClick;

        UpdateGachaPullCost();
        UpdateLewdPoints();

        ReduceCoinsPerClickAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmount);
        ReduceCoinsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickCoinAmountManual, true);
        ReduceSoulsPerClickAction = () => ReduceSoulsPerClick(ref _currentClickSoulAmount);
        ReduceSoulsPerClickManualAction = () => ReduceCoinsPerClick(ref _currentClickSoulAmountManual, true);

        #if UNITY_EDITOR
        if(_drawIsBlocked != null)
        #endif
            UnblockDrawAction = () => _drawIsBlocked.Value = false;
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
        _coinAmount.Value += coins;
        _totalCoinsEarned += coins;
    }

    public void RemoveCoins(int coins)
    {
        if(_coinAmount.Value - coins <= 0)
        {
            _coinAmount.Value = 0;
            return;
        }
        _coinAmount.Value -= coins;
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
        if(_soulAmount.Value > _gameSettings.MaxSoulsAtBase) return;
        _soulAmount.Value += souls;
        if(_soulAmount.Value > _gameSettings.MaxSoulsAtBase) _soulAmount.Value = _gameSettings.MaxSoulsAtBase;
    }

    public void RemoveSouls(int souls)
    {
        if(_soulAmount.Value - souls <= 0)
        {
            _soulAmount.Value = 0;
            return;
        }
        _soulAmount.Value -= souls;
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
        _lewdPointAmount.Value = Mathf.FloorToInt(_cardInventory.TotalStrength * _cardInventory.TypeMultiplier * GetSoulMult());

        _currentClickCoinAmount = _lewdPointAmount.Value;
        Queue<int> autoIncreases = _coinPerClickIncreases;
        for(int i = 0; i < autoIncreases.Count; i++)
        {
            _currentClickCoinAmount += autoIncreases.Dequeue();
        }

        _currentClickCoinAmountManual = _gameSettings.BaseCoinGainPerClick + Mathf.FloorToInt(_lewdPointAmount.Value/10);
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
            if(_soulAmount.Value <= soulMultBounds[i]) return soulMults[i];
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
        _drawIsBlocked.Value = true;
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
        if(moveFrom == CardGameStates.Grave) 
            foreach(CardInstance card in cardsToMove) CardCooldownManager.Instance.RemoveCardFromTracking(card);
            
        placeToMoveFrom?.RemoveMultipleCards(cardsToMove);
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
        if(moveFrom == CardGameStates.Grave) CardCooldownManager.Instance.RemoveCardFromTracking(card);
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
        UpdateLewdPoints();
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

    public void TriggerCooldownEndForCard(CardInstance card, bool noCooldownAnim = false)
    {
        if(!_grave.DeckList.Contains(card)) return;
        _grave.RemoveCard(card);
        _deck.AddCard(card);
        GraveSizeChangedEvent?.Invoke(_grave.DeckList.Count);
    }

    public List<CardInstance> GetOwnedCardsOfType(Card.CardType type) => _cardInventory.GetCardListOfType(type);
    public List<CardInstance> GetOwnedCards() => _cardInventory.CardInventory;
    public List<CardInstance> GetDeckList() => _deck.GetCardList();
    public void RemoveCardFromDeck(CardInstance card)
    {
        _deck.RemoveCard(card);
        DeckSizeChangedEvent?.Invoke(_deck.GetCardList().Count);
    }
    public void AddCardToDeck(CardInstance card)
    {
        _deck.AddCard(card);
        DeckSizeChangedEvent?.Invoke(_deck.GetCardList().Count);
    }

    public void OverwriteDecklist(List<CardInstance> cards)
    {
        _deck.OverwriteDecklist(cards);
    }

    // Gacha interactions
    public Dictionary<CardInstance, bool> GachaPull(bool isTenPull = false)
    {
        int amount = (isTenPull) ? 10 : 1;
        RemoveCoins((isTenPull) ? _gachaPullCost10.Value : _gachaPullCost.Value);
        Dictionary<CardInstance, bool> pulledCards = new Dictionary<CardInstance, bool>();
        for(int i = 0; i < amount; i++)
        {
            CardInstance card = new CardInstance(_cardList.RollWeightedCard(_gameSettings));
            bool isDupe = _cardInventory.CheckIfCardIsDuplicate(card);
            if(!isDupe) _cardInventory.AddCard(card);
            else AddCrystals(card.CardRef.Rarity);
            pulledCards.Add(card, isDupe);
        }
        _totalGachaPullAmount += amount;
        if(_totalGachaPullAmount > 10) _tenPullDisabled.Value = false;
        if(_firstRound && _totalGachaPullAmount < _gameSettings.GachaPullCostIncreaseReductionUpperBounds[_gameSettings.GachaPullCostIncreaseReductionUpperBounds.Count-1] -10) _tenPullDisabled.Value = true;
        if(_firstRound && _totalGachaPullAmount >= _gameSettings.GachaPullCostIncreaseReductionUpperBounds[_gameSettings.GachaPullCostIncreaseReductionUpperBounds.Count-1]) _reachedFirstRoundCap.Value = true;
        UpdateGachaPullCost();
        UpdateLewdPoints();
        return pulledCards;
    }

    private void UpdateGachaPullCost()
    {
        List<int> bounds = _gameSettings.GachaPullCostIncreaseReductionUpperBounds;
        List<float> mults = _gameSettings.GachaPullCostIncreaseReductions;
        int singleCost = _gachaPullCost.Value;
        _gachaPullCost.Value = CalcGachaCost(singleCost, bounds, mults);

        int cost = _gachaPullCost.Value;
        for(int i = 1; i < 10; i++)
        {
            singleCost = CalcGachaCost(singleCost, bounds, mults, i);
            cost += singleCost;
        }
        _gachaPullCost10.Value = cost;
    }

    private int CalcGachaCost(int singlePullCost, List<int> bounds, List<float> mults, int pullIncrease = 0)
    {
        if(singlePullCost is 0) singlePullCost = _gameSettings.BaseGachaPullCost;
        float incrementMult = 1f;
        bool multSet = false;
        for(int i = 0; i < mults.Count - 1; i++)
        {
            if(i >= bounds.Count) break;
            if(_totalGachaPullAmount+pullIncrease < bounds[i])
            {
                incrementMult = mults[i];
                multSet = true;
                break;
            }
        }
        if(!multSet && _firstRound) incrementMult = mults[mults.Count - 1];
        singlePullCost =  Mathf.FloorToInt(singlePullCost + (_totalGachaPullAmount+pullIncrease) * (_gameSettings.BaseGachaPullIncrement * incrementMult));
        return singlePullCost;
    }

    //! Deprecated 
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

        int output = Mathf.FloorToInt(CalcGachaCostRecursive(counter - 1, bounds, mults) + (counter * (_gameSettings.BaseGachaPullIncrement * incrementMult)));
        return output;
    }

    public void ModifyGachaCost(float amount, bool isFlat = false, bool isBlocking = false, bool starWasUsed = false)
    {
        if(starWasUsed is true && _starWasUsed.Value is true) return;
        if(_gachaCostChangesAreBlocked) return;
        if(!isFlat && !starWasUsed) _gachaPullCost.Value = Mathf.RoundToInt(_gachaPullCost.Value + (_gachaPullCost.Value * amount));
        else if(!starWasUsed) _gachaPullCost.Value += Mathf.RoundToInt(amount);
        if(isBlocking) _gachaCostChangesAreBlocked.Value = true;
        if(starWasUsed is true)
        {
            _starWasUsed.Value = true;
            _gachaPullCost10.Value = Mathf.RoundToInt(_gachaPullCost10.Value * 0.5f);
        }
    }

    public void AddCrystals(Card.CardRarity rarity)
    {
        switch(rarity)
        {
            case Card.CardRarity.Common:
                _crystalAmount.Value += _gameSettings.CardDismantleCrystalValues[0];
                break;
            case Card.CardRarity.Rare:
                _crystalAmount.Value += _gameSettings.CardDismantleCrystalValues[1];
                break;
            case Card.CardRarity.VeryRare:
                _crystalAmount.Value += _gameSettings.CardDismantleCrystalValues[2];
                break;
            case Card.CardRarity.Special:
                _crystalAmount.Value += _gameSettings.CardDismantleCrystalValues[3];
                break;
        }

        if(_crystalAmount.Value > _gameSettings.CrystalsPerStar)
        {
            _crystalAmount.Value -= _gameSettings.CrystalsPerStar;
            AddStar();
        }
    }

    public void AddStar(int amount = 1)
    {
        _starAmount.Value += amount;
    }

    public void RemoveStar(int amount = 1)
    {
        _starAmount.Value -= amount;
        if(_starAmount.Value < 0) _starAmount.Value = 0;
    }
	#endregion

    #region Coroutines
    private IEnumerator AutoclickerGlobalTiming()
    {
        int counter = 0;
        int lpInterval = _gameSettings.AutoClickerLewdpointInterval;
        int abilityInterval = _gameSettings.AutoclickerAbilityInterval;
        int counterRest = (lpInterval >= abilityInterval) ? lpInterval : abilityInterval;
        for(;;)
        {
            yield return new WaitForSeconds(1f);
            counter++;
            if(counter >= counterRest) counter = 0;
            if(_runningAutoClickerDuration > 0 && counter % abilityInterval == 0) Click();
            if(counter % lpInterval == 0) Click();
            if(_runningAutoClickerDuration > 0) _runningAutoClickerDuration--;
        }
    }
    #endregion

    #if UNITY_EDITOR
    [ContextMenu("Debug/Add Coins/1k")]
    private void DebugAddCoins1000() => _coinAmount.Value += 1000;

    [ContextMenu("Debug/Add Coins/10k")]
    private void DebugAddCoins10000() => _coinAmount.Value += 10000;

    [ContextMenu("Debug/Add Coins/100k")]
    private void DebugAddCoins100000() => _coinAmount.Value += 100000;

    [ContextMenu("Debug/Add Souls/1k")]
    private void DebugAddSouls1000() => _soulAmount.Value += 1000;

    [ContextMenu("Debug/Add Souls/10k")]
    private void DebugAddSouls10000() => _soulAmount.Value += 10000;

    [ContextMenu("Debug/Add Souls/100k")]
    private void DebugAddSouls100000() => _soulAmount.Value += 100000;

    [ContextMenu("Debug/Add Souls/1M")]
    private void DebugAddSouls1000000() => _soulAmount.Value += 1000000;

    [ContextMenu("Debug/Add Souls/10M")]
    private void DebugAddSouls10000000() => _soulAmount.Value += 10000000;

    [ContextMenu("Debug/Add Crystals/1")]
    private void DebugAddCrystals() => _crystalAmount.Value += 1;

    [ContextMenu("Debug/Add Crystals/10")]
    private void DebugAddCrystals10() => _crystalAmount.Value += 10;

    [ContextMenu("Debug/Add Crystals/100")]
    private void DebugAddCrystals100()
    {
        _crystalAmount.Value += 99;
        AddCrystals(Card.CardRarity.Common);
    }

    [ContextMenu("Debug/Add Stars/1")]
    private void DebugAddStars() => _starAmount.Value += 1;

    [ContextMenu("Debug/Add Stars/10")]
    private void DebugAddStars10() => _starAmount.Value += 10;

    [ContextMenu("Debug/Add Devil Tear/1")]
    private void DebugAddDevilTear() => _devilTearAmount.Value += 1;

    [ContextMenu("Debug/Add Devil Tear/10")]
    private void DebugAddDevilTear10() => _devilTearAmount.Value += 10;

    [ContextMenu("Debug/Add Devil Tear/100")]
    private void DebugAddDevilTear100() => _devilTearAmount.Value += 100;

    [ContextMenu("Debug/Gain Autoclicker/30sec")]
    private void DebugAddAutoclicker30() => GainAutoclicker(30);

    [ContextMenu("Debug/Gain Autoclicker/1min")]
    private void DebugAddAutoclicker60() => GainAutoclicker(60);

    [ContextMenu("Debug/Gain Autoclicker/10min")]
    private void DebugAddAutoclicker600() => GainAutoclicker(600);

    [ContextMenu("Debug/Force Update/Gacha Cost")]
    private void DebugForceUpdateGachaCost() => UpdateGachaPullCost();

    [ContextMenu("Debug/Force Update/Lewd Points")]
    private void DebugForceUpdateLewdPoints() => UpdateLewdPoints();

    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif

    public enum CardGameStates
    {
        Deck,
        Grave,
        Hand
    }
}
