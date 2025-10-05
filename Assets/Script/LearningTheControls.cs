using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class LearningTheControls : MonoBehaviour
{
    public gameManager gameManager;
    public GameObject spawnPoint, playerClone, player, tutorialSpawn;
    public CinemachineVirtualCamera cam1, minicam;
    public playerControl control;
    public Text counter;
    public TextMeshProUGUI objective;

    public Queue<string> sentences;
    public dialogue dialogue;
    public float typingSpeed = 0.1f;
    public Text nameText, dialogueText;
    public GameObject DialogueBox, nextLine, endLine;
    public bool displayNext, ended, restart;

    public int audioIndex = 0;
    public List<AudioClip> audioClips;
    public AudioSource source, backgroundMusic;

    public Image up, down, left, right;
    public Sprite pressed, notPressed;
    public float numCount;

    public bool rotateLeft, rotateRight, moveForward, moveBackwards;

    public playerControl playerScript;
    private void Awake()
    {
        backgroundMusic.Play();
        nextLine.SetActive(false);
        endLine.SetActive(false);
        gameManager = FindObjectOfType<gameManager>();
        gameManager.enabled = false;
        counter.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        instantiatePlayer(spawnPoint);
        control = FindObjectOfType<playerControl>();
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        StartCoroutine(startTutorial());
    }

    public void instantiatePlayer(GameObject spawn)
    {
        Instantiate(playerClone, spawn.transform.position, Quaternion.identity);
        if (player != null)
        {
            Destroy(player);
        }
        player = FindObjectOfType<playerControl>().gameObject;
        player.name = "Character";
        cam1.Follow = player.transform;
        cam1.LookAt = player.transform.Find("mixamorig:Hips");
        minicam.Follow = player.transform.Find("MiniMapIcon");
        minicam.LookAt = player.transform.Find("MiniMapIcon");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            left.sprite = pressed;
        }
        else
        {
            left.sprite = notPressed;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            right.sprite = pressed;
        }
        else
        {
            right.sprite = notPressed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            up.sprite = pressed;
        }
        else
        {
            up.sprite = notPressed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            down.sprite = pressed;
        }
        else
        {
            down.sprite = notPressed;
        }

        if (rotateLeft)
        {
            counter.text = "000";
            if (numCount < 90 && control.hInput < 0)
            {
                numCount += 100f * Time.deltaTime;
                counter.text = Mathf.FloorToInt(numCount).ToString();
                if (numCount >= 90)
                {
                    counter.text = "90";
                    rotateLeft = false;
                    numCount = 0;
                }
            }
        }
        if (rotateRight)
        {
            counter.text = "000";
            if (numCount < 90 && control.hInput > 0)
            {
                numCount += 100f * Time.deltaTime;
                counter.text = Mathf.FloorToInt(numCount).ToString();
                if (numCount >= 90)
                {
                    counter.text = "90";
                    rotateRight = false;
                    numCount = 0;
                }
            }
        }
        if (moveForward)
        {
            counter.text = "000";
            if (numCount < 200 && control.vInput > 0)
            {
                numCount += 100f * Time.deltaTime;
                counter.text = Mathf.FloorToInt(numCount).ToString();
                if (numCount >= 200)
                {
                    counter.text = "200";
                    moveForward = false;
                    numCount = 0;
                }
            }
        }
        if (moveBackwards)
        {
            counter.text = "000";
            if (numCount < 200 && control.vInput < 0)
            {
                numCount += 100f * Time.deltaTime;
                counter.text = Mathf.FloorToInt(numCount).ToString();
                if (numCount >= 200)
                {
                    counter.text = "200";
                    moveBackwards = false;
                    numCount = 0;
                }
            }
        }
    }

    public IEnumerator startTutorial()
    {
        StartDialogue();
        yield return new WaitWhile(() => !ended);
        StartCoroutine(rotatingCharacter());
    }

    public IEnumerator rotatingCharacter()
    {
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        counter.gameObject.SetActive(true);
        control.enabled = true;
        rotateLeft = true;
        objective.text = "Rotate towards the left until the counter hits 90";
        yield return new WaitWhile(() => rotateLeft);
        objective.text = "";
        control.enabled = false;
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        counter.gameObject.SetActive(true);
        control.enabled = true;
        rotateRight = true;
        objective.text = "Rotate towards the right until the counter hits 90";
        yield return new WaitWhile(() => rotateRight);
        objective.text = "";
        control.enabled = false;
        StartCoroutine(movingCharacter());
    }
    public IEnumerator movingCharacter()
    {
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        counter.gameObject.SetActive(true);
        control.enabled = true;
        moveForward = true;
        objective.text = "Move forwards until you hit 200";
        yield return new WaitWhile(() => moveForward);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        counter.gameObject.SetActive(true);
        control.enabled = true;
        moveBackwards = true;
        objective.text = "Move backwards until you hit 200";
        yield return new WaitWhile(() => moveBackwards);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        StartCoroutine(finishTutorial());
    }

    public IEnumerator finishTutorial()
    {
        counter.text = "";
        objective.text = "";
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        yield return new WaitForSeconds(1f);
        instantiatePlayer(tutorialSpawn);
        gameManager.enabled = true;
    }

    public void StartDialogue()
    {
        sentences = new Queue<string>();
        DialogueBox.SetActive(true);
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
        if (!DialogueBox.activeSelf)
        {
            DialogueBox.SetActive(true);
        }
        string sentence = sentences.Dequeue();
        StartCoroutine(displayLine(sentence));
    }

    IEnumerator displayLine(string line)
    {
        ended = false;
        int letterIndex = 0;
        dialogueText.text = null;
        if (audioIndex <= audioClips.ToArray().Length - 1 && audioClips.ToArray().Length != 0)
        {
            source.clip = audioClips.ToArray()[audioIndex];
            source.Play();
        }
        foreach (char letter in line.ToCharArray())
        {
            letterIndex++;
            if (letter == '+')
            {
                displayNext = true;
            }
            else
            {
                dialogueText.text += letter;
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        if (displayNext)
        {
            yield return new WaitWhile(() => source.isPlaying);
            yield return new WaitForSeconds(1f);
            audioIndex++;
            displayNextSentence();
            displayNext = false;
        }
        else
        {
            yield return new WaitWhile(() => source.isPlaying);
            yield return new WaitForSeconds(1.5f);
            EndDialogue();
            audioIndex++;
        }
    }

    public void EndDialogue()
    {
        DialogueBox.SetActive(false);
        ended = true;
    }
}
