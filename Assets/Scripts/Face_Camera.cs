using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

public class FaceCameraController : MonoBehaviour
{
    public float smoothness = 5f;
    public float maxHorizontalMovement = 2f;
    public float maxVerticalMovement = 1f;
    public float maxDepthMovement = 2f;

    private UdpClient udpClient;
    private const int port = 5065;
    private Vector3 targetPosition;
    private Vector3 smoothedTargetPosition;
    public Vector3 faceOffset { get; private set; } // Przesuniêcie twarzy

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.Client.Blocking = false;
        Debug.Log("UDP client listening on port " + port);
    }

    void Update()
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log("Received: " + message);

                string[] parts = message.Split(',');
                if (parts.Length == 3 &&
                    float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedX) &&
                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedY) &&
                    float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedZ))
                {
                    targetPosition = new Vector3(receivedX, receivedY, receivedZ);
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket error: " + e.ToString());
        }

        // Oblicz przesuniêcie twarzy na podstawie danych z Pythona
        float x = Mathf.Lerp(-maxHorizontalMovement, maxHorizontalMovement, (targetPosition.x + 1) / 2);
        float y = Mathf.Lerp(-maxVerticalMovement, maxVerticalMovement, (targetPosition.y + 1) / 2);
        float z = Mathf.Lerp(-maxDepthMovement, 0, targetPosition.z);

        smoothedTargetPosition = Vector3.Lerp(smoothedTargetPosition, new Vector3(x, y, z), Time.deltaTime * smoothness);
        // Zapisz przesuniêcie
        faceOffset = smoothedTargetPosition;
    }

    void OnDisable()
    {
        if (udpClient != null)
            udpClient.Close();
    }
}
