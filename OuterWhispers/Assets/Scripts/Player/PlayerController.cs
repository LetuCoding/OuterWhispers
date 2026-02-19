using InventoryScripts;
using Items;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SimpleSaveSystem saveSystem;
    private Player player;
    private Inventory inventory;
    private HealthComponent healthComponent;
    private ItemDatabase itemDatabase;
    private void Awake()
    {
        saveSystem = new SimpleSaveSystem();
        player = GetComponent<Player>();
        itemDatabase = FindAnyObjectByType<ItemDatabase>();
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        // Presiona G para guardar posición
        if (Input.GetKeyDown(KeyCode.G))
        {
            saveSystem.Save(player, inventory);
        }

        // Presiona L para cargar posición
        if (Input.GetKeyDown(KeyCode.L))
        {
          
            saveSystem.Load(player, inventory, itemDatabase );
            
            Debug.Log($"[LOAD] Player moved to yes");
        }
    }
}