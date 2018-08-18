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

    [Header("Sensor")]
    [SerializeField]
    BoxCollider footCollider;
    [SerializeField]
    BoxCollider headCollider;
    [SerializeField]
    BoxCollider frontCollider;

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

        footCollider.enabled = false;
	}

    void FixedUpdate() {
        float verticalMovement = body.velocity.y + jumpImpulse;

        if(verticalMovement > jumpForce) {
            verticalMovement = jumpForce;
        }

        if(isJumping) {
            if(verticalMovement < 0) {
                switch(state) {
                    case State.GROUND:
                        verticalMovement *= fallingMultiplicator;
                        break;

                    case State.FLY:
                    case State.SWIM:
                        verticalMovement *= fallingMultiplicator;
                        break;
                }
            }
        }

        if(switchState) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.5f) {
                switchState = false;
                if(state == State.SWIM) {
                    body.useGravity = false;
                }

                transform.position = new Vector3(transform.position.x, fixedHeight, 0);
            } else {
                if(transform.position.y < fixedHeight) {
                    body.velocity = new Vector3(horizontalSpeed, jumpForce, 0);
                } else {
                    body.velocity = new Vector3(horizontalSpeed, verticalMovement, 0);
                }
            }
        }else if(!isJumping) {
            if(state != State.GROUND) {
                body.velocity = new Vector3(horizontalSpeed, 0, 0);
                transform.position = new Vector3(transform.position.x, fixedHeight, 0);
            } else {
                body.velocity = new Vector3(horizontalSpeed, body.velocity.y, 0);
            }
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
                    isJumping = false;
                    switch(previousState) {
                        case State.FLY:
                            break;

                        case State.GROUND:
                            state = State.FLY;
                            animal = Animal.BIRD;
                            switchState = true;
                            body.useGravity = false;
                            break;

                        case State.SWIM:
                            state = State.GROUND;
                            animal = Animal.CAT;
                            switchState = true;
                            body.useGravity = false;
                            break;
                    }
                }
            }
        } else {
            timer = 0;
        }

        if(isJumping) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.5f && body.velocity.y < 0f) {
                isJumping = false;
                if(state != State.GROUND) {
                    body.useGravity = false;
                }
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
                    body.useGravity = false;
                }
            }
        }

        //Switch state to cat
        if(Input.GetKeyDown(KeyCode.S)) {
            if(animal == Animal.BIRD) {
                switchState = true;
                state = State.GROUND;
                animal = Animal.CAT;
                footCollider.enabled = false;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);
            } else if(animal == Animal.FISH && isJumping) {
                switchState = true;
                isJumping = false;
                state = State.GROUND;
                animal = Animal.CAT;
                footCollider.enabled = false;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);
            }
        }

        //Switch state to fish
        if(Input.GetKeyDown(KeyCode.D)) {
            if(animal == Animal.BIRD) {
                switchState = true;
                state = State.SWIM;
                animal = Animal.FISH;
                footCollider.enabled = true;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);
            } else if(animal == Animal.CAT && (isJumping || body.velocity.y < -0.5f)) {
                switchState = true;
                isJumping = false;
                state = State.SWIM;
                animal = Animal.FISH;
                footCollider.enabled = true;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);
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

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            GameManager.Instance.PlayerDeath();
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            GameManager.Instance.PlayerDeath();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            GameManager.Instance.PlayerDeath();
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
