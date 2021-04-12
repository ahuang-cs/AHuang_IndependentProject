using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float initialSpeed = 10.0f;
    public float powerUpSpeed = 20.0f;
    public float powerUpSeconds = 8f;
    public Material poweredUpMaterial;
    [Header("Effects")]
    public AudioClip eatPowerUpSFX;
    public GameObject eatPowerUpParticleSystem;
    public AudioClip eatPowerUpTimerSFX;
    public AudioClip eatPickUpSFX;
    public float eatPickUpSFXPitch = 0.8f;
    public AudioClip eatEnemySFX;
    public GameObject eatEnemyParticleSystem;
    public AudioClip deathByEnemySFX;
    public GameObject deathByEnemyParticleSystem;

    private int curDirection = -1;

    private float speed;
    private float powerUpSecondsRemaining = 0f;
    private Material originalMaterial;
    private GameController gameController;
    private AudioSource[] audioSources;
    private MeshRenderer[] renderers;
    private Animator anim;
    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        speed = initialSpeed;
        anim = GetComponent<Animator>();
        mainCamera = GameObject.Find("Main Camera");
        renderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterial = renderers[0].material;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        audioSources = GetComponents<AudioSource>();
        audioSources[1].pitch = eatPickUpSFXPitch;
    }

    private void setRenderersMaterial(Material newMaterial)
    {
        foreach(MeshRenderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, 50f, transform.position.z - 20);
        
        if (powerUpSecondsRemaining > 0f)
        {
            setRenderersMaterial(poweredUpMaterial);
            powerUpSecondsRemaining -= Time.deltaTime;
        }
        else
        {
            setRenderersMaterial(originalMaterial);
            speed = initialSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("stop");
            anim.SetTrigger("right");
            curDirection = 90;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("stop");
            anim.SetTrigger("left");
            curDirection = 270;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("stop");
            anim.SetTrigger("up");
            curDirection = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger("stop");
            anim.SetTrigger("down");
            curDirection = 180;
        }

        if (curDirection >= 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, curDirection, 0), Time.deltaTime * speed);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
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
                playParticleSystemAndDestroy(eatPowerUpParticleSystem, new Vector3(other.gameObject.transform.position.x, 2, other.gameObject.transform.position.z));
                break;
            case "PowerUpTimer":
                other.gameObject.SetActive(false);
                Destroy(other.gameObject); // TODO: temporary for instantiateRepeating assignment requirement
                audioSources[1].PlayOneShot(eatPowerUpTimerSFX);
                gameController.EatPowerUpTimer();
                playParticleSystemAndDestroy(eatPowerUpParticleSystem, new Vector3(other.gameObject.transform.position.x, 2, other.gameObject.transform.position.z));
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
                    playParticleSystemAndDestroy(eatEnemyParticleSystem, new Vector3(collision.gameObject.transform.position.x, 2, collision.gameObject.transform.position.z));
                }
                else
                {
                    audioSources[0].PlayOneShot(deathByEnemySFX);
                    playParticleSystemAndDestroy(deathByEnemyParticleSystem, new Vector3(collision.gameObject.transform.position.x, 2, collision.gameObject.transform.position.z));
                    gameController.GameOver();
                    curDirection = -1;
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    private void playParticleSystemAndDestroy(GameObject prefab, Vector3 position)
    {
        GameObject ps = Instantiate(prefab, position, Quaternion.identity);
        ps.GetComponent<ParticleSystem>().Play();
        Destroy(ps, 5f);
    }
}
