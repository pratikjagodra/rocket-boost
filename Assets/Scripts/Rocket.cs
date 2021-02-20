using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float thrustPower = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip thrustSound = null;
    [SerializeField] AudioClip deathSound = null;
    [SerializeField] AudioClip finishSound = null;

    [SerializeField] ParticleSystem thrustParticle = null;
    [SerializeField] ParticleSystem deathParticle = null;
    [SerializeField] ParticleSystem finishParticle = null;
    enum State { Alive, Dead, Transcending }
    State state = State.Alive;
    int currentIndex;
    int totalLevels;
    bool collisionDisable = false;

    Rigidbody rb;
    AudioSource audioSource;

    void Start()
    {
        currentIndex = SceneManager.GetActiveScene().buildIndex;
        totalLevels = SceneManager.sceneCountInBuildSettings;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotationInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugkey();
        }
    }

    private void RespondToDebugkey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisable = !collisionDisable;
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrust();
        }
        else
        {
            thrustParticle.Stop();
            audioSource.Stop();
        }
    }
    private void Thrust()
    {
        float thrustMultiplier = thrustPower * Time.deltaTime;
        rb.AddRelativeForce(Vector3.up * thrustMultiplier);
        if (!audioSource.isPlaying)
        {
            thrustParticle.Play();
            audioSource.PlayOneShot(thrustSound);
        }
    }
    private void RespondToRotationInput()
    {

        rb.freezeRotation = true;
        Rotate();
        rb.freezeRotation = false;
    }
    private void Rotate()
    {
        float rotationMultiplier = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationMultiplier);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationMultiplier);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionDisable)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    private void StartFinishSequence()
    {
        state = State.Transcending;
        audioSource.Stop();    //stops other sounds when finished
        thrustParticle.Stop();

        audioSource.PlayOneShot(finishSound);
        finishParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dead;
        audioSource.Stop();    //stops other sounds when dead
        thrustParticle.Stop();

        audioSource.PlayOneShot(deathSound);
        deathParticle.Play();
        Invoke("LoadCurrentLevel", levelLoadDelay);
    }
    private void LoadNextLevel()
    {
        currentIndex += 1;
        if(currentIndex < totalLevels)
        {
            SceneManager.LoadScene(currentIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }        
    }
    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(currentIndex);
    }
    /*private void LoadLevel()
    {
        print(currentLevel);
        SceneManager.LoadScene(currentLevel);
    } */
}
