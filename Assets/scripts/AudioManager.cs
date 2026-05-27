using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private AudioSource m_woodwalk;
    [SerializeField] private AudioSource m_carpetwalk;
    [SerializeField] private AudioSource m_hardwalk;
    [SerializeField] private AudioSource m_grasswalk;

    [Header("Light")]
    [SerializeField] private AudioSource m_flashlightON;
    [SerializeField] private AudioSource m_flashlightOFF;

    [Header("Pickups")]
    [SerializeField] private AudioSource m_pickupSource;
    [SerializeField] private AudioClip[] m_pickupClips;

    [Header("Drops")]
    [SerializeField] private AudioSource m_dropSource;
    [SerializeField] private AudioClip[] m_dropClips;

    [Header("Door")]
    [SerializeField] private AudioSource m_DoorOpen;
    [SerializeField] private AudioSource m_DoorClose;

    public void PlayRandomPickup()
    {
        //Set up and play an RNG clip index for our Pickup Source
        int clip = Random.Range(0, m_pickupClips.Length);
        m_pickupSource.pitch = Random.Range(0.9f, 1.1f);
        m_pickupSource.PlayOneShot(m_pickupClips[clip]);

    }

    public void PlayRandomDrop()
    {
        //Set up and play an RNG clip index for our Pickup Source
        int clip = Random.Range(0, m_dropClips.Length);
        m_dropSource.pitch = Random.Range(0.9f, 1.1f);
        m_dropSource.PlayOneShot(m_dropClips[clip]);

    }

    public void DoorClopen(bool o)
    {
        if (o)
        {
            m_DoorClose.Play();
        }
        else
        {
            m_DoorOpen.Play();
        }
    }

    //TODO: CHANGE WALKING SFX BASED ON GROUND TYPE
    //TODO: GHOST TALK SFX

}