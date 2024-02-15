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
    private float currentTime = 0.0f;
    private bool cacheValue = true;
    private Transform playerRef;


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
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponent<Transform>();
        moveTime = Mathf.Max(moveTime, 0.5f);
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
#if UNITY_ANDROID && !UNITY_EDITOR

        // 1 finger swipe left-right to move left/right
        if (touchControls.MobileMovementMap.Touch0.WasPerformedThisFrame() &&
           touchControls.MobileMovementMap.Touch1.WasPerformedThisFrame() == false)
        {
            TouchState touch0 = touchControls.MobileMovementMap.Touch0.ReadValue<TouchState>();
            switch (touch0.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (cacheValue)
                    {
                        animator.SetTrigger(walkstateindex);
                        desiredVelocity = touch0.delta.normalized;


                        currentTime = moveTime;
                        cacheValue = false;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    animator.SetTrigger(idlestateindex);
                    gameSceneManager.currentScore+= 1;
                    cacheValue = true;
                    break;
            }

            if (currentTime > 0.0f)
            {
                currentTime -= Time.fixedDeltaTime;
                if (currentTime < 0.0f) currentTime = 0.0f;
                float normalizedTime = currentTime / moveTime;
                playerRef.transform.position += new Vector3(
                    desiredVelocity.x * normalizedTime * speed * Time.fixedDeltaTime,
                   0.0f,
                    0.0f
                    );
                if (desiredVelocity.x < 0)
                {
                    sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (desiredVelocity.x > 0)
                {
                    sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

            }
        }

        // 2 finger swipe up to jump
        if (touchControls.MobileMovementMap.Touch0.WasPerformedThisFrame() &&
          touchControls.MobileMovementMap.Touch1.WasPerformedThisFrame())
        {
            TouchState touch0 = touchControls.MobileMovementMap.Touch0.ReadValue<TouchState>();
            switch (touch0.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (cacheValue)
                    {
                        animator.SetTrigger(jumpstateindex);
                        desiredVelocity = touch0.delta.normalized;
                        currentTime = moveTime;
                        cacheValue = false;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    animator.SetTrigger(walkstateindex);
                    cacheValue = true;
                    break;
            }

            if (currentTime > 0.0f)
            {
                currentTime -= Time.fixedfixedDeltaTime;
                if (currentTime < 0.0f) currentTime = 0.0f;
                float normalizedTime = currentTime / moveTime;
                playerRef.transform.position += new Vector3(
                    0.0f,
                    desiredVelocity.y * normalizedTime * speed * Time.fixedDeltaTime,
                    0.0f
                    );
            }
        }


#elif UNITY_EDITOR || UNITY_STANDALONE
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
#endif
    }
    private void Jump()
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

