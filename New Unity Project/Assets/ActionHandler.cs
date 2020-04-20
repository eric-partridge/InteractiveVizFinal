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
    public List<Stock[]> _stockData = new List<Stock[]>();

    Action()
    {
        _ID = "";
        _name = "";
        _text = "";
    }

    public Action(string id, string name, string text)
    {
        _ID = id;
        _name = name;
        _text = text;
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
        foreach (string line in lines)
        {
            string[] splitLine = line.Split('|');
            Action a = new Action(splitLine[0], splitLine[1], splitLine[2]);
            List<Stock[]> tempStockData = new List<Stock[]>();
            if (DataParse.windows)
            {
                current_directory = root_directory + "\\" + splitLine[1];
            }
            else
            {
                current_directory = root_directory + "//" + splitLine[1];
            }
            string[] csvFiles = Directory.GetFiles(current_directory);
            foreach (string file in csvFiles)
            {
                //print(file);
                a._stockData.Add(DataParse.Parse(file));
            }
            action_list.Add(a);
        }
    }

    public Action GetNewAction()
    {
        text_box.text = action_list[0]._text;
        return action_list[0];
    }
}
