using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.EventSystems;

public class levelManagerScript : MonoBehaviour
{
    public GameObject[] redLight;
    public GameObject player, mapNorm, glassDoorLookAt, timerCheckpoint, leftDoor, RightDoor, staircaseExit, restartPoint, exitpoint;
    public Animator twinDoor;
    public playerControl control;
    public Image leftCross, rightCross, tick;
    public Sprite crossImg, tickImg;
    public Button left, right, correct;
    public CinemachineVirtualCamera cam1, cam2, minimapCam;
    public bool clicked, stopTimer;
    public Image mouse;
    public Sprite mouseClicked, mouseUnclicked;
    public LayerMask doorMask;

    public Queue<string> sentences;
    public dialogue dialogue;
    public float typingSpeed = 0.1f;
    public Text nameText, dialogueText;
    public GameObject nextLine, endLine, DialogueBox;
    public bool displayNext, ended, restart;

    public int audioIndex = 0;
    public List<AudioClip> audioClips;
    public AudioClip staircase, tryAgain, escapeMusic, reminderAudio, siren;
    public AudioSource source, backgroundMusic;

    public Slider slider;
    public Text timertext;
    public TextMeshProUGUI objective;
    public float startingTime = 10f, currentTime;

    public GameObject spawnpoint, gameOverScreen, whiteRoomSpawn;

    public ending endingScript;
    private void Awake()
    {
        slider.gameObject.SetActive(false);
        slider.enabled = false;
        gameOverScreen.gameObject.SetActive(false);
        endingScript = FindObjectOfType<ending>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StopAllCoroutines();
        backgroundMusic.clip = escapeMusic;
        backgroundMusic.Play();
        control = FindObjectOfType<playerControl>().gameObject.GetComponent<playerControl>();
        player = FindObjectOfType<playerControl>().gameObject;
        redLight = GameObject.FindGameObjectsWithTag("Flickering Lights");
        StartCoroutine(blinkingLights());
        StartCoroutine(beginningDialogue());
        StartCoroutine(mapCoroutine());
        mouse.enabled = false;
        leftCross.enabled = false;
        rightCross.enabled = false;
        tick.enabled = false;
        cam2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void stopAnim(Animator anim)
    {
        anim.speed = 0;
    }
    public void reminder()
    {
        source.clip = reminderAudio;
        source.Play();
    }
    
    IEnumerator beginningDialogue()
    {
        nextLine.SetActive(false);
        endLine.SetActive(false);
        yield return new WaitForSeconds(1f);
        StartDialogue();
        yield return new WaitWhile(() => !ended);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        source.PlayOneShot(siren);
        yield return new WaitWhile(() => source.isPlaying);
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        control.enabled = false;
        source.PlayOneShot(siren);
        yield return new WaitWhile(() => source.isPlaying);
        yield return new WaitForSeconds(2f);
        control.anim.SetFloat("PosZ", 0f);
        displayNextSentence();
        StopCoroutine(beginningDialogue());
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

    public void StartDialogue()
    {
        sentences = new Queue<string>();
        DialogueBox.SetActive(true);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
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
        control.enabled = true;
        ended = true;
    }

    IEnumerator blinkingLights()
    {
        while (redLight != null)
        {
            foreach (GameObject light in redLight)
            {
                light.GetComponent<Light>().range = Mathf.PingPong(4f * Time.time, 8f);
            }
            yield return null;
        }
    }

    public void showMap()
    {
        if (Vector3.Distance(mouse.transform.position, player.transform.position) < 4f)
        {
            objective.text = "";
            StartCoroutine(mapCoroutine());
        }
    }

    IEnumerator mapCoroutine()
    {
        objective.text = "Find the exit marked out by the exit signs";
        yield return new WaitWhile(() => !ended);
        yield return new WaitWhile(() => Vector3.Distance(mouse.transform.position, player.transform.position) > 5f);
        player.SetActive(false);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        mouse.enabled = false;
        cam2.Follow = mapNorm.transform;
        cam2.LookAt = mapNorm.transform;
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 1.3f;
        switchCam();
        yield return new WaitForSeconds(1f);
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 0.5f;
        leftCross.enabled = true;
        rightCross.enabled = true;
        tick.enabled = true;
        objective.text = "Select the exit route closest to you by selecting the highlighted box closest to the green dot (you).";
        StopCoroutine(mapCoroutine());
    }

    public void leftButton()
    {
        if (left.image != crossImg)
        {
            left.image.sprite = crossImg;
        }
    }
    public void rightButton()
    {
        if (right.image.sprite != crossImg)
        {
            right.image.sprite = crossImg;
        }
    }

    public void correctAnswer()
    {
        if (correct.image.sprite != tickImg)
        {
            objective.text = "";
            left.enabled = false;
            right.enabled = false;
            correct.enabled = false;
            StartCoroutine(correctIEnum());
        }
    }

    IEnumerator correctIEnum()
    {
        correct.image.sprite = tickImg;
        displayNextSentence();
        yield return new WaitForSeconds(5f);
        cam2.Follow = glassDoorLookAt.transform;
        cam2.LookAt = glassDoorLookAt.transform;
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 5f;
        twinDoor.SetTrigger("open");
        yield return new WaitForSeconds(twinDoor.GetCurrentAnimatorStateInfo(0).length + 1.5f);
        displayNextSentence();
        switchCam();
        player.SetActive(true);
        control.enabled = false;
        yield return new WaitWhile(() => !ended);
        control.enabled = true;
        objective.text = "Escape before the timer runs out.";
        StopCoroutine(correctIEnum());
        StartCoroutine(foundStaircase());
        StartCoroutine(startTimer());
    }

    IEnumerator startTimer()
    {
        currentTime = startingTime;
        slider.maxValue = startingTime;
        yield return new WaitWhile(() => Vector3.Distance(timerCheckpoint.transform.position, player.transform.position) > 1.5f);
        slider.gameObject.SetActive(true);
        slider.enabled = true;
        while (currentTime > 0.1f)
        {
            currentTime -= Time.deltaTime;
            float min = Mathf.FloorToInt(currentTime / 60);
            float sec = Mathf.FloorToInt(currentTime % 60);
            slider.value = currentTime;
            timertext.text = string.Format("{0:00}:{1:00}", min, sec);
            yield return null;
        }
        gameOverScreen.gameObject.SetActive(true);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        slider.gameObject.SetActive(false);
        slider.enabled = false;
        slider.maxValue = startingTime;
        StopAllCoroutines();
        StartCoroutine(blinkingLights());
    }

    public void restartGame()
    {
        StartCoroutine(restartGameIEnum());
        StartCoroutine(startTimer());
    }

    public void instantiatePlayer(GameObject point)
    {
        Instantiate(player, point.transform.position, Quaternion.identity);
        Destroy(player);
        player = FindObjectOfType<playerControl>().gameObject;
        control = FindObjectOfType<playerControl>();
        cam1.Follow = player.transform;
        cam1.LookAt = player.transform.Find("mixamorig:Hips");
        minimapCam.Follow = player.transform.Find("MiniMapIcon");
        minimapCam.LookAt = player.transform.Find("MiniMapIcon");
    }

    IEnumerator restartGameIEnum()
    {
        gameOverScreen.gameObject.SetActive(false);
        instantiatePlayer(restartPoint);
        yield return new WaitForSeconds(0.5f);
        sentences.Enqueue("Shall we try to move a little faster this time?");
        audioClips.Add(tryAgain);
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        audioClips.Remove(tryAgain);
        control.enabled = true;
        yield return null;
        StopCoroutine(restartGameIEnum());
        StartCoroutine(foundStaircase());
    }
    IEnumerator foundStaircase()
    {
        yield return new WaitWhile(() => Vector3.Distance(staircaseExit.transform.position, player.transform.position) > 5f);
        control.anim.SetFloat("PosZ", 0f);
        control.enabled = false;
        switchCam();
        cam2.Follow = staircaseExit.transform;
        cam2.LookAt = staircaseExit.transform;
        cam2.transform.rotation = Quaternion.LookRotation(Vector3.right * 90f);
        cam2.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 5f;
        sentences.Enqueue("Well Done! You managed to find the staircase exit, hurry now!");
        audioClips.Add(staircase);
        displayNextSentence();
        yield return new WaitWhile(() => !ended);
        audioClips.Remove(staircase);
        switchCam();
        control.enabled = true;
        objective.text = "Escape through the staircase exit.";
        StopCoroutine(foundStaircase());
    }
}
