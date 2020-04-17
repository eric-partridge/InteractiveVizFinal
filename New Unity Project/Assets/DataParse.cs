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

    public string current_directory;
    public List<Stock[]> stockList;

    // Start is called before the first frame update

    void Start()
    {
        stockList = new List<Stock[]>();
        ParseAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ParseAll()
    {
        var engine = new FileHelperEngine<Stock>();
        current_directory = Directory.GetCurrentDirectory();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
            print(file);
            var result = engine.ReadFile(file);
            stockList.Add(result);
        }
    }
}
