using UnityEngine;
using Zenject;

public class SavePoint : MonoBehaviour
{
    [Inject] private Player _player;

    private OuterWhispersSaveSystem _saveSystem;

    [SerializeField] private string requiredItemName = "Ink";

    private bool _playerInside;

    void Start()
    {
        _saveSystem = new OuterWhispersSaveSystem();
    }

    void Update()
    {
        if (!_playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TrySave();
        }
    }

    private void TrySave()
    {
        if (!_player.Inventory.CheckItemByName(requiredItemName))
        {
            Debug.Log("Necesitas Ink Ribbon para guardar.");
            return;
        }

        _saveSystem.saveData(_player, _player.Inventory);

        _player.Inventory.RemoveItemByName(requiredItemName);

        Debug.Log("Game Saved.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            _playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            _playerInside = false;
        }
    }
}