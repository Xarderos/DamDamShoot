using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadZoneController : MonoBehaviour
{
    public Transform[] waypoints;
    public float teleportInterval = 10f;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            StartCoroutine(TeleportRoutine());
        }
        else
        {
            Debug.LogWarning("No se han asignado waypoints al controlador de zonas de recarga.");
        }
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            TeleportToRandomWaypoint();
        }
    }

    private void TeleportToRandomWaypoint()
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        transform.position = waypoints[randomIndex].position;
        Debug.Log("Zona de recarga teletransportada a: " + waypoints[randomIndex].position);
    }
}
