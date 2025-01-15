using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Transform car; // Arabanýn transformu
    [SerializeField] private Vector3 frontViewOffset = new Vector3(0, 2, 4); // Ön görünüm offseti
    [SerializeField] private Vector3 rearViewOffset = new Vector3(0, 2, -4); // Arka görünüm offseti
    [SerializeField] private float switchSpeed = 5f; // Kamera geçiþ hýzý
    [SerializeField] private Rigidbody carRigidbody; // Arabanýn Rigidbody bileþeni
    [SerializeField] private float holdDuration = 3f; // "S" tuþuna basýlý tutulma süresi

    private Vector3 currentOffset; // Þu anki offset
    private bool isRearView = false; // Arka görünüm aktif mi?
    private float holdTimer = 0f; // "S" tuþuna basýlý tutulma süresi sayaç

    void Start()
    {
        // Varsayýlan olarak ön görünümde baþla
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

        // "S" tuþuna basýlý tutulduðunu kontrol et
        if (Input.GetKey(KeyCode.S))
        {
            holdTimer += Time.deltaTime; // Sayaç artar

            if (holdTimer >= holdDuration) // 2.5 saniye geçtiyse
            {
                isRearView = true;
                currentOffset = rearViewOffset;
            }
        }
        else
        {
            holdTimer = 0f; // "S" tuþu býrakýlýrsa sayaç sýfýrlanýr
            isRearView = false;
            currentOffset = frontViewOffset;
        }
    }

    private void UpdateCameraPosition()
    {
        // Kamera pozisyonunu yumuþak geçiþle hedefe taþý
        Vector3 targetPosition = car.position + car.TransformDirection(currentOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, switchSpeed * Time.deltaTime);

        // Kameranýn her zaman arabaya doðru bakmasýný saðla
        transform.LookAt(car);
    }
}
