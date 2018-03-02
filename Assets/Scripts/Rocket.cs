﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100.0f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dying;
    [SerializeField] AudioClip levelComplete;
    [SerializeField] AudioClip explosion;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    bool collisionsDisabled = false;
    protected bool showJet = true;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();


    }

	// Update is called once per frame
	void Update () {
        // todo stop sound on death
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;

        }
    }

    private void LoadNextLevel()
    {

        SceneManager.LoadScene(1);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (isTransitioning || collisionsDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();

        //audioSource.PlayOneShot(explosion);


        audioSource.PlayOneShot(dying);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(levelComplete);
        successParticles.Play();

        Invoke("LoadNextScene", levelLoadDelay); // Parameterise time
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);   // Allow for more than 2 levels.
    }


    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {

        if (Input.GetKey(KeyCode.Space))   // Can thrust while rotating.
        {
            ApplyThrust();
            
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }      
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            
        }

        mainEngineParticles.Play();

    }

    private void RespondToRotateInput()
    {

        rigidBody.freezeRotation = true;  // Take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
}


