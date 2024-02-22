using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainPlayerController : MonoBehaviour
{
    public float damageRate = 20.0f;
    public float health = 100.0f;

    public MenuClassifier hudClassifier;

    private TextMeshProUGUI healthCounter;

    private void Start()
    {
        HUDMenu menu = MenuManager.Instance.GetMenu<HUDMenu>(hudClassifier);
        healthCounter = menu.HealthCounter;
    }

    void Update()
    {
        health -= damageRate * Time.deltaTime;
        if (health <= 0.0f)
        {
            health = 0.0f;
            Destroy(gameObject);
        }

        healthCounter.text = health.ToString();
    }
}
