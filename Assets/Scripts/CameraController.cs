using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform character; // Assign this through the Unity Editor
    public float smoothSpeed = 0.125f; // Adjust this value to change how smoothly the camera follows the character
    public Vector3 offset; // Use this to maintain a distance between the camera and the character

    private void Awake()
    {
        // Assign the character's transform to the character variable
        character = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void FixedUpdate()
    {
        // Create a desired position vector that is adjusted for the offset, but ignores the Z-axis of the character
        Vector3 desiredPosition = new Vector3(character.position.x + offset.x, character.position.y + offset.y, transform.position.z);

        // Use Lerp to smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
