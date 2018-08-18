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
        transform.position = Vector3.Lerp(transform.position, followedObject.transform.position + offset, Time.deltaTime * 5);
        transform.position = new Vector3(transform.position.x, transform.position.y, -7);
    }
}
