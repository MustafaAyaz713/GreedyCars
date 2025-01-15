using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Transform needleTransform; // Ýðnenin Transform'u
    [SerializeField] private Rigidbody carRigidbody;   // Arabanýn Rigidbody'si
    [SerializeField] private float maxSpeed = 200f;    // Maksimum hýz (km/h)
    [SerializeField] private float maxNeedleRotation = 40f; // Ýðnenin maksimum dönüþ açýsý
    [SerializeField] private float minNeedleRotation = -140;    // Ýðnenin minimum dönüþ açýsý

    private float currentNeedleRotation = 0f;

    private void Update()
    {
        float speed = carRigidbody.velocity.magnitude * 3.2f; // Hýz (km/h)
        float targetNeedleRotation = Mathf.Clamp(speed / maxSpeed, 0f, 1f) * (maxNeedleRotation - minNeedleRotation) + minNeedleRotation;

        // Ýðnenin hareketini yumuþat
        currentNeedleRotation = Mathf.Lerp(currentNeedleRotation, targetNeedleRotation, Time.deltaTime * 5f);
        needleTransform.localEulerAngles = new Vector3(0, 0, currentNeedleRotation);
    }
}
