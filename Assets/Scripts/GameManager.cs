using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    GameObject winSplash;

    [SerializeField]
    [HideInInspector]
    public float timer;

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
        DontDestroyOnLoad(this);

        if(GameObject.Find("WinSplash")) {
            winSplash = GameObject.Find("WinSplash");
            winSplash.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerDeath() {
        FindObjectOfType<TimerController>().StopTimer();
        timer = FindObjectOfType<TimerController>().GetTimer();
        StartCoroutine(loadLoseScene());
    }

    IEnumerator loadLoseScene() {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("LoseScene");
    }

    public void PlayerWin() {
        winSplash.SetActive(true);
    }

    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void newGame() {
        StartCoroutine(FindSplashWin());
    }

    IEnumerator FindSplashWin() {
        while(!GameObject.Find("WinSplash")) {
            yield return new WaitForFixedUpdate();
        }

        winSplash = GameObject.Find("WinSplash").gameObject;
        winSplash.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }
}
