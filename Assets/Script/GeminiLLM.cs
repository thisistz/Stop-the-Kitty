using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class GeminiLLM : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();

    float period = 0.0f;
    public SocialMedia socialMedia;

    public class ApiResponse
    {
        public string headline { get; set; }
        public string sentiment_score { get; set; }
    }

    static async Task<Post> GetResponse()
    {
        var url = "https://stopthekitty.maxckm.com/api/v1/generate";

        try
        {
            using (var formData = new MultipartFormDataContent())
            {
                var prompt = "You are a Redditor called roaringkitty, your first task is Without using emojis, create an attention grabbing tweet under 80 characters about BBSE stock that contains strictly no emoji and no hashtags, guessing about the future of the stock, you might be pessimistic. Your second task is to check and remove all emojis in the headline. Your third task is to give that headline a reasonable sentiment score that ranges between -1 to 1. Output as json format after removing emojis. You will output the result with following json format:{\"headline\":\"your_answer\",\"sentiment_score\":\"your_answer\"}";
            
                formData.Add(new StringContent(prompt), "prompt");

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
                Task<Post> getResponseTask = GetResponse();
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
}

