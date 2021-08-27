using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BingoTile : MonoBehaviour
{
    int localNumber;
    public TMP_Text myDisplayedNumber;

    public void UpdateLocalValue(int number)
    {
        localNumber = number;
        myDisplayedNumber.text = number.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
