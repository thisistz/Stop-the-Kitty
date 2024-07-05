using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SocialMedia : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    public List<Post> posts = new List<Post>();

    void Start()
    {
        Organize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public List<Post> GetPosts()
    {
        return posts;
    }
    public void Organize(){
        int y_offset = posts.Count - 1;

        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (Post post in posts){
            GameObject row = Instantiate(prefab, transform);
            row.GetComponent<RectTransform>().localPosition -=new Vector3(0, (y_offset * 25),0);
            row.transform.Find("Headline").GetComponent<TMP_Text>().text = post.headline;
            row.transform.Find("Sentiment Score").GetComponent<TMP_Text>().text = post.sentiment.ToString();
            row.transform.Find("Votes").Find("Upvote").GetComponent<TMP_Text>().text = post.upvotes.ToString();
            row.transform.Find("Votes").Find("Downvote").GetComponent<TMP_Text>().text = post.downvotes.ToString();
            y_offset -= 1;
        }
    }
}
