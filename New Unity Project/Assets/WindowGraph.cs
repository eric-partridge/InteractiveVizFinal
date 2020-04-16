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

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private float graphWidth;
    private float graphHeight;
    private int day = 0;
    List<float> valueList;
    private GameObject previousCircleGO = null;

    private void Awake()
    {
        graphContainer = transform.Find("Graph Container").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;

        valueList = new List<float>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 36, 33 };
        GraphSetup((float _f) => "$" + Mathf.RoundToInt(_f));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject CreateCircle(Vector2 anchoredPos)
    {
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
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        for(int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedVal = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(yLabelOffset, normalizedVal * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(normalizedVal * yMax);
        }
    }

    private void AddDataPoint()
    {
        float xSize = graphWidth / valueList.Count - 1;
        float xPos = xSize + (day-1) * xSize;
        float yPos = (valueList[day-1] / yMax) * graphHeight;
        GameObject circleGO = CreateCircle(new Vector2(xPos, yPos));
        if (previousCircleGO != null)
        {
            CreateDotConnection(previousCircleGO.GetComponent<RectTransform>().anchoredPosition, circleGO.GetComponent<RectTransform>().anchoredPosition);
        }
        previousCircleGO = circleGO;

        RectTransform labelX = Instantiate(labelTemplateX);
        labelX.SetParent(graphContainer);
        labelX.gameObject.SetActive(true);
        labelX.anchoredPosition = new Vector2(xPos, xLabelOffset);
        labelX.GetComponent<Text>().text = day.ToString();

        currentPrice.text = "Price: $" + valueList[day - 1].ToString();
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
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

    }

    public void NextDay()
    {
        day += 1;
        AddDataPoint();
        if(day == valueList.Count)
        {
            nextDayButton.interactable = false;
        }
    }
}
