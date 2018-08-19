using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTime : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI text;

    // Use this for initialization
    void Start () {
        text.text = GameManager.Instance.timer.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
