using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    public Timer timer;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed " + LocalGameManager.Instance.newPulledValue);
            LocalGameManager.Instance.Click(LocalGameManager.Instance.newPulledValue);
        }
    }
}
