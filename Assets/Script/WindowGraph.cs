using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private List<GameObject> circles = new List<GameObject>();
    private List<GameObject> connections = new List<GameObject>();
    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        
    }

    private void CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image) );
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform. anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3, 3);
        rectTransform. anchorMin = new Vector2(0, 0);
        rectTransform. anchorMax = new Vector2(0, 0);
        circles.Add(gameObject);
    }

    public void ClearGraph() {
        foreach(GameObject c in circles) {
            Destroy(c);
        }
        foreach(GameObject c in connections) {
            Destroy(c);
        }
        circles.Clear();
        connections.Clear();
    }

    public void ShowGraph(List<float> value){
        ClearGraph();
        float xSize = graphContainer.rect.width/(13 * 30);
        float yMax = (float)Math.Round(value.Max()+1, 2);
        float graphHeight = graphContainer.sizeDelta.y - 50;
        for(int i = 0; i < value.Count; i++){
            float xPos = xSize + i * xSize;
            float yPos = (value[i]/yMax) * graphHeight;
            CreateCircle(new Vector2(xPos,yPos));
        }
        if(circles.Count > 1){
            for(int i = 0; i < circles.Count - 1; i++) {
                DotConnection(circles[i].GetComponent<RectTransform>().anchoredPosition, circles[i+1].GetComponent<RectTransform>().anchoredPosition);
            }
        }
    }

    private void DotConnection(Vector2 startPos, Vector2 endPos){
        GameObject gameObject = new GameObject("dotConnection", typeof (Image) );
        gameObject. transform.SetParent(graphContainer, false);
        Color barColor = new Color(0, 0, 0);
        if(startPos.y > endPos.y){
            barColor = new Color(1,0.2f,0, 1f);
        }
        else{
            barColor = new Color(0,1,0.2f, 1f);
        }
        gameObject.GetComponent<Image>().color = barColor;
        Vector2 dir = (endPos - startPos).normalized;
        float dist = Vector2.Distance(startPos, endPos);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform. anchorMin = new Vector2(0, 0);
        rectTransform. anchorMax = new Vector2(0, 0);
        rectTransform. sizeDelta = new Vector2(graphContainer.rect.width/(13 * 30), dist);
        rectTransform. anchoredPosition = startPos + dir * dist * 0.5f;
        //gameObject.transform.Rotate(new Vector3(0,0,  - Vector2.Angle(endPos,startPos)));
        connections.Add(gameObject);
    }
}
