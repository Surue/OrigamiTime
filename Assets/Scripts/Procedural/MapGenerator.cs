using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Header("Chunks")]
    [SerializeField]
    List<Chunk> chunks;

    int nbChunk = 0;

    [System.Serializable]
    public enum Type {
        FOREST,
        VILLAGE,
        TOWN,
        WATER,
        LENGHT
    }
    
    List<Chunk> activeChunks = new List<Chunk>();

    PlayerController player;

    // Use this for initialization
    void Start () {
        Chunk c = Instantiate(GetChunk(0));
        c.transform.position = Vector3.zero;

        activeChunks.Add(c);

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(activeChunks[activeChunks.Count - 1].GetLeftAnchor(), player.transform.position) < 20) {
            AddChunk();
        }

        if(Vector3.Distance(activeChunks[0].GetRightAnchor(), player.transform.position) > 20){
            DeleteChunk();
        }
    }

    void DeleteChunk() {
        Destroy(activeChunks[0].gameObject);
        activeChunks.RemoveAt(0);
    }

    void AddChunk() {
        float t = Mathf.PerlinNoise(Time.time * 0.05f, 0) * (int)Type.LENGHT - 1;
        Debug.Log(Mathf.PerlinNoise(Time.time * 0.05f, 0) * (int)Type.LENGHT - 1);

        Chunk c = Instantiate(GetChunk(Mathf.RoundToInt(t)));
        c.transform.position = activeChunks[activeChunks.Count - 1].GetRightAnchor() + c.width * 0.5f * Vector3.right;

        activeChunks.Add(c);

        nbChunk++;
    }

    Chunk GetChunk(int n) {
        Type t = (Type)n;
        List<Chunk> possibleChunk = new List<Chunk>();


        foreach(Chunk c in chunks) {
            if(c.type == t) {
                possibleChunk.Add(c);
            }
        }

        return possibleChunk[Random.Range(0, possibleChunk.Count)];
    }
}
