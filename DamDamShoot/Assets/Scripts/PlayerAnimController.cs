using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    public Animator playerAnimator;
    private Vector3 lastPosition = Vector3.zero;
    private float movementThreshold = 0.1f; // Umbral mínimo para detectar movimiento
    private float checkInterval = 0.1f; // Intervalo de tiempo entre actualizaciones
    private float timer = 0;

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Actualizar el temporizador
        timer -= Time.deltaTime;

        // Calcular la velocidad basada en la distancia recorrida desde la última posición
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // Si la distancia recorrida supera el umbral, activar la animación
        if (distanceMoved > movementThreshold)
        {
            playerAnimator.SetBool("isRunning", true);
        }

        // Cada intervalo de tiempo, actualizar la posición anterior y desactivar la animación si no hay movimiento
        if (timer <= 0)
        {
            if (distanceMoved <= movementThreshold)
            {
                playerAnimator.SetBool("isRunning", false);
            }

            // Guardar la posición actual como la nueva posición anterior
            lastPosition = transform.position;

            // Reiniciar el temporizador
            timer = checkInterval;
        }
    }
}
