﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSpawn : MonoBehaviour
{
    public GameObject button;
    public GameObject treeBox;
    public float spacing = 25;
    private float right = -400;
    private float up = 300;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        GameObject b = Instantiate(button);
        b.transform.parent = treeBox.transform;
        RectTransform r = b.GetComponent<RectTransform>();
        r.localScale = Vector3.one;
        r.localPosition = new Vector3(right, up, 0);
        up -= 50 + spacing;
        if(up < -300)
        {
            up = 300;
            right += 200 + spacing;
        }
    }
}
