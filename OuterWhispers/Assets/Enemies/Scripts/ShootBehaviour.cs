using System;
using UnityEngine;
using System.Collections;
using Enemies.Interfaces;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float timeBetweenShots = 1f;

    private void Start()
    {
        StartCoroutine(Shoot());
    }


    IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenShots);
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        }
    }
}