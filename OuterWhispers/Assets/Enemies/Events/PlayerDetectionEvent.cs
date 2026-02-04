using UnityEngine;
using System;

public class PlayerDetectionEvent : MonoBehaviour
{
    public static event Action<Transform> OnPlayerDetected;

    public static void Trigger(Transform player)
    {
        OnPlayerDetected?.Invoke(player);
    }
}