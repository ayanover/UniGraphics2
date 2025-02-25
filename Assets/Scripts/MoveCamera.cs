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
        // Pobierz przesunięcie twarzy
        Vector3 faceOffset = faceController != null ? faceController.faceOffset : Vector3.zero;

        // Ustaw pozycję kamery (gracz + lokalna pozycja + przesunięcie twarzy)
        transform.position = playerPosition.position + cameraPosition.localPosition + faceOffset;

        // Ustaw rotację kamery
        transform.rotation = cameraRotation.rotation;
    }
}
