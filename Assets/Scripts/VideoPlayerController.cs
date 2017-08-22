using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour {
    VideoPlayer videoPlayer;
    GvrAudioSource audioSource;
    public GameObject hints;

    // Use this for initialization
    void Start () {
        videoPlayer = GetComponent<VideoPlayer>();

        audioSource = GetComponent<GvrAudioSource>();

        // Each time we reach the end, we slow down the playback by a factor of 10.
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        videoPlayer.started += VideoPlayer_started;
        hints.SetActive(false);
    }

    private void VideoPlayer_started(VideoPlayer source)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        hints.SetActive(false);
        gameObject.SetActive(false);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
            hints.SetActive(true);
        }
    }

    public void ResetVideo()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            hints.SetActive(false);
        }
    }
}
