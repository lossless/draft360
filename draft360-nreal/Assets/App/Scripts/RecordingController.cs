using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingController : MonoBehaviour
{
    AudioClip tempRecording;

    string microphoneDevice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartRecording()
    {
        tempRecording = Microphone.Start(Microphone.devices[0], true, 10, 44100);
    }

    private void EndRecording()
    {
        Microphone.End(Microphone.devices[0]);



    }

    private void GetMicrophoneDevice()
    {

    }

}
