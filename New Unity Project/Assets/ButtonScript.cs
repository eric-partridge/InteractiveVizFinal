using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [HideInInspector] public int cost = 0;

    private void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(unlockAction);
    }

    public void unlockAction()
    {
        Debug.Log("unlocked");
    }
}
