using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Powerup;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject powerUpIndicator;
    [SerializeField] private GameObject focalPoint;
    [SerializeField] private GameObject smash;
    [SerializeField] private GameObject rocketPrefab;

    readonly private float speed = 500f;
    readonly private float powerUpStrength = 15.0f;
    readonly private float fireRate = 0.4f;
    private float nextFire = 0.0f;

    private int enemyCount;

    private Rigidbody playerRb;
    private bool hasPowerUp;
    private Vector3 offSet = new(0, -0.5f, 0f);
    private PowerupType currentPowerUp = PowerupType.None;
    private Coroutine powerupCountdown;


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        powerUpIndicator.transform.position = transform.position + offSet;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        Movement();
        RocketFire();
        Smash();
    }

    private void Movement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(forwardInput * speed * Time.deltaTime * focalPoint.transform.forward);
    }

    private void RocketFire()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentPowerUp == PowerupType.Rockets && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            LaunchRockets();
        }
    }

    private void Smash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentPowerUp == PowerupType.Smash && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            transform.position = new Vector3(transform.position.x, 6, transform.position.z);
            playerRb.AddForce(-Vector3.up * 20f, ForceMode.Impulse);
            smash.SetActive(true);
            StartCoroutine(SmashCountDown());
        }
    }

    IEnumerator SmashCountDown()
    {
        yield return new WaitForSeconds(1);
        smash.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            currentPowerUp = other.gameObject.GetComponent<Powerup>().powerupType;
            powerUpIndicator.SetActive(true);
            hasPowerUp = true;
            Destroy(other.gameObject);

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerUpCountdownRoutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerupType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
        }
    }

    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerUp = false;
        powerUpIndicator.SetActive(false);
        currentPowerUp = PowerupType.None;
    }

    private void LaunchRockets()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 offset = new(0, 2f, 0);
            GameObject tmpRocket = Instantiate(rocketPrefab, transform.position + offset, Quaternion.identity);
            tmpRocket.GetComponent<Rocket>().Fire(enemy.transform);
        }
    }
}