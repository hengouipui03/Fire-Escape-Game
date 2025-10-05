using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class playerControl : MonoBehaviour
{
    public Rigidbody rig;
    public CharacterController controller;
    public Animator anim;
    public float rotSpeed = 100f, xRotation, moveSpeed, hInput, vInput;
    public int min, max;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        playerMovement();
    }

    private void playerMovement()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 target = new Vector3(0f, hInput * Time.deltaTime * rotSpeed, 0f);
        transform.Rotate(target);
        Vector3 moveForward = transform.forward * vInput;
        controller.Move(moveForward * Time.deltaTime * moveSpeed);
        anim.SetFloat("PosZ", vInput/2, 0.05f, Time.deltaTime);
    }

    public void footsteps()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
}
