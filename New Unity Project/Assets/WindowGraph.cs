using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{

    //graph variables
    public float yMax;
    public Vector2 circleSize;
    public float lineWidth;
    public int seperatorCount;
    public float xLabelOffset;
    public float yLabelOffset;
    public Button nextDayButton;
    public Text currentPrice;
    public float startValue;

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private float graphWidth;
    private float graphHeight;
    private int day = 1;
    private List<float> valueList;
    private List<float> previousValues;
    private List<GameObject> dots;
    private List<GameObject> lines;
    private List<RectTransform> xAxisLabels;
    private GameObject previousCircleGO = null;
    private float currentValue;
    private bool redrawingGraph = false;

    private void Awake()
    {
        graphContainer = transform.Find("Graph Container").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialize list
        valueList = new List<float>();
        previousValues = new List<float>();
        dots = new List<GameObject>();
        lines = new List<GameObject>();
        xAxisLabels = new List<RectTransform>();

        //set data from NASDAQ as intitail data
        foreach (Stock s in DataParse.instance.stockList[2])
        {
            valueList.Add(float.Parse(s.Value));
        }

        //set up graph
        GraphSetup();

        //start with initial value of 50
        AddDataPoint(50f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject CreateCircle(Vector2 anchoredPos)
    {
        //draw a circle at desired location and return that object
        GameObject circle = new GameObject("circle", typeof(Image));
        circle.transform.SetParent(graphContainer, false);
        circle.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = circleSize;
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return circle;
    }

    private void GraphSetup(Func<float, string> getAxisLabelY = null)
    {
        //setup labels on y axis
        for(int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedVal = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(yLabelOffset, normalizedVal * graphHeight);
            labelY.GetComponent<Text>().text = "$" + (normalizedVal * yMax).ToString();
        }
    }

    private void AddDataPoint(float value, int i)
    {
        currentValue = value;
        previousValues.Add(value);

        //calculate x and y position
        float xSize = graphWidth / 15 - 1;
        float xPos;

        if (redrawingGraph) { xPos = xSize + (i - 1) * xSize; }
        else
        {
            if(day > 15) { xPos = xSize + (14) * xSize; }
            else { xPos = xSize + (day - 1) * xSize; }
        }

        float yPos = (value/ yMax) * graphHeight;

        //draw circle at that positon
        GameObject circleGO = CreateCircle(new Vector2(xPos, yPos));
        if (previousCircleGO != null)
        {
            CreateDotConnection(previousCircleGO.GetComponent<RectTransform>().anchoredPosition, circleGO.GetComponent<RectTransform>().anchoredPosition);
        }
        dots.Add(circleGO);
        previousCircleGO = circleGO;

        if (!redrawingGraph)
        {
            if(day <= 15)
            {
                //add x axis label
                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPos, xLabelOffset);
                labelX.GetComponent<Text>().text = day.ToString();
                xAxisLabels.Add(labelX);
            }
            else
            {
                xAxisLabels[14].GetComponent<Text>().text = day.ToString();
            }

        }

        currentPrice.text = "Price: $" + value.ToString();
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        //create line connecting two dots
        GameObject line = new GameObject("dotConnenction", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        Vector2 diff = dotPositionB - dotPositionA;
        float sign = (dotPositionB.y < dotPositionA.y) ? -1.0f : 1.0f;
        rectTransform.localEulerAngles =  new Vector3(0, 0, Vector2.Angle(Vector2.right, diff)*sign);

        lines.Add(line);
    }

    public void NextDay()
    {
        day += 1;
        if (day > 15) 
        { 
            ClearGraph();
            RedrawGraph();
        }

        AddDataPoint(GetNewValue(), 0);

        if(day == valueList.Count) { nextDayButton.interactable = false; }
    }

    private float GetNewValue()
    {
        float percentChange = ((valueList[day - 1] - valueList[0]) / valueList[0]);
        if(percentChange < 0f)
        {
            float newValue = (int)((50 + (50 * percentChange)) * 100.0f) / 100.0f;
            return newValue;
        }
        else if(percentChange > 0f)
        {
            float newValue = (int)((50 * (1f + percentChange)) * 100.0f) / 100.0f;
            return newValue;
        }
        else
        {
            return currentValue;
        }
    }

    private void ClearGraph()
    {
        //remove dots
        foreach(GameObject x in dots) { Destroy(x); }

        //remove connecting lines
        foreach(GameObject x in lines) { Destroy(x); }

        previousCircleGO = null;
    }

    private void RedrawGraph()
    {
        redrawingGraph = true;
        for(int i = 1; i < 15; i++)
        {
            AddDataPoint(previousValues[day - 15 + i], i);
            xAxisLabels[i-1].GetComponent<Text>().text = (day - 15 + i).ToString();
        }
        redrawingGraph = false;
    }
}
