using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BingoTile : MonoBehaviour
{
    int localNumber;
    public Image image;
    public TMP_Text myDisplayedNumber;

    public void MarkAsPressed()
    {
        print(GetComponentInChildren<Image>().name);
        image.color = Color.cyan;
        myDisplayedNumber.color = Color.white;
    }

    public void UpdateLocalValue(int number)
    {
        localNumber = number;
        myDisplayedNumber.text = number.ToString();
    }

    public void UpdateLocalName(string newName)
    {
        myDisplayedNumber.text = newName;
    }

    public void ClickOnTile()
    {
        Debug.Log("Clicked on " + localNumber);
        LocalGameManager.Instance.Click(localNumber);
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
