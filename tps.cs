using UnityEngine;

public class tps : MonoBehaviour
{
    public Transform target;            // Target untuk diikuti (karakter)
    public Vector3 offset = new Vector3(0, 2, -5); // Jarak kamera ke target
    public float followSpeed = 10f;     // Seberapa cepat kamera mengikuti
    public float rotationSpeed = 5f;    // Sensitivitas rotasi mouse

    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 15f;

    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float currentZoom;
    private float yaw = 0f;     // rotasi horizontal
    private float pitch = 20f;  // rotasi vertikal

    void Start()
    {
        currentZoom = offset.magnitude;
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Rotasi mouse kanan
        if (Input.GetMouseButton(1)) // Klik kanan
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    void UpdateCameraPosition()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -currentZoom);

        Vector3 desiredPosition = target.position + direction + Vector3.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}
