using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovment : MonoBehaviour
{
    public float moveTime = 1.0f;
    public float speed = 10.0f;

    private Vector2 desiredVelocity;
    private float currentTime = 0.0f;
    private bool cacheValue = true;
    private Transform playerRef;
    private MobileMovements touchControls;

    // Adding Game Manager here
    public GameSceneManager gameSceneManager;

    private void Awake()
    {
        touchControls = new MobileMovements();
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
    void Update()
    {

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
                        gameObject.GetComponent<Animator>().SetTrigger("DidMove");
                        desiredVelocity = touch0.delta.normalized;
                        currentTime = moveTime;
                        cacheValue = false;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    gameObject.GetComponent<Animator>().SetTrigger("DidMoveEnd");
                    gameSceneManager.currentScore += 1;
                    cacheValue = true;
                    break;
            }

            if (currentTime > 0.0f)
            {
                currentTime -= Time.deltaTime;
                if (currentTime < 0.0f) currentTime = 0.0f;
                float normalizedTime = currentTime / moveTime;
                playerRef.transform.position += new Vector3(
                    desiredVelocity.x * normalizedTime * speed * Time.deltaTime,
                   0.0f,
                    0.0f
                    );
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
                        gameObject.GetComponent<Animator>().SetTrigger("DidJump");
                        desiredVelocity = touch0.delta.normalized;
                        currentTime = moveTime;
                        cacheValue = false;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    gameObject.GetComponent<Animator>().SetTrigger("DidJumpEnd");
                    cacheValue = true;
                    gameSceneManager.currentScore += 10;
                    break;
            }

            if (currentTime > 0.0f)
            {
                currentTime -= Time.deltaTime;
                if (currentTime < 0.0f) currentTime = 0.0f;
                float normalizedTime = currentTime / moveTime;
                playerRef.transform.position += new Vector3(
                    0.0f,
                    desiredVelocity.y * normalizedTime * speed * Time.deltaTime,
                    0.0f
                    );
            }
        }
    }
}
