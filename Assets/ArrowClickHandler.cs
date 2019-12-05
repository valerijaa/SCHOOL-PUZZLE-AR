using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArrowClickHandler : MonoBehaviour
{
    GameObject arrow;
    GameObject textInstructions;

    public Color ActiveArrowColor;

    public Stop StopData; // is set in CloudTrackableEventHandler

    // Start is called before the first frame update
    void Start()
    {
        textInstructions = GameObject.Find("Instruction");
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
            if (Hit.transform.tag != "Arrow")
                return;

            // update styles, text and score only if arrow wasn't already scored
            if (!scoreKeeper.IsStepScored(StopData.VuforiaName))
            {
                StyleAsScored(this.gameObject);
                textInstructions.GetComponentInChildren<Text>().text = StopData.Data;
                scoreKeeper.AddScore(StopData.VuforiaName);
            }
        }
    }

    public void StyleAsScored(GameObject arrow)
    {

        // TODO: check if current tracking image is already marked as scored
        var child = arrow.transform.GetChild(0);
        var child_2 = arrow.transform.GetChild(1);

        var rend = child.GetComponent<Renderer>();
        var rend_2 = child_2.GetComponent<Renderer>();

        Destroy(arrow.GetComponent<Animator>());

        rend.material.SetColor("_Color", ActiveArrowColor);
        rend_2.material.SetColor("_Color", ActiveArrowColor);
    }
}
