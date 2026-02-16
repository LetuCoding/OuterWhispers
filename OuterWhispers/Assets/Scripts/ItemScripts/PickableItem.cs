using System;
using InventoryScripts;
using UnityEditor;
using UnityEngine;
using Zenject;

public class PickableItem : MonoBehaviour
{
   [SerializeField] private ItemData item;
   [SerializeField] private AudioClip pickupSound;
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private GameObject pickupParticlesPrefab;
   
   private IAudioManager _audioManager;
   [Inject]
   public void Construct(IAudioManager audioManager)
   {
       _audioManager = audioManager;
   }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = item.Icon;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D");
        if (other.CompareTag("Player"))
        {
            Inventory inv = other.GetComponent<Inventory>();
            if (inv != null)
            {
                _audioManager.PlaySFX(pickupSound, audioSource, 1f);
                GameObject particles = Instantiate(
                    pickupParticlesPrefab,
                    transform.position,
                    Quaternion.identity
                );
                Destroy(particles, 2f);
                inv.AddItem(item);
                Destroy(gameObject);
            }
        }
    }
}
