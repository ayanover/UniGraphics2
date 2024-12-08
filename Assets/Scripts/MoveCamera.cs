using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform playerPosition;
    public Transform cameraRotation;
    public FaceCameraController faceController; // Referencja do FaceCameraController

    private void LateUpdate()
    {
        // Pobierz przesuniêcie twarzy
        Vector3 faceOffset = faceController != null ? faceController.faceOffset : Vector3.zero;

        // Ustaw pozycjê kamery (gracz + lokalna pozycja + przesuniêcie twarzy)
        transform.position = playerPosition.position + cameraPosition.localPosition + faceOffset;

        // Ustaw rotacjê kamery
        transform.rotation = cameraRotation.rotation;
    }
}
