using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class arabahareket : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    public int score = 0; // Puanlama için deðiþken
    private bool isGameOver = false;
    public string hiz2;
    public AudioSource motorAudio ;
    public GameObject ESC;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private bool isPaused = false; // Oyunun duraklatýlýp duraklatýlmadýðýný kontrol etmek için

    [SerializeField] private float motorForce = 2000f; // Motor torkunu azalt
    [SerializeField] private float breakForce = 3000f;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private TextMeshProUGUI scoreText; // Sol üstte skor yazýsý

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private AudioSource hornAudio; // Korona sesi kaynaðý
    [SerializeField] private TextMeshProUGUI speedText; // Sað alt hýz yazýsý
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private AudioSource coinAudio; // Korona sesi kaynaðý
    [SerializeField] private TextMeshProUGUI timerText; // Sol üstte süre yazýsý
    private float elapsedTime = 0f; // Geçen süre
    private bool isTimerActive = true; // Zamanlayýcýnýn aktif olup olmadýðýný kontrol eder



    private Rigidbody rb;

    private void Start()
    {


        rb = GetComponent<Rigidbody>();
        motorAudio = GetComponent<AudioSource>();

        rb.centerOfMass = new Vector3(0, -1.0f, 0); // Aðýrlýk merkezini ayarla

        // Süspansiyon ayarlarýný güncelle
        JointSpring suspension = frontLeftWheelCollider.suspensionSpring;
        suspension.spring = 50000f; // Süspansiyon sertliði
        suspension.damper = 5000f; // Süspansiyon sönümleme
        suspension.targetPosition = 0.5f; // Süspansiyon orta noktasý

        frontLeftWheelCollider.suspensionSpring = suspension;
        frontRightWheelCollider.suspensionSpring = suspension;
        rearLeftWheelCollider.suspensionSpring = suspension;
        rearRightWheelCollider.suspensionSpring = suspension;

        WheelFrictionCurve friction = frontLeftWheelCollider.sidewaysFriction;
        friction.stiffness = 2.0f; // Varsayýlan genellikle 1.0'dýr, daha iyi tutuþ için artýrýldý
        frontLeftWheelCollider.sidewaysFriction = friction;
        frontRightWheelCollider.sidewaysFriction = friction;
        rearLeftWheelCollider.sidewaysFriction = friction;
        rearRightWheelCollider.sidewaysFriction = friction;
    }
    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC tuþuna basýldýðýnda
        {
            if (isPaused)
            {
                ResumeGame(); // Oyuna devam et
            }
            else
            {
                PauseGame(); // Oyunu duraklat
            }
        }
    }

    private void Update()
    {
        HandlePause(); // ESC tuþu kontrolü

        if (isTimerActive)
        {
            elapsedTime += Time.deltaTime; // Geçen süreyi artýr
            UpdateTimerUI(); // UI'ý güncelle
        }

        if (carRigidbody != null)
        {
            float speed = carRigidbody.velocity.magnitude * 3.6f; // Hýz (km/h)
            speedText.text = speed.ToString("F0") + " km/h";
        }

        HandleHorn(); // Korona kontrolü
        
            if (motorAudio != null)
            {
                // Rigidbody hýzýný km/h cinsinden hesapla
                float speed = rb.velocity.magnitude * 3.6f;

                // Hýzlanmaya baðlý olarak motor sesi frekansý deðiþir
                if (speed < 30f)
                {
                    motorAudio.pitch = 1f; // 30 km/h altý
                }

                else
                {
                    if (motorAudio != null)
                    {
                        // Motor sesi frekansýný hýzla orantýlý deðiþtir (smooth)
                        motorAudio.pitch = Mathf.Lerp(1f, 2f, speed / 100f); // Maksimum hýz 100 km/h varsayýlýr
                    }
                }
            }
        
       
    }


    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // mm:ss formatýnda göster
    }
    private void HandleHorn()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E tuþuna basýldýðýnda
        {
            if (hornAudio != null)
            {
                hornAudio.Play(); // Korona sesini çal
                Debug.Log("Korona Çalýndý!");
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            UpdateWheels();
            DisplaySpeed();
            CheckCarPosition(); // Denge kontrolü
            StabilizeCar(); // Denge saðlama
        }
        else
        {
            StopCar();
        }
    }

    private void StabilizeCar()
    {
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

        if (tiltAngle > 30f) // Eðer eðim açýsý çok fazlaysa
        {
            rb.angularVelocity = Vector3.zero; // Döndürme hareketini sýfýrla
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Aracý dik konuma getir
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);

    }

    private void HandleMotor()
    {
        // Motor torku
        frontLeftWheelCollider.motorTorque = -verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = -verticalInput * motorForce;
        rearLeftWheelCollider.motorTorque = -verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = -verticalInput * motorForce;

        // Frenleme
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

        // Hýz sýnýrýný uygula
        float speed = rb.velocity.magnitude * 3.6f; // Hýzý km/h'ye çevir
        if (speed > 130f) // Eðer hýz 140 km/h'yi geçerse
        {
            rb.velocity = rb.velocity.normalized * (130f / 3.6f); // Maksimum hýzý sýnýrla
        }
    }



    [SerializeField] private AudioSource brakeAudio; // Fren sesi kaynaðý

    private void ApplyBreaking()
    {
        if (isBreaking)
        {
            // Arka tekerleklere daha az fren uygula
            frontRightWheelCollider.brakeTorque = breakForce;
            frontLeftWheelCollider.brakeTorque = breakForce;
            rearLeftWheelCollider.brakeTorque = breakForce * 2.0f;
            rearRightWheelCollider.brakeTorque = breakForce * 2.0f;
            if (!brakeAudio.isPlaying)
            {
                brakeAudio.Play();
            }
        }
        else
        {
            frontRightWheelCollider.brakeTorque = 0f;
            frontLeftWheelCollider.brakeTorque = 0f;
            rearLeftWheelCollider.brakeTorque = 0f;
            rearRightWheelCollider.brakeTorque = 0f;

             if (brakeAudio.isPlaying)
            {
                brakeAudio.Stop();
            }
        }

    }
    public void PauseGame()
    {
        isPaused = true; // Duraklatýldýðýný iþaretle
        Time.timeScale = 0f; // Oyun zamanýný durdur
        ESC.SetActive(true); // Pause ekranýný aktif et
    }

    public void ResumeGame()
    {
        isPaused = false; // Oyunun devam ettiðini iþaretle
        Time.timeScale = 1f; // Oyun zamanýný baþlat
        ESC.SetActive(false); // Pause ekranýný kapat
    }

    private void HandleSteering()
    {
        // Dönüþ açýsýný hýzla orantýlý olarak azalt
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }


    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
    [SerializeField] private AudioSource crashAudio; // Çarpýþma sesi kaynaðý

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "engel") // Çarpýþma engelle
        {
            if (crashAudio != null && !crashAudio.isPlaying)
            {
                crashAudio.Play(); // Çarpýþma sesi çal
            }

            // Çarpýþma sonrasý arabayý biraz geri çek ve hýzýný sýfýrla
            //MoveCarBackAndStop();
        }

        if (collision.collider.tag == "coin") // Coin toplama
        {
            score++;
            scoreText.text = score.ToString(); // UI güncelleme
            Destroy(collision.gameObject);
            coinAudio.Play(); // Çarpýþma sesi çal

        }
        if (collision.collider.tag == "bitis")
        {
            isTimerActive = false; // Zamanlayýcýyý durdur
            StopCar(); // Arabayý durdur
            Destroy(collision.gameObject);

            Invoke("ShowResultPanel", 3f); // 3 saniye sonra sonucu göster
        }
    }
    [SerializeField] private TextMeshProUGUI resultText; // Sonuç ekraný için yazý
    [SerializeField] private TextMeshProUGUI süreText; // Sonuç ekraný için yazý
    [SerializeField] private TextMeshProUGUI kumsaatiText; // Sonuç ekraný için yazý

    [SerializeField] private GameObject resultPanel; // Bitiþ paneli


    private void ShowResultPanel()
    {
        Time.timeScale = 0f; // Oyunu durdur

        int totalScore = Mathf.Max(0, Mathf.FloorToInt(elapsedTime) - score); // Skordan süreyi çýkar ve sýfýrýn altýna düþmesini engelle
        resultPanel.gameObject.SetActive(true); // Sonuç yazýsýný göster
        resultText.text = $" {totalScore} saniye"; // Sonuç ekraný
        kumsaatiText.text = score.ToString(); // Sonuç ekraný
        süreText.text = Mathf.FloorToInt(elapsedTime).ToString();

        StopCar(); // Arabayý durdur

    }

    private void MoveCarBackAndStop()
    {
        // Rigidbody bileþenine eriþ
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Hýzý ve açýsal hýzý sýfýrla
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Arabanýn pozisyonunu ve rotasýný ayarla
        Vector3 newPosition = transform.position - transform.forward * -16f; // Arabayý 2 birim geri çek
        transform.position = newPosition;

        // Arabanýn dik durmasýný saðla
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        Debug.Log("Araba geri çekildi ve durduruldu.");
    }



    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isGameOver = false;
        score = 0; // Puan sýfýrlanýr
        elapsedTime = 0f; // Süre sýfýrlanýr
        isTimerActive = true; // Zamanlayýcý yeniden baþlatýlýr
    }

    private void StopCar()
    {
        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        frontLeftWheelCollider.brakeTorque = 1000f;
        frontRightWheelCollider.brakeTorque = 1000f;
    }

    private void DisplaySpeed()
    {
        // Rigidbody hýzýný al ve konsola yazdýr
        float speed = rb.velocity.magnitude * 3.6f; // Hýzý km/h cinsine çevir
        hiz2 = speed.ToString("F0") + " km/h";
    }

    private void CheckCarPosition()
    {
        // Aracýn dikey açýsýný kontrol et (devrilme kontrolü)
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

        // Eðer aracýn eðim açýsý 45 dereceden büyükse devrilmiþ kabul edilir
        if (tiltAngle > 45f)
        {
            Debug.Log("Araba devrildi! Yeniden baþlatýlýyor...");
            Restart();
        }

        // Araba yerin altýna düþerse yeniden baþlat
        if (transform.position.y < -5f)
        {
            Debug.Log("Araba çok düþük bir pozisyonda! Yeniden baþlatýlýyor...");
            Restart();
        }
    }

    private void CorrectCarPosition()
    {
        // Araba devrildiðinde düzelt
        Debug.Log("Araba düzeltildi.");
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        rb.velocity = Vector3.zero; // Hareketi sýfýrla
        rb.angularVelocity = Vector3.zero; // Dönme hareketini sýfýrla
    }
}
