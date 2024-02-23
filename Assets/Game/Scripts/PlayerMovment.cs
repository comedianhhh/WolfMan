using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovment : MonoBehaviour
{
    public float moveTime = 1.0f;
    public float speed = 10.0f;
    public int health = 3;
    private Vector2 desiredVelocity;

  


    public GameSceneManager gameSceneManager;


    private MobileMovements touchControls;
    private Animator animator;
    private GameObject sprite;
    public AudioClip CoinSound;

    public string IdleState = "Idle";
    public string WalkState = "Walk";
    public string HurtState = "Hurt";
    public string JumpState = "Jump";


    public int jumpstateindex;
    public int idlestateindex;
    public int walkstateindex;
    public int hurtstateindex;

    //Junmp
    public float jumpForce = 6200f; // Adjust this force for a suitable jump height
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private bool isGrounded; // To check if the player is touching the ground
    private float fallMultiplier = 2.5f; // This is used to make the jump feel more natural

    private void Awake()
    {

        touchControls = new MobileMovements();
        touchControls.MobileMovementMap.Movement.performed += ctx => desiredVelocity = ctx.ReadValue<Vector2>();
        touchControls.MobileMovementMap.Movement.canceled += ctx => desiredVelocity = Vector2.zero;
        touchControls.MobileMovementMap.Jump.performed += ctx => Jump();

        idlestateindex = Animator.StringToHash(IdleState);
        walkstateindex = Animator.StringToHash(WalkState);
        hurtstateindex = Animator.StringToHash(HurtState);
        jumpstateindex = Animator.StringToHash(JumpState);

        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        moveTime = Mathf.Max(moveTime, 0.5f);
        gameSceneManager = FindObjectOfType<GameSceneManager>();
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector2.up * rb.mass * (fallMultiplier - 1) * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        Vector3 move = new Vector3(desiredVelocity.x, 0, 0) * speed * Time.fixedDeltaTime;
        transform.Translate(move);


        if (desiredVelocity.x == 0)
        {
            animator.SetTrigger(idlestateindex);
        }
        else if (desiredVelocity.x != 0)
        {
            animator.SetTrigger(walkstateindex);
        }



        if (desiredVelocity.x < 0)
        {
            sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (desiredVelocity.x > 0)
        {
            sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }
    public void MoveLeft()
    {
        desiredVelocity = Vector2.left;
    }

    public void MoveRight()
    {
        desiredVelocity = Vector2.right;
    }

    public void StopMovement()
    {
        desiredVelocity = Vector2.zero;
    }
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply a vertical force
            animator.SetTrigger(jumpstateindex); // Trigger jump animation
            gameSceneManager.currentScore += 1;
            isGrounded = false; // Update the grounded status
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(CoinSound);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Update the grounded status
        }

    }
}

