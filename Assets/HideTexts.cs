using UnityEngine;
using UnityEngine.UI;

public class HideTexts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var textsContainer = GameObject.Find("Texts");

        // hide and reset texts for all containers
        ResetTextAndDisable(textsContainer.transform.Find("FactContainer"));
        ResetTextAndDisable(textsContainer.transform.Find("NavigationContainer"));
        ResetTextAndDisable(textsContainer.transform.Find("WinContainer"));
    }

    private void ResetTextAndDisable(Transform transform)
    {
        transform.gameObject.SetActive(false);
        transform.gameObject.GetComponentInChildren<Text>().text = "";
    }
}
