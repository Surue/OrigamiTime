﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

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
    [SerializeField]
    SkeletonAnimation skeletonDragon;
    SkeletonAnimation skeletonActive;
    bool isMorphing = false;
    [SerializeField]
    GameObject droplets;

    [Header("UI")]
    [SerializeField]
    List<Letters> ultimateCoinsImages;

    //State
    bool isDead = false;

    //Animal state
    enum Animal {
        CAT,
        BIRD,
        FISH,
        DRAGON
    }
    Animal animal = Animal.CAT;

    //TimerController
    TimerController timerController;

    //invulnerability time
    float invulnerabilityTime = 1f;

    //Ultimate coin
    public int nbUltimeCoin = 0;
    #endregion

    void Start () {
        body = GetComponent<Rigidbody>();

        footCollider.enabled = false;

        skeletonFish.gameObject.SetActive(false);
        skeletonBird.gameObject.SetActive(false);

        skeletonActive = skeletonCat;
        skeletonActive.AnimationState.SetAnimation(0, "run", true);

        timerController = FindObjectOfType<TimerController>();

        transform.position = new Vector3(0, groundHeight, 0);
        body.velocity = Vector3.right * horizontalSpeed;

        fixedHeight = groundHeight;
    }

    void FixedUpdate() {
        if(isDead) {
            body.useGravity = true;
            body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);
            return;
        }

        float verticalMovement = body.velocity.y + jumpImpulse;

        if(switchState) {
            if(Mathf.Abs(transform.position.y - fixedHeight) < 0.2f) {
                body.velocity = new Vector3(horizontalSpeed, 0, 0);
                transform.position = new Vector3(transform.position.x, fixedHeight, 0);

                switchState = false;
                
                skeletonActive.AnimationState.SetAnimation(0, "run", true);
            } else {
                if(transform.position.y < fixedHeight) {
                    body.velocity = new Vector3(horizontalSpeed, jumpForce, 0);
                } else {
                    body.velocity = new Vector3(horizontalSpeed, -jumpForce, 0);
                }
            }
        } else if(!isJumping) {
            if(animal == Animal.CAT) {
                if(Physics.Raycast(transform.position, Vector3.down, 10 , LayerMask.NameToLayer("Ground"))) {
                    body.velocity = new Vector3(horizontalSpeed, -jumpForce*0.5f, 0);
                } else {
                    body.velocity = new Vector3(horizontalSpeed, 0, 0);
                }
            } else {
                body.velocity = new Vector3(horizontalSpeed, 0, 0);
            }
        } else if(isJumping) {
            if(verticalMovement < 0) {
                verticalMovement *= fallingMultiplicator;
            }

            body.velocity = new Vector3(horizontalSpeed, verticalMovement, 0);
        }
    }
    
    void Update () {
        if(isDead) {
            return;
        }

        if(isJumping) {
            if((Mathf.Abs(transform.position.y - fixedHeight) < 0.2f && body.velocity.y <= 0f)) {
                isJumping = false;

                body.useGravity = false;
                body.velocity = new Vector3(body.velocity.x, 0, 0);
                transform.position = new Vector3(transform.position.x, fixedHeight, 0);

                skeletonActive.AnimationState.SetAnimation(0, "run", true);
            }
        }

        jumpImpulse = 0;

        //jump
        if((Input.GetButtonDown("A") || Input.GetButtonDown("Jump")) && !switchState) {
            if(!isJumping) {
                isJumping = true;

                jumpImpulse = jumpForce;

                if(animal == Animal.FISH) {
                    jumpImpulse *= 1f;
                }

                body.useGravity = true;
            } 
        }

        #region Switch Animal Form

        //Switch state to Bird
        if(Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Y")) {
            if(animal != Animal.FISH && animal != Animal.BIRD) {
                if(isJumping) {
                    body.useGravity = false;
                    switchState = true;
                    isJumping = false;
                    animal = Animal.BIRD;
                    headCollider.enabled = true;

                    StartCoroutine(SwitchSkin(skeletonCat, skeletonBird));

                    fixedHeight = airHeight;
                }
            }
        }

        //Switch state to cat
        if(Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("X")) {
            if(animal == Animal.BIRD) {
                switchState = true;
                animal = Animal.CAT;
                footCollider.enabled = false;
                headCollider.enabled = true;

                StartCoroutine(SwitchSkin(skeletonBird, skeletonCat));

                fixedHeight = groundHeight;
            } else if(animal == Animal.FISH && isJumping) {
                switchState = true;
                isJumping = false;
                animal = Animal.CAT;
                footCollider.enabled = false;
                headCollider.enabled = true;
                body.useGravity = false;

                jumpImpulse = jumpForce * 0.5f;

                StartCoroutine(SwitchSkin(skeletonFish, skeletonCat));

                fixedHeight = groundHeight;
            }
        }

        //Switch state to fish
        if(Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("B")) {
            if(animal == Animal.BIRD) {
                switchState = true;
                animal = Animal.FISH;
                footCollider.enabled = true;
                headCollider.enabled = false;

                StartCoroutine(SwitchSkin(skeletonBird, skeletonFish));

                fixedHeight = underwaterHeight;
            } else if(animal == Animal.CAT && (isJumping || body.velocity.y < -0.5f)) {
                body.useGravity = false;
                switchState = true;
                isJumping = false;
                animal = Animal.FISH;
                footCollider.enabled = true;
                headCollider.enabled = false;

                StartCoroutine(SwitchSkin(skeletonCat, skeletonFish));

                fixedHeight = underwaterHeight;
            }
        }
        #endregion

        //Animation
        if(isJumping) {
            if(body.velocity.y > 0) {
                skeletonActive.AnimationState.SetAnimation(0, "jump", false);
            } else {
                skeletonActive.AnimationState.SetAnimation(0, "land", false);
            }
        }else if(switchState) {
            if(!isMorphing) {
                if(body.velocity.y > 0) {
                    skeletonActive.AnimationState.SetAnimation(0, "jump", false);
                } else {
                    skeletonActive.AnimationState.SetAnimation(0, "land", false);
                }
            }
        } else {
            if(skeletonActive.AnimationName != "run") {
                skeletonActive.AnimationState.SetAnimation(0, "run", true);
            }
        }

        invulnerabilityTime -= Time.deltaTime;
	}

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Water")) {
            GameObject instance = Instantiate(droplets);
            instance.transform.position = transform.position;
            Destroy(instance, 3f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Water")) {
            GameObject instance = Instantiate(droplets);
            instance.transform.position = transform.position;
            Destroy(instance, 3f);
        }

        if(animal == Animal.DRAGON) {
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Coin")) {
            Destroy(other.gameObject);
            timerController.AddTime(10);
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") && invulnerabilityTime < 0) {
            KillPlayer();

            GameManager.Instance.PlayerDeath();
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            KillPlayer();

            GameManager.Instance.PlayerDeath();
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Water") && animal != Animal.FISH && body.velocity.y < 0) {
            KillPlayer();

            GameManager.Instance.PlayerDeath();
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("UltimateCoin")) {
            Debug.Log("Coucou");
            ultimateCoinsImages[nbUltimeCoin].Founded();
            Destroy(other.gameObject);
            timerController.AddTime(10);
            nbUltimeCoin++;

            if(nbUltimeCoin >= ultimateCoinsImages.Count) {
                Win();
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ennemy")) {
            KillPlayer();

            GameManager.Instance.PlayerDeath();
            return;
        }
    }

    void Win() {
        timerController.StopTimer();

        GameManager.Instance.PlayerWin();

        skeletonDragon.gameObject.SetActive(true);

        switch(animal) {
            case Animal.CAT:
                StartCoroutine(SwitchSkin(skeletonCat, skeletonDragon));
                break;

            case Animal.BIRD:
                StartCoroutine(SwitchSkin(skeletonBird, skeletonDragon));
                break;

            case Animal.FISH:
                StartCoroutine(SwitchSkin(skeletonFish, skeletonDragon));
                break;
        }

        animal = Animal.DRAGON;
    }

    public void KillPlayer() {

        isDead = true;
        skeletonActive.AnimationState.SetAnimation(0, "dead", false);

        body.useGravity = true;

        StopAllCoroutines();
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
