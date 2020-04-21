using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public int skill_points = 0;
    public Text skillPointDisplay;
    private string displaySetup = "Skill Points: ";

    private void Awake()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        //skillPointDisplay.text = displaySetup + skill_points;
    }

    public void UpdateSkillPoints()
    {
        skill_points += Mathf.RoundToInt(Random.Range(0, 3));
        UpdateDisplay();
    }
}
