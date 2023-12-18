using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{
    private bool isReady = false;

    private void Start()
    {
        StartCoroutine(WaitForVideo());
    }
    void Update()
    {
        if(!GetComponent<VideoPlayer>().isPlaying && isReady)
        {
            SceneManager.LoadScene("MainScene");;
        }
    }

    private IEnumerator WaitForVideo()
    {
        yield return new WaitForSeconds(1);
        isReady = true;
    }
}
