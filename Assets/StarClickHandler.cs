using UnityEngine;
using UnityEngine.UI;

public class StarClickHandler : MonoBehaviour
{
    public Color ActiveColor;

    public Stop StopData; // is set in CloudTrackableEventHandler

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(); // default value

        // when on android we want to use get touch position
        if (Application.platform == RuntimePlatform.Android)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        }
        // we want to use mouse position, used for development & quick testing
        else if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit))
        {
            Debug.Log(Hit.transform.tag);
            // we want to handle input only on 'Star' object
            if (Hit.transform.tag != "Star")
                return;

            // update styles, text and score only if arrow wasn't already scored
            if (!scoreKeeper.IsStepScored(StopData.VuforiaName))
            {
                StyleAsScored(this.transform.parent.gameObject);

                // set and enable navigation text
                var textsContainer = GameObject.Find("Texts");
                var navigationContainer = textsContainer.transform.Find("NavigationContainer");
                navigationContainer.GetComponentInChildren<Text>().text = StopData.Data;
                navigationContainer.gameObject.SetActive(true);

                // show fact
                var factContainer = textsContainer.transform.Find("FactContainer");
                factContainer.gameObject.GetComponentInChildren<Text>().text = StopData.Fact;
                factContainer.gameObject.SetActive(true);

                // update score
                scoreKeeper.AddScore(StopData.VuforiaName);
            }
        }
    }

    public void StyleAsScored(GameObject pointer)
    {
        var star = pointer.transform.GetChild(1);
        var arrow = pointer.transform.GetChild(0);
        Destroy(arrow.gameObject); // destroy arrow object

        // change color of star
        var rend = star.GetComponent<Renderer>();
        rend.material.SetColor("_Color", ActiveColor);
    }
}
