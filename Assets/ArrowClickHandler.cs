using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowClickHandler : MonoBehaviour
{
    public GameObject destination;

    [Multiline]
    public string text;

    void Start()
    {
        var destinationObjectTextChild = destination.transform.GetChild(0);
        var textMesh = destinationObjectTextChild.GetComponent<TextMesh>();
        textMesh.text = text;
        destination.SetActive(false);
    }

    void OnMouseDown()
    {
        destination.SetActive(true);
    }
}
