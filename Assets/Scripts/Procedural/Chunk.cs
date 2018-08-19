﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    [SerializeField]
    public MapGenerator.Type type;
    [Range(0, 100)]
    [SerializeField]
    public int width;

    Vector3 rightAnchor;
    Vector3 leftAnchor;

	// Use this for initialization
	void Start () {
        leftAnchor = Vector3.left * (width * 0.5f);
        rightAnchor = Vector3.right * (width * 0.5f);

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
