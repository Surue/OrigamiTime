using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    Image winSplash;

    static GameManager instance;
    public static GameManager Instance {
        get {
            return instance;
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerDeath() {
        //SceneManager.LoadScene("LoseScene");
        FindObjectOfType<TimerController>().StopTimer();
    }

    public void PlayerWin() {
        winSplash.gameObject.SetActive(true);
    }

    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void Quit() {
        Application.Quit();
    }
}
