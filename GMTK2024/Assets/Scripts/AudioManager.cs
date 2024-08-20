using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<SFX> sfxLibrary = new List<SFX>();
    private AudioSource source;

    private static AudioManager instance = null;
    public static AudioManager Instance {  get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
            Debug.LogWarning(gameObject.name + " had an extra AudioManager, which was deleted");
        }
        source = GetComponent<AudioSource>();
    }

    public void PlaySFX(string sfxName)
    {
        foreach(SFX sfx in sfxLibrary)
        {
            if(sfx.Name == sfxName)
            {
                source.PlayOneShot(sfx.Clip, sfx.VolumeModifier);
                return;
            }
        }
    }
}

[System.Serializable]
public struct SFX
{
    [SerializeField] private string sfxName;
    [SerializeField] private AudioClip sfxClip;
    [SerializeField] private float volumeModifier;

    public string Name { get { return sfxName; } }
    public AudioClip Clip { get { return sfxClip; } }
    public float VolumeModifier { get {  return volumeModifier; } }
}