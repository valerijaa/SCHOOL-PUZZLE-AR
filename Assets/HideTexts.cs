using UnityEngine;
using UnityEngine.UI;

public class HideTexts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var textsContainer = GameObject.Find("Texts");

        // hide and reset texts for all containers
        var factsContainer = textsContainer.transform.Find("FactContainer");
        factsContainer.gameObject.SetActive(false);
        factsContainer.gameObject.GetComponentInChildren<Text>().text = "";

        var navigationContainer = textsContainer.transform.Find("NavigationContainer");
        navigationContainer.gameObject.SetActive(false);
        navigationContainer.gameObject.GetComponentInChildren<Text>().text = "";

        var winContainer = textsContainer.transform.Find("WinContainer");
        winContainer.gameObject.SetActive(false);
    }
}
