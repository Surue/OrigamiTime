using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [SerializeField]
    public Spawnable so_spawnable;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos() {
        if(so_spawnable.prefabToSpawn.GetComponent<MeshFilter>()) {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawMesh(so_spawnable.prefabToSpawn.GetComponent<MeshFilter>().sharedMesh, transform.position);
        } else {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
