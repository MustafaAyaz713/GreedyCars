using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Transform needleTransform; // ��nenin Transform'u
    [SerializeField] private Rigidbody carRigidbody;   // Araban�n Rigidbody'si
    [SerializeField] private float maxSpeed = 200f;    // Maksimum h�z (km/h)
    [SerializeField] private float maxNeedleRotation = 40f; // ��nenin maksimum d�n�� a��s�
    [SerializeField] private float minNeedleRotation = -140;    // ��nenin minimum d�n�� a��s�

    private float currentNeedleRotation = 0f;

    private void Update()
    {
        float speed = carRigidbody.velocity.magnitude * 3.2f; // H�z (km/h)
        float targetNeedleRotation = Mathf.Clamp(speed / maxSpeed, 0f, 1f) * (maxNeedleRotation - minNeedleRotation) + minNeedleRotation;

        // ��nenin hareketini yumu�at
        currentNeedleRotation = Mathf.Lerp(currentNeedleRotation, targetNeedleRotation, Time.deltaTime * 5f);
        needleTransform.localEulerAngles = new Vector3(0, 0, currentNeedleRotation);
    }
}
