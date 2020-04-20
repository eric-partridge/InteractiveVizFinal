using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using FileHelpers;
using System.Runtime.InteropServices;

[DelimitedRecord(",")]
public class Stock
{
	public string Symbol;

	public string Date;

	public string Value;
}

public class DataParse : MonoBehaviour
{

    [HideInInspector] public string current_directory;
    public List<Stock[]> stockList;
    public static bool windows = false;
    public static DataParse instance;

    private void Awake()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            windows = true;
        }
    }

    void Start()
    {
        instance = this;
        stockList = new List<Stock[]>();
        ParseAll();
    }

    void Update()
    {

    }

    void ParseAll()
    {
        var engine = new FileHelperEngine<Stock>();
        current_directory = Directory.GetCurrentDirectory();
        if (windows)
        {
            current_directory += "\\Data";
        }
        else
        {
            current_directory += "//Data";
        }
        string[] files = Directory.GetFiles(current_directory);
        foreach (string file in files)
        {
            var result = engine.ReadFile(file);
            stockList.Add(result);
        }
    }

    public static Stock[] Parse(string path)
    {
        var engine = new FileHelperEngine<Stock>();
        var result = engine.ReadFile(path);
        return result;
    }
}
