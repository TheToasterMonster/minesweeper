using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Square : MonoBehaviour
{
    public event Action<int, int> onClick;
    public GameObject flagPrefab;

    private GameObject flagSprite;

    private int i, j;
    private bool isFlagged;
    private bool isClicked;
    
    public static int flagCount;
    public static bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        float x = transform.position.x - transform.localScale.x - transform.localScale.x / 20;
        while (x > -Camera.main.orthographicSize * Camera.main.aspect)
        {
            x -= (transform.localScale.x + transform.localScale.x / 20);
            i++;
        }
        float y = transform.position.y - transform.localScale.y - transform.localScale.y / 20;
        while (y > -Camera.main.orthographicSize)
        {
            y -= (transform.localScale.y + transform.localScale.y / 20);
            j++;
        }

        Transform text = transform.GetChild(0);
        text.position = transform.position;
        text.localScale = transform.localScale;

        isFlagged = false;
        isClicked = false;

        flagSprite = Instantiate(flagPrefab, transform.position, Quaternion.identity);
        flagSprite.transform.localScale = transform.localScale * 1.1f;
        flagSprite.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void OnMouseOver()
    {
        if (gameOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isFlagged && !isClicked)
        {
            onClick(i, j);
            isClicked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && !isClicked)
        {
            if (flagCount <= 0 && !isFlagged)
            {
                return;
            }

            isFlagged = !isFlagged;
            flagSprite.GetComponent<SpriteRenderer>().enabled = isFlagged;
            flagCount += isFlagged ? -1 : 1;
        }
    }

    public bool getIsFlagged()
    {
        return isFlagged;
    }
}
