using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portfolio : MonoBehaviour
{
    [SerializeField]
    private float buyPower = 6000000f;
    private float ownedShares = 0f, shortedShares = 500000000f;
    private float buyAmount = 0f, shortAmount = 0f;

    public List<Shorts> shortList = new List<Shorts>(){
        new Shorts(50000000, 1.69f), 
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),
        new Shorts(50000000, 1.69f),};
    public Transform shortLog;
    public GameObject prefab;

    public TMP_Text t_buyPower, t_shortedStocks, t_ownedStocks, t_shortEstimate, t_buyEstimate;
    public TMP_InputField t_buyAmount, t_shortedAmount;

    Stocks stocks;
    // Start is called before the first frame update
    void Start()
    {
        stocks = (Stocks)FindObjectOfType(typeof(Stocks));
        t_shortedStocks.text = shortedShares.ToString("N") + " shares";
        
        OrganizeShortLog();
    }

    // Update is called once per frame
    void Update()
    {   
        t_buyPower.text = buyPower.ToString("C");
        if (t_shortedAmount.text == ""){
            t_shortedAmount.text = "0";
        }
        t_shortEstimate.text = "= " +(float.Parse(t_shortedAmount.text) * stocks.price).ToString("C");

        if (t_buyAmount.text == ""){
            t_buyAmount.text = "0";
        }
        t_buyEstimate.text = "= " +(float.Parse(t_buyAmount.text) * stocks.price).ToString("C");

    }
    public void GetBuyAmount(){
        buyAmount = Int32.Parse(t_buyAmount.text);
    }

    public void GetShortAmount(){
        shortAmount = Int32.Parse(t_shortedAmount.text);
    }
    public void ShortStocks(){
        if(shortAmount > 0){
            buyPower += shortAmount * stocks.price;
            shortedShares += shortAmount;
            shortList.Add(new Shorts((int)shortAmount, stocks.price));
            stocks.shortInfluence += shortAmount / 100000;
            print("shorted: " + shortAmount);
            OrganizeShortLog();

            t_shortedAmount.text = "0";
            shortAmount = 0;
            t_shortedStocks.text = Math.Round(shortedShares, 6).ToString("N") + " shares";
        }
    }

    public void BuyBackShort(float amount, int index){
        buyPower -= amount * stocks.price;
        shortedShares -= amount;
        shortList.RemoveAt(index);
        print("bought back: " + amount);
        OrganizeShortLog();

        t_shortedAmount.text = "0";
        shortAmount = 0;
        t_shortedStocks.text = Math.Round(shortedShares, 6).ToString("N") + " shares";
    }
    public void BuyStocks(){
        if (buyPower >= buyAmount * stocks.price)
        {
            buyPower -= buyAmount * stocks.price;
            ownedShares += buyAmount;
            print("bought: " + buyAmount);
            t_buyAmount.text = "0";
            buyAmount = 0;
            t_ownedStocks.text = Math.Round(ownedShares, 6).ToString("N") + " shares";
        }
    }

    public void SellStocks(){
        if (ownedShares > buyAmount)
        {
            buyPower += buyAmount * stocks.price;
            ownedShares -= buyAmount;
            t_ownedStocks.text = Math.Round(ownedShares, 6).ToString("N") + " shares";
            print("sold: " + buyAmount);
            t_buyAmount.text = "0";
            buyAmount = 0;
        }
    }
    
    void OrganizeShortLog(){
        int y_offset = shortList.Count - 1;
        for(int i = shortLog.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(shortLog.transform.GetChild(i).gameObject);
        }
        foreach (Shorts shorts in shortList){
            GameObject log = Instantiate(prefab, shortLog);
            log.GetComponent<RectTransform>().localPosition -=new Vector3(0, (y_offset * 20),0);
            log.transform.Find("amount").GetComponent<TMP_Text>().text = shorts.amount.ToString();
            log.transform.Find("price").GetComponent<TMP_Text>().text = shorts.shortPrice.ToString("0.00");
            
            print("amount: " + shorts.amount.ToString() + " price: " + shorts.shortPrice.ToString());
            y_offset -= 1;
        }
    }
}
