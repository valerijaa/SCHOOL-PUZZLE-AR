using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Cock : MonoBehaviour
{
    string btnName;
    public GameObject arrow;
    public Color active;
    public GameObject canvas;
    public bool isSet = false;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvas.GetComponentInChildren<Text>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit))
        {
            Debug.Log("pizdiec");
            btnName = Hit.transform.tag;
            var arrows = GameObject.FindGameObjectsWithTag(btnName);
            arrow = arrows.SingleOrDefault(x => x.gameObject == Hit.transform.gameObject);
            Color active = new Color(0.0078f, 0.74901f, 0.0078f);


            var child = arrow.transform.GetChild(0);
            var child_2 = arrow.transform.GetChild(1);



            var rend = child.GetComponent<Renderer>();
            var rend_2 = child_2.GetComponent<Renderer>();

            Destroy(arrow.GetComponent<Animator>());
            rend.material.SetColor("_Color", active);
            rend_2.material.SetColor("_Color", active);
            var text = canvas.GetComponentInChildren<Text>();
            text.enabled = true;
            var script = arrow.GetComponent<arrow>();

            if (script.isSet) return;
            scoreKeeper.score += 1;
            GameObject.Find("ScoreCanvas").GetComponentInChildren<UnityEngine.UI.Text>().text = scoreKeeper.score + " of 10";
            script.isSet = true;
            text.text = script.displayText ?? "banana";
        }
    }
}