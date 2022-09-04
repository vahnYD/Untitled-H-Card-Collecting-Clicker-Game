/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using UnityEngine;

public static class FloatExtensions
{
    public static float CutToDecimalAmount(this float number, int decimalAmount)
    {
        long n = (long) (number * Mathf.Pow(10, decimalAmount));
        return n/Mathf.Pow(10, decimalAmount);
    }
}
