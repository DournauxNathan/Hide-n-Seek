using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameOn = false;
    public List<AIController> hiders; // Reference to the Hider AIs.
    public List<AIController> seekers; // Reference to the Hider AIs.
    public float gameDuration = 300.0f; // Set the game duration in seconds.
    public float hidingTime = 300.0f; // Set the game duration in seconds.

    private float gameTimer;
    private float timer;
    private bool isLookingForHidingSpots;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameTimer = gameDuration;
        timer = hidingTime;
        isLookingForHidingSpots = true;
    }

    private void Update()
    {
        if (timer > 0 && gameTimer == gameDuration)
        {
            timer -= Time.deltaTime;

            // Allow the Hider to look for hiding spots during the initial look duration.
            if (isLookingForHidingSpots && hiders.Count != 0)
            {
                foreach (AIController hider in hiders)
                {
                    hider.LookAround();
                }
            }
        }
        else if (timer <= 0 && gameTimer > 0)
        {
            isLookingForHidingSpots = false; //To Update it AIControlelr
            isGameOn = true;
            gameTimer -= Time.deltaTime;
            // Implement game logic during the main game duration.
            Debug.Log("Seeker, Find the Hider !");
            
        }
        else
        {
            // The game has ended. You can add game-ending logic here.
        }
    }

    private void Reset()
    {
        gameTimer = gameDuration;
        timer = hidingTime;        
    }

    public void SubscribeHiders(AIController transform)
    {
        hiders.Add(transform);
    }
    
    public void SubscribeSeekers(AIController transform)
    {
        seekers.Add(transform);
    }
}

