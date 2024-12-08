using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float senseX = 100f; // Czu³oœæ myszki w poziomie
    public float senseY = 100f; // Czu³oœæ myszki w pionie

    public Transform orientation; // Odniesienie do obiektu orientacji gracza
    public Transform playerBody; // Cia³o gracza (np. jego model)

    private float xRotation;
    private float yRotation;

    public float smoothTime = 0.3f; // Czas wyg³adzania (im mniejszy, tym szybsza reakcja)
    private float currentXRotation; // Aktualny obrót w osi X
    private float currentYRotation; // Aktualny obrót w osi Y
    private float xRotationVelocity; // Do SmoothDamp
    private float yRotationVelocity; // Do SmoothDamp

    public Transform cameraPosition; // Obiekt reprezentuj¹cy kamerê w przestrzeni gracza
    public Vector3 cameraOffset; // Offset kamery w stosunku do gracza

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ukryj kursor i zablokuj go w oknie
        Cursor.visible = false;
    }

    private void Update()
    {
        // Dane wejœciowe z myszki
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senseY;

        // Dodaj wartoœci od myszki
        yRotation += mouseX;
        xRotation -= mouseY; // Odwrotnie, poniewa¿ Unity tak interpretuje oœ Y

        // Ograniczanie obrotu w osi X
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Wyg³adzanie obrotu w osi X
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVelocity,smoothTime);

        // Wyg³adzanie obrotu w osi Y
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationVelocity,smoothTime);

        // Ustawienie rotacji kamery
        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);

        // Ustawienie rotacji orientacji gracza
        orientation.rotation = Quaternion.Euler(0, currentYRotation, 0);
        playerBody.rotation = Quaternion.Euler(0, currentYRotation, 0);


        // Obliczanie nowej pozycji kamery z uwzglêdnieniem offsetu
        // Offset kamery wzglêdem cia³a gracza (np. za plecami)
        Vector3 targetCameraPosition = playerBody.position + playerBody.forward * cameraOffset.z + playerBody.right * cameraOffset.x + Vector3.up * cameraOffset.y;

        // Aktualizacja pozycji kamery z wyg³adzaniem
        cameraPosition.position = Vector3.Lerp(cameraPosition.position, targetCameraPosition, Time.deltaTime * smoothTime);
    }
}
