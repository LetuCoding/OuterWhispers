using System;
using InventoryScripts;
using UnityEditor;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
   [SerializeField] private ItemData item;
   
   
   
   
   
   
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
                inv.AddItem(item);
                Destroy(gameObject);
            }
        }
    }
}
