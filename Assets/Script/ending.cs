using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
public class ending : MonoBehaviour
{
    levelManagerScript levelManager;
    public GameObject door, videoPlayer, bigScreen, whiteRoomRespawn, AIClone, aiSpawn, player, UICanvas, endingCanvas;
    public Animator lastDoorAnim;
    public AudioClip leave;
    public AudioSource source;
    public void Awake()
    {
        endingCanvas.SetActive(false);
        levelManager = FindObjectOfType<levelManagerScript>();
        bigScreen.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            levelManager.objective.text = "";
            levelManager.StopAllCoroutines();
            levelManager.backgroundMusic.Stop();
            player = FindObjectOfType<playerControl>().gameObject;
            StartCoroutine(endingIEnum(player));
        }
    }

    IEnumerator endingIEnum(GameObject player)
    {
        player.GetComponent<playerControl>().enabled = false;
        player.GetComponent<playerControl>().anim.SetFloat("PosZ", 0f);
        lastDoorAnim.SetTrigger("open");
        yield return new WaitForSeconds(lastDoorAnim.GetCurrentAnimatorStateInfo(0).length + 1.5f);
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        levelManager.control.enabled = false;
        UICanvas.SetActive(false);
        endingCanvas.SetActive(true);
        source.clip = leave;
        source.Play();
        yield return new WaitForSeconds(4f);
    }

}
