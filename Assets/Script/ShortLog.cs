using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShortLog : MonoBehaviour
{
    Stocks stocks;
    float valueGrowth;
    string str_growth;
    TMP_Text t_amount, t_price, t_percentage, t_days;
    // Start is called before the first frame update
    void Start()
    {
        stocks = (Stocks)FindObjectOfType(typeof(Stocks));
        t_amount = this.transform.Find("amount").GetComponent<TMP_Text>();
        t_price = this.transform.Find("price").GetComponent<TMP_Text>();
        t_percentage = this.transform.Find("percentage").GetComponent<TMP_Text>();
        t_days = this.transform.Find("days").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        valueGrowth = 1-(stocks.price/float.Parse(t_price.text));
        if(valueGrowth > 0){
            str_growth = "+" + valueGrowth.ToString("0.00") + "%";
        }
        else if(valueGrowth < 0){
            str_growth = "-" + valueGrowth.ToString("0.00") + "%";
        }
        else{str_growth = valueGrowth.ToString("0.00") + "%";}

        t_percentage.text = str_growth;
    }

    public void BuyBackShort(TMP_Text amount){
        float bbAmount = float.Parse(amount.text);
        int index = transform.GetSiblingIndex();
        GameObject portfolio = GameObject.Find("Portfolio Manager");
        portfolio.GetComponent<Portfolio>().BuyBackShort(bbAmount, index);
    }
}
