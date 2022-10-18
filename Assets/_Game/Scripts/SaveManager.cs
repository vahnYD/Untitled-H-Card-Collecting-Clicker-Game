/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {get; private set;}
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
    private void Save()
    {

    }

    private void Load()
    {

    }

    private void Reset()
    {
        
    }
	#endregion
}
