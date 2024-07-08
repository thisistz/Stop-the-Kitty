using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailySystem : MonoBehaviour
{
    public int dayCount = 1;
    Stocks stocks;
    // Start is called before the first frame update
    void Start()
    {
        stocks = (Stocks)FindObjectOfType(typeof(Stocks));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            PauseDay();
        }
    }

    public void StartDay(){
        //start graph
        //enable trades
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
        //reset graph
        //disable trades
        //record last price
        //for each shorts in portfolio, days --, collect interest
    }
}
