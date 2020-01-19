using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recording : Snapshot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SpriteRenderer microphoneIcon;
    [SerializeField] SpriteRenderer soundwavesIcon;

    private AudioSource audioSource;
    private AudioClip audioClip;

    private bool isPlayingback;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioClip(AudioClip _audioClip)
    {
        audioClip = _audioClip;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //audioSource.PlayOneShot(audioClip);
        StartPlayback();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //audioSource.Stop();
        StopPlayback();
    }

    public void StartPlayback()
    {
        if (!isPlayingback)
        {
            microphoneIcon.enabled = false;
            soundwavesIcon.enabled = true;
            audioSource.PlayOneShot(audioClip);
            isPlayingback = true;
        }
    }

    public void StopPlayback()
    {
        if (isPlayingback)
        {
            microphoneIcon.enabled = true;
            soundwavesIcon.enabled = false;
            audioSource.Stop();
            isPlayingback = false;
        }
    }
}