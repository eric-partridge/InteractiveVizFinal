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
    public GameObject dataReader;

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private float graphWidth;
    private float graphHeight;
    private int day = 1;
    private List<float> valueList;
    private List<float> actionValueList;
    private List<float> previousValues;
    private List<GameObject> dots;
    private List<GameObject> lines;
    private List<RectTransform> xAxisLabels;
    private GameObject previousCircleGO = null;
    private float currentValue;
    private bool redrawingGraph = false;
    private bool actionComplete = true;
    private Action action;
    private ActionHandler actionHandlerScript;
    private int actionDay = 2;

    private PlayerActions player;

    private void Awake()
    {
        graphContainer = transform.Find("Graph Container").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
        actionHandlerScript = dataReader.GetComponent<ActionHandler>();
        player = GameObject.Find("Player").GetComponent<PlayerActions>();
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
        actionValueList = new List<float>();

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

        if (!redrawingGraph) { previousValues.Add(value); }

        //calculate xsize
        float xSize = graphWidth / 15;
        float xPos;

        //calculate x position
        if (redrawingGraph) { xPos = xSize + (i - 1) * xSize; }
        else
        {
            if(day > 15) { xPos = xSize + (14) * xSize; }
            else { xPos = xSize + (day - 1) * xSize; }
        }

        //calculate y position
        float yPos = (value/ yMax) * graphHeight;

        //draw circle at that positon and at it to list for managing later
        GameObject circleGO = CreateCircle(new Vector2(xPos, yPos));
        if (previousCircleGO != null)
        {
            CreateDotConnection(previousCircleGO.GetComponent<RectTransform>().anchoredPosition, circleGO.GetComponent<RectTransform>().anchoredPosition);
        }
        dots.Add(circleGO);
        previousCircleGO = circleGO;


        if (!redrawingGraph)
        {
            //if less than day 15, create new label and assign text to day value
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
            //over day 15, label already exists, simply change text
            else
            {
                xAxisLabels[14].GetComponent<Text>().text = day.ToString();
            }
        }

        //update current price text at top
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
        if (!actionComplete) { actionDay += 1; }

        CheckToGetAction();

        //if reached end of the list, button is no longer interactable
        if (day >= valueList.Count && actionComplete) { 
            nextDayButton.interactable = false;
            return;
        }
        //whenever over day 15, must redraw graph each time
        if (day > 15) 
        { 
            ClearGraph();
            RedrawGraph();
        }

        AddDataPoint(GetNewValue(), 0);

        player.UpdateSkillPoints();
    }

    private float GetNewValue()
    {
        float percentChange = 0f;

        //calculate percent change
        if (actionComplete) { percentChange = ((valueList[day - 1] - valueList[day-2]) / valueList[day-2]); }
        else { percentChange = ((actionValueList[actionDay - 1] - actionValueList[actionDay-2]) / actionValueList[actionDay-2]);  }

        //multiply it to emphasize change
        percentChange *= UnityEngine.Random.Range(2.75f, 3.75f);

        //calulate new value based on percent change
        if(percentChange < 0f)
        {
            float newValue = (int)((currentValue + (currentValue * percentChange)) * 100.0f) / 100.0f;
            return newValue;
        }
        else if(percentChange > 0f)
        {
            float newValue = (int)((currentValue * (1f + percentChange)) * 100.0f) / 100.0f;
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

        //set previous circle to null
        previousCircleGO = null;
    }

    private void RedrawGraph()
    {
        redrawingGraph = true;

        //redraw first 14 points
        for(int i = 1; i < 15; i++)
        {
            AddDataPoint(previousValues[day - 15 + i -1], i);
            xAxisLabels[i-1].GetComponent<Text>().text = (day - 15 + i).ToString();
        }

        redrawingGraph = false;
    }

    private void CheckToGetAction()
    {
        //gone through all data for an action, reset action variables
        if(actionDay == actionValueList.Count)
        {
            actionHandlerScript.ResetText();
            actionValueList.Clear();
            actionComplete = true;
            action = null;
            actionDay = 2;
        }

        //dont check for new action if you are already using one
        if (actionComplete)
        {
            float x = UnityEngine.Random.Range(0f, 1f);
            if (x < .1f)
            {
                actionHandlerScript.updateWeights(0.1f);
                action = actionHandlerScript.GetNewAction();
                foreach(Stock s in action._stockData[0])
                {
                    actionValueList.Add(float.Parse(s.Value));
                }
                actionComplete = false;
            }
        }
    }
}
