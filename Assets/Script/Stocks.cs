using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stocks : MonoBehaviour
{
    public float price = 1.0f;
    float period = 0f, marketTime = 0f, shortInfluencePeriod = 0f;

    public List<float> stockPrices = new List<float>();
    public WindowGraph graph;
    public TMP_Text t_stockPrice;
    public float priceJump = 0.0f, shortInfluence = 0.0f;
    SocialMedia socialMedia;
    DailySystem dailySystem;
    List<Post> postRef = new List<Post>();
    public bool paused = true;
    // Start is called before the first frame update
    void Start()
    {
        socialMedia = (SocialMedia)FindObjectOfType(typeof(SocialMedia));
        dailySystem = (DailySystem)FindObjectOfType(typeof(DailySystem));
        paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused){
            PriceCalc();
        }
    }

    void PriceCalc(){
        if (marketTime < 1560.0f){
            if (period > 4.0f) //every 4 seconds = 1 minute in real world. stock market opens 390 minutes every day
            {
                postRef = socialMedia.GetPosts();
                float logShort = 0;
                if(postRef.Count >0)
                    priceJump = (postRef[postRef.Count -1].sentiment)/2;
                stockPrices.Add(price);
                float probabilty = 0;
                if(priceJump < 0){
                    probabilty = UnityEngine.Random.Range(1.5f * priceJump, -priceJump/1.2f);
                }
                else{
                    probabilty = UnityEngine.Random.Range(-priceJump/1.2f, priceJump);
                }

                if(shortInfluencePeriod > 20.0f){
                    shortInfluence = 0f;
                    shortInfluencePeriod = 0;
                }
                if(shortInfluence > 0) {
                    logShort = - Mathf.Log(Mathf.Abs(price * shortInfluence + 1), 1/20);
                }
                else{
                    logShort = Mathf.Log(Mathf.Abs(price * shortInfluence + 1), 1/20);
                }

                price += 10 * probabilty * probabilty * probabilty + logShort;
                price = Mathf.Max(price, 0.01f);
                price = (float)Math.Round(price, 2);
                t_stockPrice.text = price.ToString("C");
                graph.ShowGraph(stockPrices);
                period = 0;
            }
        }
        else{
            dailySystem.EndDay();
            marketTime = 0f;
            print("Day End");
        }
        
        period += UnityEngine.Time.deltaTime;
        marketTime += UnityEngine.Time.deltaTime;
        shortInfluencePeriod += UnityEngine.Time.deltaTime;
        
    }
}

