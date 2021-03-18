using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float initialSpeed = 10.0f;
    public float powerUpSpeed = 20.0f;
    public float powerUpSeconds = 8f;
    public Material poweredUpMaterial;
    [Header("Audio Clips")]
    public AudioClip eatPowerUpSFX;
    public AudioClip eatPowerUpTimerSFX;
    public AudioClip eatPickUpSFX;
    public float eatPickUpSFXPitch = 0.8f;
    public AudioClip eatEnemySFX;
    public AudioClip deathByEnemySFX;

    private Vector3 curDirection = Vector3.zero;

    private float speed;
    private float powerUpSecondsRemaining = 0f;
    private Material originalMaterial;
    private GameController gameController;
    private AudioSource[] audioSources;
    
    // Start is called before the first frame update
    void Start()
    {
        speed = initialSpeed;
        originalMaterial = GetComponent<MeshRenderer>().material;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        audioSources = GetComponents<AudioSource>();
        audioSources[1].pitch = eatPickUpSFXPitch;
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
            speed = initialSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) curDirection = Vector3.right;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) curDirection = Vector3.left;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) curDirection = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) curDirection = Vector3.back;

        transform.Translate(curDirection * Time.deltaTime * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "PickUp":
                Renderer pickupRenderer = other.gameObject.GetComponent<Renderer>();
                if (pickupRenderer.enabled)
                {
                    pickupRenderer.enabled = false;
                    audioSources[0].PlayOneShot(eatPickUpSFX);
                    gameController.EatPickUp();
                }
                break;
            case "PowerUp":
                other.gameObject.SetActive(false);
                audioSources[1].PlayOneShot(eatPowerUpSFX);
                gameController.EatPowerUp();
                speed = powerUpSpeed;
                powerUpSecondsRemaining = powerUpSeconds;
                break;
            case "PowerUpTimer":
                other.gameObject.SetActive(false);
                Destroy(other.gameObject); // TODO: temporary for instantiateRepeating assignment requirement
                audioSources[1].PlayOneShot(eatPowerUpTimerSFX);
                gameController.EatPowerUpTimer();
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
                    audioSources[0].PlayOneShot(eatEnemySFX);
                    collision.gameObject.SetActive(false);
                    gameController.EatEnemy();
                }
                else
                {
                    audioSources[0].PlayOneShot(deathByEnemySFX);
                    gameController.GameOver();
                }
                break;
/*            case "Wall":
                curDirection = Vector3.zero;
                break;
*/
        }
    }
}
