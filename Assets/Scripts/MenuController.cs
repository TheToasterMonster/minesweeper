using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuController : MonoBehaviour
{
    bool showing;

    // Start is called before the first frame update
    void Start()
    {
        showing = false;
        GetComponent<SpriteRenderer>().enabled = showing;
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = showing;
        transform.GetChild(1).GetComponent<MeshRenderer>().enabled = showing;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            showing = !showing;
            GetComponent<SpriteRenderer>().enabled = showing;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = showing;
            transform.GetChild(1).GetComponent<MeshRenderer>().enabled = showing;
        }
    }
}
