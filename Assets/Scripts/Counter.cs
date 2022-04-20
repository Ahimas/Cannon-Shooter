using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Text CounterText;

    private int Count = 0;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Enemy") )
        {
            Count += 1;
            CounterText.text = "Count : " + Count;
            gameManager.enemiesInGame -= 1;
         
            if ( other.transform.parent != null )
            {
                other.transform.parent.gameObject.SetActive(false);
            } else
            {
                other.gameObject.SetActive(false);
            }

        }
        

        
    }
}
