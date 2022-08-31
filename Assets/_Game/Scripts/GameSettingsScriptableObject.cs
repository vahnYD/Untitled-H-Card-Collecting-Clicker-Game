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
    [SerializeField, Min(0)] private int _baseCoinGainPerClick;
    public int BaseCoinGainPerClick => _baseCoinGainPerClick;
    [SerializeField, Tooltip("If yes: caps at base coin gain;\nOtherwise: caps at 0")] private bool _capCoinsPerClickAtBase;
    public bool isCoinGainLowerBoundAtBase => _capCoinsPerClickAtBase;
    [SerializeField, Min(0)] private int _baseGachaPullCost;
    public int BaseGachaPullCost => _baseGachaPullCost;
}
