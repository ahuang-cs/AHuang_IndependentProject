using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    public float secondsPerLevel = 120f;
    public Vector3 playerInitialPosition = new Vector3(40f, 2f, 40f);
    public GameObject playerPrefab;
    public GameObject powerUpPrefab;
    public GameObject powerUpTimerPrefab;
    public GameObject powerUpForcefieldPrefab;
    public GameObject pickUpPrefab;
    public GameObject enemyPrefab;
    public static int numEnemies = 4;
    public float enemyRespawnSeconds = 8f;
    public float powerUpTimerRegenerateSeconds = 45f;
    public float powerUpTimerAddSeconds = 5f;
    public float powerUpForcefieldRegenerateSeconds = 60f;

    [Header("Score Settings")]
    public int enemyValue = 5;
    public int pickUpValue = 1;
    public int powerUpValue = 0;

    [Header("UI Settings")]
    public GameObject titleUI;
    public GameObject scoreUIText;
    public GameObject timerUIText;

    private GameObject player;
    private GameObject[] enemies = new GameObject[numEnemies];
    private float[] enemyRespawnSecondsRemaining = new float[numEnemies];
    private GameObject[,] powerUps = new GameObject[2, 2];
    // 29row x 26col pick up grid
    private GameObject[,] pickUps = new GameObject[26, 30];
    private int rows = 26;
    private int cols = 29;
    private int numPickups = 0;
    private int pickupsRemaining;

    private float xOrigin = 4.75f;
    private float yOrigin = 1.75f;
    private float zOrigin = 3.5f;
    private float xOffset = 2.82f;
    private float zOffset = 2.35f;

    private int score = 0;
    private float secondsRemainingInLevel;

    private Text titleText;
    private Text timerText;
    private Text scoreText;
    private bool gameOver = true;

    // Start is called before the first frame update
    void Start()
    {
        titleText = titleUI.GetComponent<Text>();
        scoreText = scoreUIText.GetComponent<Text>();
        timerText = timerUIText.GetComponent<Text>();

        initializePlayer();
        initializePowerUps();
        initializePickUps();
        initializeEnemies();
    }
    private IEnumerator powerUpForcefieldGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(powerUpForcefieldRegenerateSeconds);
            Instantiate(powerUpForcefieldPrefab, getRandomPickupPosition(), powerUpForcefieldPrefab.transform.rotation);
        }
    }

    public void AddToSecondsRemainingInLevel(float secs)
    {
        secondsRemainingInLevel += secs;
        timerText.text = "Timer: " + (int) secondsRemainingInLevel;
    }
    public void EatEnemy()
    {
        AddToScore(enemyValue);
    }
    public void EatPickUp()
    {
        AddToScore(pickUpValue);
        pickupsRemaining--;
        checkWinCondition();
    }
    private void checkWinCondition()
    {
        if (pickupsRemaining <= 0)
        {
            GameOver();
        }
    }
    public void EatPowerUp()
    {
        AddToScore(powerUpValue);
    }

    public void EatPowerUpTimer()
    {
        AddToSecondsRemainingInLevel(powerUpTimerAddSeconds);
    }

    private void GeneratePowerUpTimer()
    {
        Instantiate(powerUpTimerPrefab, playerInitialPosition, Quaternion.identity);
    }

    private void AddToScore(int newScore)
    {
        if (!gameOver)
        {
            score += newScore;
            scoreText.text = "Score: " + score;
        }
    }
    public void GameOver()
    {
        StopCoroutine(powerUpForcefieldGenerator());
        CancelInvoke("GeneratePowerUpTimer");

        if (secondsRemainingInLevel > 0f)
        {
            AddToScore((int)Mathf.Floor(secondsRemainingInLevel));
        }
        gameOver = true;
        titleText.text = "Game Over!";
        titleUI.SetActive(true);
    }
    public void ResetLevel()
    {
        StartCoroutine(powerUpForcefieldGenerator());
        InvokeRepeating("GeneratePowerUpTimer", powerUpTimerRegenerateSeconds, powerUpTimerRegenerateSeconds);

        titleUI.SetActive(false);
        score = 0;
        scoreText.text = "Score: " + score;
        secondsRemainingInLevel = secondsPerLevel;
        timerText.text = "Timer: " + secondsRemainingInLevel;

        for (int i = 0; i < numEnemies; i++)
        {
            enemyRespawnSecondsRemaining[i] = 0f;
        }
        secondsRemainingInLevel = secondsPerLevel;
        player.SetActive(true);
        player.transform.position = playerInitialPosition;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        foreach (GameObject powerUp in powerUps)
        {
            powerUp.SetActive(true);
        }
        foreach (GameObject pickUp in pickUps)
        {
            if (pickUp != null)
            {
                pickUp.GetComponent<Renderer>().enabled = true;
            }
        }
        pickupsRemaining = numPickups;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver && secondsRemainingInLevel > 0f)
        {
            AddToSecondsRemainingInLevel(-Time.deltaTime);
        }
        else if (!gameOver && secondsRemainingInLevel <= 0f)
        {
            GameOver();
        }

        for (int i = 0; i < numEnemies; i++)
        {
            // dead and respawn timer active
            if (!enemies[i].activeSelf && enemyRespawnSecondsRemaining[i] > 0f)
            {
                enemyRespawnSecondsRemaining[i] -= Time.deltaTime;
            }
            // dead and respawn timer up
            else if (!enemies[i].activeSelf && enemyRespawnSecondsRemaining[i] <= 0f)
            {
                enemyRespawnSecondsRemaining[i] = enemyRespawnSeconds;
                enemies[i].transform.position = getRandomPickupPosition();
                enemies[i].SetActive(true);
            }
        }
    }

    private Vector3 getRandomPickupPosition()
    {
        GameObject randomPickUp = null;
        do
        {
            try
            {
                randomPickUp = pickUps[UnityEngine.Random.Range(0, cols - 1), UnityEngine.Random.Range(0, rows - 1)];
                if(!randomPickUp.activeSelf)
                {
                    randomPickUp = null;
                }
            }
            catch (Exception e)
            {
                // do nothing
                e.ToString();
            }
        } while (randomPickUp == null);
        return randomPickUp.transform.position;
    }
    private void initializePlayer()
    {
        player = (GameObject)Instantiate(playerPrefab, playerInitialPosition, Quaternion.identity);
        player.SetActive(false);
    }
    private void initializeEnemies()
    {
        for(int i = 0; i < numEnemies; i++)
        {
            enemies[i] = (GameObject)Instantiate(enemyPrefab, getRandomPickupPosition(), Quaternion.identity);
            enemyRespawnSecondsRemaining[i] = enemyRespawnSeconds;
        }
    }
    private void initializePowerUps()
    {
        powerUps[0, 0] = (GameObject)Instantiate(powerUpPrefab, new Vector3(16.184f, 1.75f, 10.375f), Quaternion.identity);
        powerUps[0, 1] = (GameObject)Instantiate(powerUpPrefab, new Vector3(64.125f, 1.75f, 10.375f), Quaternion.identity);
        powerUps[1, 0] = (GameObject)Instantiate(powerUpPrefab, new Vector3(16.184f, 1.75f, 62.625f), Quaternion.identity);
        powerUps[1, 1] = (GameObject)Instantiate(powerUpPrefab, new Vector3(64.125f, 1.75f, 62.625f), Quaternion.identity);
    }
    private void initializePickUps()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                //don't place in center (rows 13-19 and cols 9-18)
                if(col >= 13 && col <= 19 && row >= 9 && row <= 16)
                {
                    continue;
                }
                Vector3 pickUpCoordinates = new Vector3(xOrigin + row * xOffset, yOrigin, zOrigin + col * zOffset);
                if (!Physics.CheckBox(pickUpCoordinates, new Vector3(0.6f, 0.6f, 0.6f)))
                {
                    pickUps[row, col] = (GameObject)Instantiate(pickUpPrefab, pickUpCoordinates, Quaternion.identity);
                    numPickups++;
                }
            }
        }
    }
}
