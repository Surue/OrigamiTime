using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnnemiesController : MonoBehaviour {

    Rigidbody body;

    [SerializeField]
    float fixedHeight = 10f;
    [SerializeField]
    float horizontalSpeed = -1f;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        transform.position = new Vector3(transform.position.x, fixedHeight, 0);

        body.velocity = Vector3.right * horizontalSpeed;
    }
}
