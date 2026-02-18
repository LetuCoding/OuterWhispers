using InventoryScripts;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SimpleSaveSystem saveSystem;
    private Player player;
    private Inventory inventory;
    private HealthComponent healthComponent;
    private void Awake()
    {
        saveSystem = new SimpleSaveSystem();
        player = GetComponent<Player>();
        
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
            Vector3 loadedPos = saveSystem.Load();
            transform.position = loadedPos;
            Debug.Log($"[LOAD] Player moved to {loadedPos}");
        }
    }
}