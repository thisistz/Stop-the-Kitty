using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post{
    public string headline { get; set; }
    public float sentiment { get; set; }
    public int upvotes, downvotes;

    public Post(string line, float score){
        headline = line;
        sentiment = score;
        upvotes = (int)(1000*(sentiment*sentiment));
        downvotes = (int)(1000*(1-sentiment)*(1-sentiment));
    }
}
