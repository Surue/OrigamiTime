using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables
    //Movement
    [Header("Movement")]
    //Jump
    [SerializeField]
    float jumpForce = 8f;
    float jumpImpulse = 0;
    float fixedHeight = 0;
    float timeButtonPressedToSwitchState = 0.5f;
    float timer = 0;
    bool isJumping = false;
    [SerializeField]
    float horizontalSpeed = 7f;
    [SerializeField]
    float fallingMultiplicator = 1.05f;
    Rigidbody body;

    //Lane height
    [Header("Lane height")]
    [SerializeField]
    float groundHeight = 0.5f;
    [SerializeField]
    float airHeight = 10f;
    [SerializeField]
    float underwaterHeight = -5f;
    bool switchState = false;

    //State
    enum State {
        GROUND,
        FLY,
        SWIM
    }

    State state = State.GROUND;
    State previousState = State.GROUND;

    //Animal state
    enum Animal {
        CAT,
        BIRD,
        FISH
    }
    Animal animal = Animal.CAT;
    #endregion

    void Start () {
        body = GetComponent<Rigidbody>();
	}

    void FixedUpdate() {
        float verticalMovement = body.velocity.y + jumpImpulse;

        if(isJumping) {
            if(verticalMovement < 0) {
                switch(state) {
                    case State.GROUND:
                        verticalMovement *= fallingMultiplicator;
                        break;

                    case State.FLY:
                    case State.SWIM:
                        verticalMovement *= 0.95f;
                        break;
                }
            }
        }

        if(switchState) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.5f) {
                transform.position = new Vector3(transform.position.x, fixedHeight, 0);
                switchState = false;
            } else {
                if(transform.position.y < fixedHeight) {
                    body.velocity = new Vector3(horizontalSpeed, jumpForce, 0);
                } else {
                    body.velocity = new Vector3(horizontalSpeed, verticalMovement, 0);
                }
            }
        }else if(!isJumping) {
            body.velocity = new Vector3(horizontalSpeed, 0, 0);
            transform.position = new Vector3(transform.position.x, fixedHeight, 0);
        } else {
            body.velocity = new Vector3(horizontalSpeed, verticalMovement, 0);
        }
    }
    
    void Update () {
        jumpImpulse = 0;

        //jump
        if(Input.GetButton("Jump") && !switchState) {
            if(!isJumping) {
                isJumping = true;
                previousState = state;
                body.useGravity = true;

                jumpImpulse = jumpForce;
            } else {
                timer += Time.deltaTime;

                if(timer > timeButtonPressedToSwitchState) {
                    timer = 0;
                    isJumping = false;
                    switch(previousState) {
                        case State.FLY:
                            break;

                        case State.GROUND:
                            state = State.FLY;
                            switchState = true;
                            break;

                        case State.SWIM:
                            state = State.GROUND;
                            switchState = true;
                            break;
                    }
                }
            }
        } else {
            timer = 0;
        }

        if(isJumping) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.5f && body.velocity.y < 0) {
                isJumping = false;
                body.useGravity = false;
            }
        }

        //Switch state to Bird
        if(Input.GetKeyDown(KeyCode.A)) {
            if(animal != Animal.FISH && animal != Animal.BIRD) {
                if(isJumping) {
                    switchState = true;
                    isJumping = false;
                    state = State.FLY;
                    animal = Animal.BIRD;
                }
            }
        }

        //Switch state to cat
        if(Input.GetKeyDown(KeyCode.S)) {
            if(animal == Animal.BIRD) {
                switchState = true;
                state = State.GROUND;
                animal = Animal.CAT;
            }else if(animal == Animal.FISH && isJumping) {
                switchState = true;
                isJumping = false;
                state = State.GROUND;
                animal = Animal.CAT;
            }
        }

        //Switch state to fish
        if(Input.GetKeyDown(KeyCode.D)) {
            if(animal == Animal.BIRD) {
                switchState = true;
                state = State.SWIM;
                animal = Animal.FISH;
            } else if(animal == Animal.CAT && isJumping) {
                switchState = true;
                isJumping = false;
                state = State.SWIM;
                animal = Animal.FISH;
            }
        }

        //State
        switch(state) {
            case State.GROUND:
                fixedHeight = groundHeight;
                break;

            case State.FLY:
                fixedHeight = airHeight;
                break;

            case State.SWIM:
                fixedHeight = underwaterHeight;
                break;
        }

        //Animal
        switch(animal) {
            case Animal.BIRD:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;

            case Animal.CAT:
                GetComponent<SpriteRenderer>().color = Color.grey;
                break;

            case Animal.FISH:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
        }
	}

    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        //Fly lane
        Gizmos.DrawLine(new Vector3(-1000, airHeight), new Vector3(1000, airHeight));

        //Ground lane
        Gizmos.DrawLine(new Vector3(-1000, groundHeight), new Vector3(1000, groundHeight));

        //Underwater lane
        Gizmos.DrawLine(new Vector3(-1000, underwaterHeight), new Vector3(1000, underwaterHeight));
    }
}
