using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {

    [SerializeField]
    float initialTimer = 60f;

    [SerializeField]
    TextMeshProUGUI text;

    bool isStopped = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(isStopped) {
            return;
        }

        if(initialTimer < 0) {
            GameManager.Instance.PlayerDeath();
        }

        initialTimer -= Time.deltaTime;

        text.text = ((int)initialTimer / 60).ToString();
        if((initialTimer % 60) < 10) {
            text.text = "0" + (initialTimer % 60).ToString();
        } else {
            text.text = (initialTimer - ((int)initialTimer / 60) % 60).ToString();
        }
    }

    public void AddTime(float t) {
        initialTimer += t;
    }

    public void StopTimer() {
        isStopped = true;
    }
}
