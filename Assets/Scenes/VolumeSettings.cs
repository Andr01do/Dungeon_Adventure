using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;    

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer; 

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}