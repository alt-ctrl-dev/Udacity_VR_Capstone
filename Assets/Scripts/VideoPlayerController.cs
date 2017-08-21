using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour {
    VideoPlayer videoPlayer;
    public GameObject hints;

    // Use this for initialization
    void Start () {
        videoPlayer = GetComponent<VideoPlayer>();

        // Each time we reach the end, we slow down the playback by a factor of 10.
        videoPlayer.loopPointReached += onVideoEndReached;
        hints.SetActive(false);
    }

    private void onVideoEndReached(VideoPlayer source)
    {
        hints.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PlayVideo()
    {
        if (videoPlayer == null)
        {
            if (!videoPlayer.isPlaying) videoPlayer.Play();
            hints.SetActive(true);
        }
    }

    public void ResetVideo()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying) videoPlayer.Stop();
            hints.SetActive(false);
        }
    }
}
