using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject player;
    public AudioClip AudioClip;
    public Transform respawnPoint;

    private void Awake()
    {
        player=GameObject.Find("Wolf");
        respawnPoint = GameObject.Find("RespawnPoint").GetComponent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.transform.position = respawnPoint.position;
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(AudioClip);
        }
    }
}
