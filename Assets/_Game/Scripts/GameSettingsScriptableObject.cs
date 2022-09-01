/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Game Settings Container",menuName ="ScriptableObjects/Game Settings Container", order =2)]
public class GameSettingsScriptableObject : ScriptableObject
{
    [SerializeField, Min(0)] private int _startingCoins;
    public int StartingCoins => _startingCoins;
    [SerializeField, Min(0)] private int _startingSouls;
    public int StartingSouls => _startingSouls;
    [SerializeField] private long _maxSoulsBase;
    public long MaxSoulsAtBase => _maxSoulsBase;
    [SerializeField] private List<float> _soulMultipliers = new List<float>();
    public List<float> SoulMultipliers => _soulMultipliers;
    [SerializeField] private List<int> _soulMultiplierUpperBounds = new List<int>();
    public List<int> SoulMultiplierUpperBounds => _soulMultiplierUpperBounds;
    [SerializeField] private List<float> _cardTypeMultipliers = new List<float>();
    public List<float> CardTypeMultipliers => _cardTypeMultipliers;
    [SerializeField] private List<int> _cardTypeMultiplierUpperBounds = new List<int>();
    public List<int> CardTypeMultiplierUpperBounds => _cardTypeMultiplierUpperBounds;
    [SerializeField, Min(0)] private int _baseCoinGainPerClick;
    public int BaseCoinGainPerClick => _baseCoinGainPerClick;
    [SerializeField, Tooltip("If yes: caps at base coin gain;\nOtherwise: caps at 0")] private bool _capCoinsPerClickAtBase;
    public bool isCoinGainLowerBoundAtBase => _capCoinsPerClickAtBase;
    [SerializeField, Min(1)] private int _autoclickerIntervalLp;
    public int AutoClickerLewdpointInterval => _autoclickerIntervalLp;
    [SerializeField, Min(1)] private int _autoclickerIntervalAbility;
    public int AutoclickerAbilityInterval => _autoclickerIntervalAbility;
    [SerializeField, Min(0)] private int _baseGachaPullCost;
    public int BaseGachaPullCost => _baseGachaPullCost;
    [SerializeField, Min(1)] private int _maxDupesAllowedInDeck;
    public int AmountOfDupesAllowedInDeck => _maxDupesAllowedInDeck;
}
