using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float thrust = 20.0f;
    public float powerUpSeconds = 8f;
    public Material poweredUpMaterial;
    [Header("Audio Clips")]
    public AudioClip eatPowerUpSFX;
    public AudioClip eatPickUpSFX;
    public AudioClip eatEnemySFX;
    public AudioClip deathByEnemySFX;

    private Rigidbody rb;
    private float movementX;
    private float movementY;

    private float powerUpSecondsRemaining = 0f;
    private Material originalMaterial;
    private GameController gameController;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalMaterial = GetComponent<MeshRenderer>().material;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (powerUpSecondsRemaining > 0f)
        {
            GetComponent<MeshRenderer>().material = poweredUpMaterial;
            powerUpSecondsRemaining -= Time.deltaTime;
        }
        else
        {
            GetComponent<MeshRenderer>().material = originalMaterial;
        }
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x + 5, 15f, gameObject.transform.position.z - 15);
        movementX = Input.GetAxis("Horizontal");
        movementY = Input.GetAxis("Vertical");

        rb.AddForce(new Vector3(movementX, 0f, movementY) * thrust);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "PickUp":
                other.gameObject.SetActive(false);
                audio.PlayOneShot(eatPickUpSFX);
                gameController.EatPickUp();
                break;
            case "PowerUp":
                other.gameObject.SetActive(false);
                audio.PlayOneShot(eatPowerUpSFX);
                gameController.EatPowerUp();
                powerUpSecondsRemaining = powerUpSeconds;
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                if (powerUpSecondsRemaining > 0f)
                {
                    audio.PlayOneShot(eatEnemySFX);
                    collision.gameObject.SetActive(false);
                    gameController.EatEnemy();
                }
                else
                {
                    audio.PlayOneShot(deathByEnemySFX);
                    gameController.GameOver();
                }
                break;
        }
    }
}
