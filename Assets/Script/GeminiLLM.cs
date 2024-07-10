using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class GeminiLLM : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    TakeScreenshotAndSave screenshot;

    float period = 0.0f, boughtPeriod = 0f;
    public SocialMedia socialMedia;

    public class ApiResponse
    {
        public string headline { get; set; }
        public string sentiment_score { get; set; }
    }

    static async Task<Post> GetResponse(string input)
    {
        var url = "https://stopthekitty.maxckm.com/api/v1/generate";
        try
        {
            using (var formData = new MultipartFormDataContent())
            {
                var prompt = input;
            
                formData.Add(new StringContent(prompt), "prompt");

                string imagePath = Application.dataPath + "/ScreenshotSave.png";
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                if(imageBytes!= null){
                    string base64Image = Convert.ToBase64String(imageBytes);
                    formData.Add(new StringContent(base64Image), "image");
                }

                var response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();


                var responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log("Response: " + responseBody);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                Debug.Log("Headline: " + apiResponse.headline);
                Debug.Log("Sentiment Score: " + apiResponse.sentiment_score);
                Post post = new Post(apiResponse.headline, float.Parse(apiResponse.sentiment_score));
                return post;
            }
        }
        
        catch (HttpRequestException e)
        {
            Debug.Log("Request error: " + e.Message);
            return null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        screenshot = (TakeScreenshotAndSave) GameObject.FindObjectOfType (typeof(TakeScreenshotAndSave));
        StartCoroutine(PeriodicUpdate());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator PeriodicUpdate()
    {
        while (true)
        {
            if (period > 4.0f)
            {
                StartCoroutine(screenshot.TakeSnapShotAndSave());
                Task<Post> getResponseTask = GetResponse(
                    "Your first task is WITHOUT using emojis, create an attention grabbing tweet under 80 characters about BBSE stock that contains strictly NO EMOJI and no hashtags, guessing about the future of the stock. Your second task is to check and remove all emojis in your response and store it as \"headline\" Your third task is to give that tweet a reasonable sentiment score that ranges between -1 to 1. You will output nothing other than this json format: {\"headline\":\"\"headline\"\",\"sentiment_score\":\"your_answer\"}");
                
                yield return new WaitUntil(() => getResponseTask.IsCompleted);

                if (getResponseTask.Result != null)
                {
                    socialMedia.posts.Add(getResponseTask.Result);
                    socialMedia.Organize();
                }

                period = 0.0f;
            }
            period += UnityEngine.Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator BadPR()
    {
        int duration = 30;
        while (duration > 0)
        {
            if (boughtPeriod > 3.0f)
            {
                StartCoroutine(screenshot.TakeSnapShotAndSave());
                Task<Post> getResponseTask = GetResponse(
                    "Your first task is WITHOUT using emojis, create an attention grabbing tweet under 80 characters about BBSE stock that contains strictly NO EMOJI and no hashtags, guessing negatively about the future of the stock. Your second task is to check and remove all emojis in your response and store it as \"headline\" Your third task is to give that tweet a reasonable sentiment score that ranges between -1 to 1. You will output nothing other than this json format: {\"headline\":\"\"headline\"\",\"sentiment_score\":\"your_answer\"}");
                
                yield return new WaitUntil(() => getResponseTask.IsCompleted);

                if (getResponseTask.Result != null)
                {
                    socialMedia.posts.Add(getResponseTask.Result);
                    socialMedia.Organize();
                }

                period = 0.0f;
                duration --;
            }
            boughtPeriod += UnityEngine.Time.deltaTime;
            yield return null;
        }
    }
}

