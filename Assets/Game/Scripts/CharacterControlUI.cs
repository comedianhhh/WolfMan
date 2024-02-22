using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterControlUI : MonoBehaviour
{
    public PlayerMovment movement;


    public void Up()
    {
        movement= FindObjectOfType<PlayerMovment>();
        if (movement != null)  movement.Jump();
    }

    public void Down()
    {
        movement = FindObjectOfType<PlayerMovment>();
        if (movement != null)  movement.StopMovement();
    }
    public void Left()
    {
        movement = FindObjectOfType<PlayerMovment>();
        if (movement != null) movement.MoveLeft();
    }
    public void Right()
    {
        movement = FindObjectOfType<PlayerMovment>();
        if (movement != null) movement.MoveRight();
    }
}
