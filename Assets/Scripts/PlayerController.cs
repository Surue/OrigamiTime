using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

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

    //Animation
    [Header("Animation")]
    [SerializeField]
    SkeletonAnimation skeletonCat;
    [SerializeField]
    SkeletonAnimation skeletonFish;
    [SerializeField]
    SkeletonAnimation skeletonBird;
    SkeletonAnimation skeletonActive;
    bool isMorphing = false;

    //State
    enum State {
        GROUND,
        FLY,
        SWIM,
        DEAD
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

        skeletonFish.gameObject.SetActive(false);
        skeletonBird.gameObject.SetActive(false);

        skeletonActive = skeletonCat;
        skeletonActive.AnimationState.SetAnimation(0, "run", true);
    }

    void FixedUpdate() {
        if(state == State.DEAD) {
            body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);
            return;
        }

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
                skeletonActive.AnimationState.SetAnimation(0, "run", true);
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

            case State.DEAD:
                return;
        }

        if(isJumping) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.5f && body.velocity.y < 0f) {
                isJumping = false;
                if(state != State.GROUND) {
                    body.useGravity = false;
                }

                skeletonActive.AnimationState.SetAnimation(0, "run", true);
            }
        }

        #region Switch Animal Form

        //Switch state to Bird
        if(Input.GetKeyDown(KeyCode.A)) {
            if(animal != Animal.FISH && animal != Animal.BIRD) {
                if(isJumping) {
                    switchState = true;
                    isJumping = false;
                    state = State.FLY;
                    animal = Animal.BIRD;
                    body.useGravity = false;

                    StartCoroutine(SwitchSkin(skeletonCat, skeletonBird));
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

                StartCoroutine(SwitchSkin(skeletonBird, skeletonCat));
            } else if(animal == Animal.FISH && isJumping) {
                switchState = true;
                isJumping = false;
                state = State.GROUND;
                animal = Animal.CAT;
                footCollider.enabled = false;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);
                
                StartCoroutine(SwitchSkin(skeletonFish, skeletonCat));
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

                StartCoroutine(SwitchSkin(skeletonBird, skeletonFish));
            } else if(animal == Animal.CAT && (isJumping || body.velocity.y < -0.5f)) {
                switchState = true;
                isJumping = false;
                state = State.SWIM;
                animal = Animal.FISH;
                footCollider.enabled = true;
                body.useGravity = true;

                body.velocity = new Vector3(body.velocity.x, 0, 0);

                StartCoroutine(SwitchSkin(skeletonCat, skeletonFish));
            }
        }
        #endregion

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

            case State.DEAD:
                break;
        }

        //Animation
        if(isJumping) {
            if(body.velocity.y > 0) {
                skeletonActive.AnimationState.SetAnimation(0, "jump", true);
            } else {
                skeletonActive.AnimationState.SetAnimation(0, "land", true);
            }
        }
        if(switchState) {
            if(!isMorphing) {
                if(body.velocity.y > 0) {
                    skeletonActive.AnimationState.SetAnimation(0, "jump", true);
                } else {
                    skeletonActive.AnimationState.SetAnimation(0, "land", true);
                }
            }
        }
	}

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            GameManager.Instance.PlayerDeath();

            state = State.DEAD;
            skeletonActive.AnimationState.SetAnimation(0, "dead", false);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            GameManager.Instance.PlayerDeath();

            state = State.DEAD;
            skeletonActive.AnimationState.SetAnimation(0, "dead", false);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            GameManager.Instance.PlayerDeath();

            state = State.DEAD;
            skeletonActive.AnimationState.SetAnimation(0, "dead", false);
        }
    }

    IEnumerator SwitchSkin(SkeletonAnimation previousAnimation, SkeletonAnimation nextAnimation) {
        isMorphing = true;
        skeletonActive.AnimationState.SetAnimation(0, "out", false);
        yield return new WaitForSeconds(0.367f);

        previousAnimation.gameObject.SetActive(false);
        nextAnimation.gameObject.SetActive(true);
        skeletonActive = nextAnimation;
        skeletonActive.AnimationState.SetAnimation(0, "in", false);
        yield return new WaitForSeconds(0.367f);
        isMorphing = false;
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
