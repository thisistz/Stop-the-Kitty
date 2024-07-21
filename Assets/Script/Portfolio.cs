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
    public GameObject prefab, winScreen, loseScreen;
    public float dividendRatio = 0.1f, shortInterestRatio = 0.1f;

    public TMP_Text t_buyPower, t_shortedStocks, t_ownedStocks, t_shortEstimate, t_buyEstimate, t_PR, t_endScreen, t_loseMsg;
    public TMP_InputField t_buyAmount, t_shortedAmount;

    int pricePR = 5000000;
    Stocks stocks;
    GeminiLLM llm;
    // Start is called before the first frame update
    void Start()
    {
        stocks = (Stocks)FindObjectOfType(typeof(Stocks));
        llm = (GeminiLLM)FindObjectOfType(typeof(GeminiLLM));
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
        if(buyPower >= amount * stocks.price){
            buyPower -= amount * stocks.price;
        shortedShares -= amount;
        shortList.RemoveAt(index);
        print("bought back: " + amount);
        OrganizeShortLog();

        t_shortedAmount.text = "0";
        shortAmount = 0;
        t_shortedStocks.text = Math.Round(shortedShares, 6).ToString("N") + " shares";
        }

        CheckWin();
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
        if (ownedShares >= buyAmount)
        {
            buyPower += buyAmount * stocks.price;
            ownedShares -= buyAmount;
            stocks.shortInfluence += shortAmount / 100000;
            t_ownedStocks.text = Math.Round(ownedShares, 6).ToString("N") + " shares";
            print("sold: " + buyAmount);
            t_buyAmount.text = "0";
            buyAmount = 0;
        }
    }
    
    public void OrganizeShortLog(){
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
            log.transform.Find("days").GetComponent<TMP_Text>().text = shorts.daysCD.ToString();
            
            print("amount: " + shorts.amount.ToString() + " price: " + shorts.shortPrice.ToString());
            y_offset -= 1;
        }
    }

    public void BuyPR(){
        if (buyPower>= pricePR){
            buyPower -= pricePR;
            StartCoroutine(llm.BadPR());
            pricePR += 1000000;
            t_PR.text = "Buy PR (" + pricePR.ToString("C0") + ")";
        }
        
    }

    public void CalcPayout(){
        float dividendPay = ownedShares * stocks.price * dividendRatio;
        float shortInterest = shortedShares * stocks.price * shortInterestRatio;
        t_endScreen.text = 
                            "You shorted: " + shortedShares.ToString("N") + " shares" + "\n" +
                            "You bought: " + ownedShares.ToString("N") + " shares" + "\n\n" +
                            "Dividend: " + dividendPay.ToString("C") + "\n" +
                            "Short interest due: " + shortInterest.ToString("C") + "\n\n" +
                            "Keep up the good work.";
    }

    public void Payout(){
        float dividendPay = ownedShares * stocks.price * dividendRatio;
        float shortInterest = 0f;
        ///
        foreach(Shorts shorts in shortList){
            shortInterest += shorts.amount * shorts.shortPrice * shortInterestRatio;
        }
        ///
        buyPower += dividendPay;
        if(buyPower >= shortInterest){
            buyPower -= shortInterest;
        }
        else{
            Bankrupt();
        }
    }

    void CheckWin(){
        if(shortList.Count < 1){
            stocks.paused = true;
            winScreen.SetActive(true);
        }
    }

    void Bankrupt(){
        float debt = shortedShares * stocks.price - buyPower;
        stocks.isBankrupt = true;
        t_loseMsg.text = "Unfortunately, you've made some irrational descisions and directly led your investments into a big flop.\n\n" +
                         "Your loss cost us " + debt.ToString("C2") + 
                         "\nWe won't be needing you anymore.\n\n" +
                         "Make sure to return your badge on your way out.";
        loseScreen.SetActive(true);
    }
}
