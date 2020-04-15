using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using FileHelpers;

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

    // Start is called before the first frame update

    void Start()
    {
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
        current_directory += "\\Data";
        string[] files = Directory.GetFiles(current_directory);
        foreach (string file in files)
        {
            print(file);
            var result = engine.ReadFile(file);
        }
    }
}
