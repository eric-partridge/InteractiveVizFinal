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

    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(unlockAction);
        ActionHandlerScript = GameObject.Find("DataReader").GetComponent<ActionHandler>();
    }

    public void unlockAction()
    {
        Action a = ActionHandlerScript.FindByID(ID);
        a.Unlock();
        btn.interactable = false;
    }
}
