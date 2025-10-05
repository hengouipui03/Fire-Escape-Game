using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.AI;
using System;
using TMPro;
public class gameManager : MonoBehaviour
{
    public AudioSource source;
    public AudioClip proceed, reminderAudio;
    public AudioClip[] audioClips;
    public int audioIndex = 0;

    public Transform cam2Follow, cam2LookAt;
    public GameObject guide, player, playerClone, map, lookat, mapUI, spawnPoint, teleportPoint;
    public Image mapImg;
    public Sprite ERT;
    public Button left, right;
    public NavMeshAgent nav;
    public Animator AiAnim;
    public int animIndex, checkpointIndex, mapI;
    public CinemachineVirtualCamera cam1, cam2, minicam;
    public GameObject[] checkpoints;
    public playerControl control;
    public Sprite[] mapImgs;

    public Text nameText, dialogueText, mapName;
    public TextMeshProUGUI objective;
    public GameObject nextLine, endLine, DialogueBox, exitSign;
    public Queue<string> sentences;
    public dialogue dialogue;
    public float typingSpeed = 0.1f;
    public bool reached, ended, leaveStartRoom;
    levelManagerScript levelManager;
    public AudioClip followAIreminder;

    private void Awake()
    {
        DialogueBox.gameObject.SetActive(false);
        sentences = new Queue<string>();
        levelManager = FindObjectOfType<levelManagerScript>();
        levelManager.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        StopAllCoroutines();
        control = FindObjectOfType<playerControl>();
        player = FindObjectOfType<playerControl>().gameObject;
        switchCam();
        DialogueBox.gameObject.SetActive(true);
        StartCoroutine(StartDialogue());        
        guide.transform.LookAt(player.transform);
        cam2Follow = cam2.Follow;
        cam2LookAt = cam2.LookAt;
        leaveStartRoom = true;
    }

    public void reminder()
    {
        source.clip = reminderAudio;
        source.Play();
    }

    public void switchCam()
    {
        if (cam1.Priority < cam2.Priority)
        {
            cam1.Priority = 10;
            cam2.Priority = 5;
        }
        else
        {
            cam1.Priority = 5;
            cam2.Priority = 10;
        }
    }

    public IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1f);
        DialogueBox.SetActive(true);
        player.SetActive(false);
        sentences.Clear();
        nameText.text = dialogue.name;
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        displayNextSentence();
    }

    public void displayNextSentence()
    {
        string sentence = sentences.Dequeue();
        StartCoroutine(displayLine(sentence));
        if (cam2.Follow == map.transform && sentences.Count <= 5) 
        {
            if (mapI < mapImgs.Length - 1)
            {
                mapI++;
                Debug.Log(mapI);
                mapImg.sprite = mapImgs[mapI];
                Debug.Log(mapImg.name);
                mapImg.SetNativeSize();
            }
        }
    }

    

    IEnumerator displayLine(string line)
    {
        dialogueText.text = null;
        nextLine.SetActive(false);
        endLine.SetActive(false);
        ended = false;
        if (audioIndex <= audioClips.Length-1)
        {
            source.clip = audioClips[audioIndex];
            source.Play();
        }
        if (animIndex <= AiAnim.parameterCount-2)
        {
            AiAnim.SetTrigger(AiAnim.parameters[animIndex].name);
        }
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        if (sentences.Count == 0)
        {
            yield return new WaitWhile(() => source.isPlaying);
            endLine.SetActive(true);
            audioIndex++;
        }
        else if(sentences.Count != 0)
        {
            yield return new WaitWhile(() => source.isPlaying);
            nextLine.SetActive(true);
            audioIndex++;
            animIndex++;
        }
    }
    public void EndDialogue()
    {
        switchCam();
        endLine.SetActive(false);
        DialogueBox.SetActive(false);
        player.SetActive(true);
        ended = true;
        if (leaveStartRoom)
        {
            StartCoroutine(exitRoom());
        }
        if (cam2.Follow == map.transform)
        {
            reminder();
            StartCoroutine(moveToExit());
        }
    }

    IEnumerator moveToExit()
    {
        objective.text = "Follow Brooke, your AI Guide.";
        cam2.Follow = cam2Follow;
        cam2.LookAt = cam2LookAt;
        StartCoroutine(moveToCheckpoint());
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, checkpoints[checkpointIndex].transform.position) > 1f);
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, player.transform.position) > 4f);
        guide.transform.LookAt(player.transform.position);
        guide.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        switchCam();
        player.SetActive(false);
        StartCoroutine(exitSignDialogue());
    }

    IEnumerator moveToCheckpoint()
    {
        checkpointIndex++;
        AiAnim.SetBool("walking", true);
        nav.SetDestination(checkpoints[checkpointIndex].transform.position);
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, checkpoints[checkpointIndex].transform.position) > 1f);
        AiAnim.SetBool("walking", false);
    }

    IEnumerator exitSignDialogue()
    {
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 3.5f;
        sentences.Enqueue("In every floor such as this one, there are neon-lighted exit signs all over to help guide you towards the various exit route.");
        DialogueBox.SetActive(true);
        displayNextSentence();
        GameObject[] exitSigns = GameObject.FindGameObjectsWithTag("exit sign");
        for (int i = 0; i < exitSigns.Length; i++)
        {
            cam2.Follow = exitSigns[i].transform;
            cam2.LookAt = exitSigns[i].transform;
            cam2.transform.rotation = Quaternion.LookRotation(exitSigns[i].transform.forward);
            yield return new WaitForSeconds(2f);
        }
        cam2.Follow = cam2Follow;
        cam2.LookAt = cam2LookAt;
        cam2.transform.rotation = Quaternion.Euler(0f,0f,0f);
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 1.27f;
        yield return new WaitWhile(() => !ended);
        guide.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        StartCoroutine(movetoEscape());
    }

    IEnumerator movetoEscape()
    {
        cam2.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60f;
        reminder();
        for (int i = 1; i < checkpoints.Length; i++)
        {
            nav.ResetPath();
            nav.SetDestination(checkpoints[i].transform.position);
            AiAnim.SetBool("walking", true);
            yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, checkpoints[i].transform.position) > 1f);
            AiAnim.SetBool("walking", false);
            float timer = 5f;
            while (Vector3.Distance(guide.transform.position, player.transform.position) > 3f)
            {
                Vector3 direction = player.transform.position - guide.transform.position;
                Quaternion target = Quaternion.LookRotation(direction);
                guide.transform.rotation = Quaternion.Lerp(guide.transform.rotation, target, Time.deltaTime * 2f);
                yield return null;
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = 5f;
                    source.clip = followAIreminder;
                    source.Play();
                }
            }
            yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, player.transform.position) > 3f);
        }
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, player.transform.position) > 6f);
        AiAnim.SetBool("walking", false);
        switchCam();
        cam2.transform.rotation = Quaternion.Euler(0f, -120f, 0f);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        cam2.transform.rotation = Quaternion.LookRotation(Vector3.left * 60f);
        sentences.Enqueue("Well done! You have passed the tutorial level! Please move towards the exit in front of you!");
        DialogueBox.SetActive(true);
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        StartCoroutine(teleportToMainLevel());
    }

    IEnumerator teleportToMainLevel()
    {
        objective.text = "Move towards the exit in front of you.";
        control.enabled = true;
        yield return new WaitWhile(() => Vector3.Distance(teleportPoint.transform.position, player.transform.position) > 4f);
        objective.text = "";
        Instantiate(playerClone, spawnPoint.transform.position, Quaternion.identity);
        Destroy(player);
        playerClone = FindObjectOfType<playerControl>().gameObject;
        playerClone.name = "Character";
        cam1.Follow = playerClone.transform;
        cam1.LookAt = playerClone.transform.Find("mixamorig:Hips");
        minicam.Follow = playerClone.transform.Find("MiniMapIcon");
        minicam.LookAt = playerClone.transform.Find("MiniMapIcon");
        levelManager.enabled = true;
        yield return null;
    }

    IEnumerator exitRoom()
    {
        objective.text = "Proceed towards the room in front of you";
        reminder();
        yield return new WaitWhile(() => source.isPlaying);
        leaveStartRoom = false;
        yield return new WaitForSeconds(0.5f);
        source.clip = proceed;
        source.Play();
        control.enabled = true;
        nav.SetDestination(checkpoints[checkpointIndex].transform.position);
        AiAnim.SetBool("walking", true);
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, checkpoints[checkpointIndex].transform.position) > 1f);
        nav.ResetPath();
        AiAnim.SetBool("walking", false);
        guide.transform.Rotate(Vector3.up * Time.deltaTime * 80f);
        guide.transform.rotation = Quaternion.Euler(0f, 165f, 0f);
        yield return new WaitWhile(() => Vector3.Distance(guide.transform.position, player.transform.position) > 5f);
        StartCoroutine(mapRoomFloorDialogue());
    }

    IEnumerator mapRoomFloorDialogue()
    {
        objective.text = "";
        switchCam();
        cam2.Follow = map.transform;
        cam2.LookAt = map.transform;
        cam2.transform.rotation = Quaternion.LookRotation(Vector3.left * 90f);
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 2.6f;
        cam2.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 70f;
        player.SetActive(false);
        sentences.Enqueue("This is the map room, in front of you shows the floor plan of every level in Sembcorp HQ. Click the 'Next' button to browse through the different floor plans.");
        sentences.Enqueue("BASEMENT 1");
        sentences.Enqueue("LEVEL 1");
        sentences.Enqueue("LEVEL 2");
        sentences.Enqueue("LEVEL 3");
        sentences.Enqueue("LEVEL 4");
        sentences.Enqueue("LEVEL 5");
        sentences.Enqueue("Whenever you are ready, do make your way towards the exit in front of you and I shall guide you to the end point.");
        DialogueBox.SetActive(true);
        displayNextSentence();
        yield return null;
    }
}