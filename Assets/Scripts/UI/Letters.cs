using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Letters : MonoBehaviour {

    SkeletonGraphic skeleton;

	// Use this for initialization
	void Start () {
        skeleton = GetComponent<SkeletonGraphic>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Founded() {
        skeleton.AnimationState.SetAnimation(0, "transition_caught", false);
        skeleton.AnimationState.SetAnimation(1, "idle_caught", true);
    }
}
