using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shorts
{
    public int amount;
    public float shortPrice;
    public int daysCD;

    public Shorts(int amountShorted, float price){
        amount = amountShorted;
        shortPrice = price;
        daysCD = 30;
    }
}
