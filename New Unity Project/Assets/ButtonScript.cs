using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [HideInInspector] public int cost = 0;
    [HideInInspector] public string ID = "";
    private ActionHandler ActionHandlerScript;
    private Button btn;
    private PlayerActions player;
    private Text errorDiplay;

    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(unlockAction);
        ActionHandlerScript = GameObject.Find("DataReader").GetComponent<ActionHandler>();
        player = GameObject.Find("Player").GetComponent<PlayerActions>();
        errorDiplay = GameObject.Find("DialogueBox").GetComponent<Text>();
    }

    public void unlockAction()
    {
        if (player.skill_points >= cost)
        {
            Action a = ActionHandlerScript.FindByID(ID);
            a.Unlock();
            btn.interactable = false;
            player.skill_points -= cost;
            player.UpdateDisplay();
        }
        else
        {
            errorDiplay.text = "Not enough skill points.";
        }
    }
}
