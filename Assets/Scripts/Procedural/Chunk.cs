using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    
    [Range(0, 200)]
    [SerializeField]
    public int width;

    Vector3 rightAnchor;
    Vector3 leftAnchor;

	// Use this for initialization
	void Start () {
        leftAnchor = Vector3.left * (width * 0.5f);
        rightAnchor = Vector3.right * (width * 0.5f);

        if(FindObjectsOfType<Chunk>().Length < 2) {
            return;
        }

        //Look spawn
        SpawnPoint[] spawnPoints = GetComponentsInChildren<SpawnPoint>();

        List<SpawnPoint> coinSpawn = new List<SpawnPoint>();
        List<SpawnPoint> ennemySpawn = new List<SpawnPoint>();

        foreach(SpawnPoint spawn in spawnPoints) {
            if(spawn.so_spawnable.type == Spawnable.Type.COLLTECTIBLE) {
                coinSpawn.Add(spawn);
            } else {
                ennemySpawn.Add(spawn);
            }
        }

        if(coinSpawn.Count > 0) {
            if(Random.Range(0,4) > 2) {
                SpawnPoint spawn = coinSpawn[Random.Range(0, coinSpawn.Count)];
                GameObject instance = Instantiate(FindObjectOfType<MapGenerator>().ultimatesCoins[FindObjectOfType<PlayerController>().nbUltimeCoin], spawn.transform);
            } else {
                SpawnPoint spawn = coinSpawn[Random.Range(0, coinSpawn.Count)];
                GameObject instance = Instantiate(spawn.so_spawnable.prefabToSpawn, spawn.transform);
            }
        }

        for(int i = 0; i < ennemySpawn.Count;i++) {
            if(Random.Range(0,3) > 0.5f) {
                Instantiate(ennemySpawn[i].so_spawnable.prefabToSpawn, ennemySpawn[i].transform);
            } else {
                i = ennemySpawn.Count;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 GetLeftAnchor() {
        return leftAnchor + transform.position;
    }
    public Vector3 GetRightAnchor() {
        return rightAnchor + transform.position;
    }


    private void OnDrawGizmos() {
        //left ppoint
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * (width * 0.5f), 1);
        Gizmos.DrawWireSphere(transform.position + Vector3.right * (width * 0.5f), 1);
    }
}
