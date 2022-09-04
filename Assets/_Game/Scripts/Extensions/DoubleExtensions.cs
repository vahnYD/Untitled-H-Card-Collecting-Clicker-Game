/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;

public static class DoubleExtensions
{
    public static double CutToDecimalAmount(this double number, int decimalAmount)
    {
        long n = (long)(number * Math.Pow(10, decimalAmount));
        return n/Math.Pow(10, decimalAmount);
    }
}
