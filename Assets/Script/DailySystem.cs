using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailySystem : MonoBehaviour
{
    public int dayCount = 1;
    public GameObject tutorialScreen;
    public GameObject dayEndScreen;
    public TMP_Text t_days;
    Stocks stocks;
    Portfolio portfolio;
    // Start is called before the first frame update
    void Start()
    {
        stocks = (Stocks)FindObjectOfType(typeof(Stocks));
        portfolio = (Portfolio)FindObjectOfType(typeof(Portfolio));
        tutorialScreen.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            EndDay();
        }
    }

    public void StartDay(){
        //start graph
        stocks.paused = false;
        //enable trades
        tutorialScreen.SetActive(false);

        t_days.text = "Day " + dayCount;
    }

    public void PauseDay(){
        stocks.paused = !stocks.paused;
        print("pause status: " + stocks.paused);
        //disable trades
        var foundBtn = FindObjectsOfType<Button>(false);
        foreach(var btn in foundBtn){
            Button button = btn.GetComponent<Button>();
            
        }
    }

    public void EndDay(){
        //pause graph
        stocks.paused = true;
        //disable trades
        dayEndScreen.SetActive(true);
        //record last price
        //for each shorts in portfolio, days --, collect interest
    }

    public void NextDay(){
        //clear graph
        stocks.stockPrices.Clear();
        stocks.graph.ClearGraph();
        //new day
        dayCount ++;
        StartDay();
        //update short countdown
        foreach(Shorts s in portfolio.shortList){
            s.daysCD --;
        }
        
        dayEndScreen.SetActive(false);
    }
}
