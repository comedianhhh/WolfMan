using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BackgroundAnimation : MonoBehaviour
{
    public float duration = 4f; // How long the shake lasts
    public float strength = 0.2f; // The strength of the shake
    public int vibrato = 3; // Vibrato specifies the amount of shake segments
    public float randomness = 90f; // The randomness of the shake, higher values result in more random movement
    public bool snapping = false; // If true, the shake will snap to integers, useful for pixel-art games
    public bool fadeOut = true; // If true, the shake intensity will decrease over time


    public float scaleDuration = 4f;
    public float scaleStrength = 0.05f; // Subtle scale change
    public int scaleVibrato = 2;
    public float scaleRandomness = 90f;
    private void Start()
    {
        // Start the tree waving effect when the game starts
        StartWaving();
    }

    private void StartWaving()
    {
        // Use DoTween's DOShakePosition to simulate the tree swaying due to wind
        transform.DOShakePosition(duration, new Vector3(strength, strength, 0), vibrato, randomness, snapping, fadeOut)
                 .SetLoops(-1, LoopType.Restart); // Loop the effect indefinitely

        // Simulate the effect of wind on the tree's scale
        transform.DOShakeScale(scaleDuration, new Vector3(scaleStrength, scaleStrength, 0), scaleVibrato, scaleRandomness, fadeOut)
                 .SetLoops(-1, LoopType.Restart); // Loop the effect indefinitely
    }

}
