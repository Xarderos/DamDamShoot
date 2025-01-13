using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    public Animator playerAnimator;
    private Vector3 lastPosition = Vector3.zero;
    private float movementThreshold = 0.1f; // Umbral m�nimo para detectar movimiento
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

        // Calcular la velocidad basada en la distancia recorrida desde la �ltima posici�n
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // Si la distancia recorrida supera el umbral, activar la animaci�n
        if (distanceMoved > movementThreshold)
        {
            playerAnimator.SetBool("isRunning", true);
        }

        // Cada intervalo de tiempo, actualizar la posici�n anterior y desactivar la animaci�n si no hay movimiento
        if (timer <= 0)
        {
            if (distanceMoved <= movementThreshold)
            {
                playerAnimator.SetBool("isRunning", false);
            }

            // Guardar la posici�n actual como la nueva posici�n anterior
            lastPosition = transform.position;

            // Reiniciar el temporizador
            timer = checkInterval;
        }
    }
}
