using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scoreKeeper : MonoBehaviour
{
    // private
    const string Hostname = "https://school-puzzle-api.herokuapp.com/api/";

    // public
    public static int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0; // current score by default 0

        // get and set max score
        StartCoroutine(GetAndSetTotalStops());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetAndSetTotalStops()
    {
        UnityWebRequest uwr = UnityWebRequest.Get(Hostname+"stats");
        yield return uwr.SendWebRequest();

        if (!uwr.isNetworkError)
        {
            var stats = JsonUtility.FromJson<Stats>(uwr.downloadHandler.text);
            var textComponent = this.gameObject.GetComponent<Text>();
            textComponent.text = "0 of " + stats.TotalStops;
        }
        else
        {
            Debug.LogError("Failed to fetch data");
        }
    }
}
