using InventoryScripts;
using Items;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private OuterWhispersSaveSystem _outerWhispersSaveSystem;
    private Player player;
    private Inventory inventory;
    private HealthComponent healthComponent;
    private ItemDatabase itemDatabase;
    private void Awake()
    {
        _outerWhispersSaveSystem = new OuterWhispersSaveSystem();
        player = GetComponent<Player>();
        itemDatabase = FindAnyObjectByType<ItemDatabase>();
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        // Presiona G para guardar posición
        if (Input.GetKeyDown(KeyCode.G))
        {
            _outerWhispersSaveSystem.saveData(player, inventory);
        }

        // Presiona L para cargar posición
        if (Input.GetKeyDown(KeyCode.L))
        {
          
            _outerWhispersSaveSystem.LoadData(player, inventory, itemDatabase );
            
            Debug.Log($"[LOAD] Player moved to yes");
        }
    }
}