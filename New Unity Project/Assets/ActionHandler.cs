using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Action
{
    public string _ID;
    public string _name;
    public string _text;
    public float _weight;
    private float initWeight;
    private float time = 1f;
    private bool locked = false;
    private int cost = 0;
    public List<Stock[]> _stockData = new List<Stock[]>();

    Action()
    {
        _ID = "";
        _name = "";
        _text = "";
        initWeight = 0.2f;
        _weight = initWeight;
        locked = false;
        cost = 0;
    }

    public Action(string id, string name, string text, float weight, bool isLocked, int aCost)
    {
        _ID = id;
        _name = name;
        _text = text;
        initWeight = weight;
        _weight = initWeight;
        locked = isLocked;
        cost = aCost;
    }

    //speed is a value between 0 and 1 that gets added to time. Reccomended 0.1 speed
    public void updateWeight(float speed)
    {
        if(_weight < initWeight)
        {
            _weight = Mathf.Lerp(_weight, initWeight, time);
            time += speed;
            if(time > 1) { time = 1; }
        }
        if(_weight > initWeight)
        {
            _weight = initWeight;
        }
    }

    public bool isLocked()
    {
        return locked;
    }

    public void Unlock()
    {
        locked = false;
    }

    public int getCost()
    {
        return cost;
    }
}

public class ActionHandler : MonoBehaviour
{
    [HideInInspector] public string text_directory;
    [HideInInspector] public string root_directory;
    [HideInInspector] public string current_directory;
    public Text text_box;
    public List<Action> action_list;

    // Start is called before the first frame update
    void Start()
    {
        action_list = new List<Action>();
        GetActions();
        text_box.text = "The market is currently normal";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            text_box.text = action_list[0]._text;
        }
    }

    void GetActions()
    {
        text_directory = Directory.GetCurrentDirectory();
        root_directory = text_directory;
        if (DataParse.windows)
        {
            root_directory += "\\Data";
            text_directory += "\\Text";
        }
        else
        {
            root_directory += "//Data";
            text_directory += "//Text";
        }
        string[] files = Directory.GetFiles(text_directory);
        string[] lines = File.ReadAllLines(files[0]);
        //line is ID|Folder|Description|Weight
        foreach (string line in lines)
        {
            string[] splitLine = line.Split('|');
            float w = float.Parse(splitLine[3]);
            int cost = int.Parse(splitLine[5]);
            bool locked = false;
            if(splitLine[4] == "y")
            {
                locked = true;
                GameObject button = ButtonSpawn.instance.Spawn();
                button.GetComponentInChildren<Text>().text = splitLine[1];
                button.GetComponent<ButtonScript>().cost = cost;
                button.GetComponent<ButtonScript>().ID = splitLine[0];

            }
            Action a = new Action(splitLine[0], splitLine[1], splitLine[2], w, locked, cost);
            if (DataParse.windows)
            {
                current_directory = root_directory + "\\" + splitLine[1];
            }
            else
            {
                current_directory = root_directory + "//" + splitLine[1];
            }
            string[] csvFiles = Directory.GetFiles(current_directory);
            //print(file);
            //only parse the first file in each dir
            a._stockData.Add(DataParse.Parse(csvFiles[0]));
            action_list.Add(a);
        }
    }

    //t is time from update weight
    public void updateWeights(float t)
    {
        foreach(Action a in action_list)
        {
            a.updateWeight(t);
        }
    }

    public Action FindByID(string ID)
    {
        Action ret = null;
        foreach(Action a in action_list)
        {
            if(a._ID == ID)
            {
                ret = a;
                break;
            }
        }
        if(ret == null)
        {
            Debug.Log("Could Not Find ID: " + ID);
        }
        return ret;
    }

    public Action GetNewAction()
    {
        Action selected = action_list[0];
        float w = selected._weight;
        for(int i = 1; i < action_list.Count; i++)
        {
            if (!action_list[i].isLocked())
            {
                float x = action_list[i]._weight;
                float keepChance = Random.Range(0, w / (w + x));
                float nextChance = Random.Range(0, x / (w + x));
                if (keepChance > nextChance)
                {
                    break;
                }
                else
                {
                    selected = action_list[i];
                    w += selected._weight;
                }
            }
        }
        text_box.text = selected._text;
        selected._weight = 0;
        return selected;
    }

    public void ResetText()
    {
        text_box.text = "The stock market is back to normal now";
    }
}
