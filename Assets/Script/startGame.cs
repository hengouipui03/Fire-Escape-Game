using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class startGame : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "burningFire.mp4");
        videoPlayer.Play();
    }

    public void switchScenes()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
