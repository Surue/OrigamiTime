using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Header("Ultimate Coin")]
    [SerializeField]
    public List<GameObject> ultimatesCoins;

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
        Chunk c = Instantiate(chunks[0]);
        c.transform.position = new Vector3(0, -4.5f, 0);

        activeChunks.Add(c);

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(activeChunks[activeChunks.Count - 1].GetLeftAnchor(), player.transform.position) < activeChunks[0].width) {
            AddChunk();
        }

        if(Vector3.Distance(activeChunks[0].GetRightAnchor(), player.transform.position) > activeChunks[0].width){
            DeleteChunk();
        }
    }

    void DeleteChunk() {
        Destroy(activeChunks[0].gameObject);
        activeChunks.RemoveAt(0);
    }

    void AddChunk() {
        Chunk c = Instantiate(GetChunk());
        c.transform.position = activeChunks[activeChunks.Count - 1].GetRightAnchor() + c.width * 0.5f * Vector3.right;

        activeChunks.Add(c);

        nbChunk++;
    }

    Chunk GetChunk() {
        return chunks[Random.Range(0, chunks.Count)];
        //return chunks[1];
    }
}
