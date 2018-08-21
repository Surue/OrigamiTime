using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    [SerializeField]
    GameObject followedObject;
    [SerializeField]
    Vector3 offset;

	// Use this for initialization
	void Start () {
        transform.position = followedObject.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = new Vector3(followedObject.transform.position.x + offset.x, offset.y, offset.z);
    }
}
