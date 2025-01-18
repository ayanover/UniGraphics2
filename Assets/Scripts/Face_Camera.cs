using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;
using Debug = UnityEngine.Debug;

public class FaceCameraController : MonoBehaviour
{
    public float smoothness = 5f;
    public float maxHorizontalMovement = 2f;
    public float maxVerticalMovement = 1f;
    public float maxDepthMovement = 2f;
    public float maxTiltAngle = 45f;

    private UdpClient udpClient;
    private const int port = 5065;
    private Vector3 targetPosition;
    private Vector3 smoothedTargetPosition;
    private float targetTilt;
    private float smoothedTilt;
    public Vector3 faceOffset { get; private set; }
    public float faceTilt { get; private set; }

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
                if (parts.Length == 4 &&
                    float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedX) &&
                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedY) &&
                    float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedZ) &&
                    float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float receivedTilt))
                {
                    targetPosition = new Vector3(receivedX, receivedY, receivedZ);
                    targetTilt = receivedTilt;
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket error: " + e.ToString());
        }

        float x = Mathf.Lerp(-maxHorizontalMovement, maxHorizontalMovement, (targetPosition.x + 1) / 2);
        float y = Mathf.Lerp(-maxVerticalMovement, maxVerticalMovement, (targetPosition.y + 1) / 2);
        float z = Mathf.Lerp(-maxDepthMovement, 0, targetPosition.z);

        smoothedTargetPosition = Vector3.Lerp(smoothedTargetPosition, new Vector3(x, y, z), Time.deltaTime * smoothness);
        smoothedTilt = Mathf.Lerp(smoothedTilt, targetTilt * maxTiltAngle, Time.deltaTime * smoothness);

        faceOffset = smoothedTargetPosition;
        faceTilt = smoothedTilt;
    }

    void OnDisable()
    {
        if (udpClient != null)
            udpClient.Close();
    }
}