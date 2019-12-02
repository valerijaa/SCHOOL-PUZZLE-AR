using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{

        private void OnMouseDown()
        {
            Debug.Log(this.name + "Clicked");
        }

}
