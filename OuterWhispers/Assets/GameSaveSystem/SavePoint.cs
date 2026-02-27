using UnityEngine;
using Zenject;

public class SavePoint : MonoBehaviour
{
    [Inject] private Player _player;
    
    [SerializeField] private string requiredItemName = "Ink";
    public SaveMenuManager SaveMenuManager;

    private bool _playerInside;

    void Update()
    {
        if (!_playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveMenuManager.Open();
        }
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