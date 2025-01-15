using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Transform car; // Araban�n transformu
    [SerializeField] private Vector3 frontViewOffset = new Vector3(0, 2, 4); // �n g�r�n�m offseti
    [SerializeField] private Vector3 rearViewOffset = new Vector3(0, 2, -4); // Arka g�r�n�m offseti
    [SerializeField] private float switchSpeed = 5f; // Kamera ge�i� h�z�
    [SerializeField] private Rigidbody carRigidbody; // Araban�n Rigidbody bile�eni
    [SerializeField] private float holdDuration = 3f; // "S" tu�una bas�l� tutulma s�resi

    private Vector3 currentOffset; // �u anki offset
    private bool isRearView = false; // Arka g�r�n�m aktif mi?
    private float holdTimer = 0f; // "S" tu�una bas�l� tutulma s�resi saya�

    void Start()
    {
        // Varsay�lan olarak �n g�r�n�mde ba�la
        currentOffset = frontViewOffset;
    }

    void Update()
    {
        HandleCameraSwitch();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void HandleCameraSwitch()
    {
        float carSpeed = carRigidbody.velocity.magnitude * 3.6f;

        // "S" tu�una bas�l� tutuldu�unu kontrol et
        if (Input.GetKey(KeyCode.S))
        {
            holdTimer += Time.deltaTime; // Saya� artar

            if (holdTimer >= holdDuration) // 2.5 saniye ge�tiyse
            {
                isRearView = true;
                currentOffset = rearViewOffset;
            }
        }
        else
        {
            holdTimer = 0f; // "S" tu�u b�rak�l�rsa saya� s�f�rlan�r
            isRearView = false;
            currentOffset = frontViewOffset;
        }
    }

    private void UpdateCameraPosition()
    {
        // Kamera pozisyonunu yumu�ak ge�i�le hedefe ta��
        Vector3 targetPosition = car.position + car.TransformDirection(currentOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, switchSpeed * Time.deltaTime);

        // Kameran�n her zaman arabaya do�ru bakmas�n� sa�la
        transform.LookAt(car);
    }
}
