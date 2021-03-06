﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    // Player attributes
    // Public variables 
    public int playerIndex = 1;
    public float maxSpeed = 4;
    public float jumpForce = 400;
    public float minHeight, maxHeight;
    public int maxHealth = 10;
    public Collider interactObj;
    public AudioClip punchSound, collisionSound, jumpSound, healthItem;

    // Private variables
    private bool onGround2;
    private bool isDead2 = false;
    private bool isFacingRight2 = false;
    private bool isJumping2 = false;
    private int currentHealth;
    private float currentSpeed;
    private float torchDistance;


    // GameObjects
    private Player player;
    private Rigidbody rb;
    // private GameManager gManager 
    private GameObject torch;
    private Animator anim2;
    private Transform groundCheck2;
    private Vector2 inputVector;
    private TorchControllerSS torchControl;
    public AudioSource currAudioSource;
    GameObject fireBall;

    public AudioClip collisionSound2, jumpSound2, healthItem2;

    // Initialization
    void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        anim2 = GetComponent<Animator>();
        torch = GameObject.Find("Torch");
        currAudioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        groundCheck2 = gameObject.transform.Find("GroundCheck2");
        currentHealth = maxHealth;
        currentSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Set onGround and animation bools
        onGround2 = Physics.Linecast(transform.position, groundCheck2.position, 1 << LayerMask.NameToLayer("Ground"));
        anim2.SetBool("OnGround", onGround2);
        anim2.SetBool("Dead", isDead2);
        
        
    }

    public void SetInputVector(Vector2 direction)
	{
		inputVector = direction;
	}

    // Updates independently of frame
    private void FixedUpdate()
    {
        if (!isDead2)
        {
            // Movement
            if (onGround2) anim2.SetFloat("Speed", 0.1f + Mathf.Abs(rb.velocity.magnitude));
            // Player 2 Movement
            float h = inputVector.x;
            float v = inputVector.y;

            if (!onGround2) v = 0;

            rb.velocity = new Vector3(h * currentSpeed, rb.velocity.y, v * currentSpeed);

            // Flips sprite based on movement
            if(h < 0 && !isFacingRight2)
            {
                Flip();
            } else if(h > 0 && isFacingRight2)
            {
                Flip();
            }

            // If player is jumping, add vertical force
            if (isJumping2)
            {
                isJumping2 = false;
                rb.AddForce(Vector3.up * jumpForce);
            }

            float minWidth = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x;
			float maxWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10)).x;
			rb.position = new Vector3(Mathf.Clamp(rb.position.x, minWidth + 1, maxWidth - 1),
				rb.position.y,
				Mathf.Clamp(rb.position.z, minHeight, maxHeight));

            // Torch Interaction
            Vector3 torchPosition = torch.transform.position - transform.position;
            torchDistance = torchPosition.x;
            torchControl = torch.GetComponent<TorchControllerSS>();
            // if(Input.GetKeyDown(KeyCode.U) && torchDistance.x <= 1.5f && !torchControl.isLit)
            // {
            //     print("Lighting lantern!");
            //     torchControl.lightLantern();
            // }else if((Input.GetKeyDown(KeyCode.U) && torchDistance.x <= 1.5f && torchControl.isLit))
            // {
            //     torchControl.darkLantern();
            // }

            // Attacks + Specials
            // Currently using keycode inputs, please change to Input System Package ASAP
            /*if ()
            {
                Debug.Log("Is attacking");
                anim2.ResetTrigger("Attack");
                anim2.SetTrigger("Attack");
            }

            if ()
            {
                Debug.Log("Is healing");
                anim2.ResetTrigger("Attack");
                anim2.SetTrigger("Attack");
            }*/
        }
    }

    
    public void Jump()
    {
        isJumping2 = true;
    }

    public void Interact(Collider other)
    {
        if (other.CompareTag("Health Item"))
        {
            Destroy(other.gameObject);
            interactObj = null;
            anim2.SetTrigger("Catching");
            PlaySong(healthItem);
            currentHealth = maxHealth;
            // FindObjectOfType<UIManager>().UpdateHealth(currentHealth);
        }

        if (other.CompareTag("Torch"))
            print("here");
        {
            if (torchDistance <= 1.5f && !torchControl.isLit)
            {
                print("Lighting lantern!");
                torchControl.lightLantern();
            }
            else if (torchDistance <= 1.5f && torchControl.isLit)
            {
                torchControl.darkLantern();
            }
        }
    }

    // Flip function for flipping sprite when facing in a direction
    private void Flip()
    {
        isFacingRight2 = !isFacingRight2;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Hit Damage function
    public void TookDamage(int damage) 
    {
        if (!isDead2)
        {
            Player.currentHealth-= damage;
            currentHealth -= damage;
            anim2.SetTrigger("HitDamage");
            //print("Player 2 took damage!");
            //PlaySong(collisionSound);
            if (currentHealth <= 0)
            {
                isDead2 = true;
                FindObjectOfType<GameManager>().lives--;
                if (isFacingRight2)
                {
                    rb.AddForce(new Vector3(-3, 5, 0), ForceMode.Impulse);
                }
                else
                {
                    rb.AddForce(new Vector3(3, 5, 0), ForceMode.Impulse);
                }
            }
        }
    }

    public void PlaySong(AudioClip clip)
    {
        currAudioSource.clip = clip;
        currAudioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        interactObj = other;
    }

    // Player Respawn function
    void PlayerRespawn()
    {
        if (FindObjectOfType<GameManager>().lives > 0)
        {
            isDead2 = false;
            FindObjectOfType<UIManager>().UpdateLives();
            currentHealth = maxHealth;
            // FindObjectOfType<UIManager>().UpdateHealth(currentHealth;
            anim2.Rebind();
            float minWidth = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x;
            transform.position = new Vector3(minWidth, 10, -4);
        }
        else
        {
            FindObjectOfType<UIManager>().UpdateDisplayMessage("Game Over");
            Destroy(FindObjectOfType<GameManager>().gameObject);
            Invoke("LoadScene", 2f);
        }
    }
}
