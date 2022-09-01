/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Extensions
{
    public class CoroutineExtensions : MonoBehaviour
    {
        public static IEnumerator InvokeActionAfterSeconds(Action action, int seconds)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

        public static IEnumerator InvokeActionForSecondsInInterval(Action action, int duration, int interval)
        {
            int counter = 0;
            do
            {
                yield return new WaitForSeconds(1f);
                counter++;
                if(counter == interval)
                {
                    action.Invoke();
                    counter = 0;
                }
                duration--;
            }while(duration > 0);
        }
    }
}