using UnityEngine;
using System.Collections.Generic;

public class cauldron : Useable
{
    public List<Item> given = new List<Item>();
    [SerializeField]
    private Item finalpotion;
    private AudioSource m_Bubble;
    private AudioSource m_Plunk;
    private AudioSource m_PotionDrop;
    void Awake()
    {
        var m_Sources = GetComponents<AudioSource>();
        m_Bubble = m_Sources[0];
        m_Plunk = m_Sources[1];
        m_PotionDrop = m_Sources[2];

    }

    public override void Activate(int keyused)
    {
        if(!given.Contains(keys[keyused].key))
        {
            given.Add(keys[keyused].key);
            m_Plunk.Play();
            if(given.Count == keys.Length)
            {
                Success();
            }
        }
    }

    public void Success()
    {
        Instantiate(finalpotion.item_prefab, 
                transform.position + new Vector3(-0.5f, 0f, 0f), 
                Quaternion.identity);
        m_PotionDrop.Play();
    }
}