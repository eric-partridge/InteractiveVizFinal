using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int cost = 0;
    [HideInInspector] public string ID = "";
    private ActionHandler ActionHandlerScript;
    private Button btn;
    private PlayerActions player;
    private Text errorDiplay;
    private Text btnText;
    private bool mouse_over;
    private string init_text;

    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(unlockAction);
        ActionHandlerScript = GameObject.Find("DataReader").GetComponent<ActionHandler>();
        player = GameObject.Find("Player").GetComponent<PlayerActions>();
        errorDiplay = GameObject.Find("DialogueBox").GetComponent<Text>();
        btnText = btn.GetComponentInChildren<Text>();
        init_text = btnText.text;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
        Debug.Log("Mouse enter");
        btnText.text = cost.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        Debug.Log("Mouse exit");
        btnText.text = init_text;
    }
}
