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
    public int score = 0; // Puanlama i�in de�i�ken
    private bool isGameOver = false;
    public string hiz2;
    public AudioSource motorAudio ;
    public GameObject ESC;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private bool isPaused = false; // Oyunun duraklat�l�p duraklat�lmad���n� kontrol etmek i�in

    [SerializeField] private float motorForce = 2000f; // Motor torkunu azalt
    [SerializeField] private float breakForce = 3000f;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private TextMeshProUGUI scoreText; // Sol �stte skor yaz�s�

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private AudioSource hornAudio; // Korona sesi kayna��
    [SerializeField] private TextMeshProUGUI speedText; // Sa� alt h�z yaz�s�
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private AudioSource coinAudio; // Korona sesi kayna��
    [SerializeField] private TextMeshProUGUI timerText; // Sol �stte s�re yaz�s�
    private float elapsedTime = 0f; // Ge�en s�re
    private bool isTimerActive = true; // Zamanlay�c�n�n aktif olup olmad���n� kontrol eder



    private Rigidbody rb;

    private void Start()
    {


        rb = GetComponent<Rigidbody>();
        motorAudio = GetComponent<AudioSource>();

        rb.centerOfMass = new Vector3(0, -1.0f, 0); // A��rl�k merkezini ayarla

        // S�spansiyon ayarlar�n� g�ncelle
        JointSpring suspension = frontLeftWheelCollider.suspensionSpring;
        suspension.spring = 50000f; // S�spansiyon sertli�i
        suspension.damper = 5000f; // S�spansiyon s�n�mleme
        suspension.targetPosition = 0.5f; // S�spansiyon orta noktas�

        frontLeftWheelCollider.suspensionSpring = suspension;
        frontRightWheelCollider.suspensionSpring = suspension;
        rearLeftWheelCollider.suspensionSpring = suspension;
        rearRightWheelCollider.suspensionSpring = suspension;

        WheelFrictionCurve friction = frontLeftWheelCollider.sidewaysFriction;
        friction.stiffness = 2.0f; // Varsay�lan genellikle 1.0'd�r, daha iyi tutu� i�in art�r�ld�
        frontLeftWheelCollider.sidewaysFriction = friction;
        frontRightWheelCollider.sidewaysFriction = friction;
        rearLeftWheelCollider.sidewaysFriction = friction;
        rearRightWheelCollider.sidewaysFriction = friction;
    }
    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC tu�una bas�ld���nda
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
        HandlePause(); // ESC tu�u kontrol�

        if (isTimerActive)
        {
            elapsedTime += Time.deltaTime; // Ge�en s�reyi art�r
            UpdateTimerUI(); // UI'� g�ncelle
        }

        if (carRigidbody != null)
        {
            float speed = carRigidbody.velocity.magnitude * 3.6f; // H�z (km/h)
            speedText.text = speed.ToString("F0") + " km/h";
        }

        HandleHorn(); // Korona kontrol�
        
            if (motorAudio != null)
            {
                // Rigidbody h�z�n� km/h cinsinden hesapla
                float speed = rb.velocity.magnitude * 3.6f;

                // H�zlanmaya ba�l� olarak motor sesi frekans� de�i�ir
                if (speed < 30f)
                {
                    motorAudio.pitch = 1f; // 30 km/h alt�
                }

                else
                {
                    if (motorAudio != null)
                    {
                        // Motor sesi frekans�n� h�zla orant�l� de�i�tir (smooth)
                        motorAudio.pitch = Mathf.Lerp(1f, 2f, speed / 100f); // Maksimum h�z 100 km/h varsay�l�r
                    }
                }
            }
        
       
    }


    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // mm:ss format�nda g�ster
    }
    private void HandleHorn()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E tu�una bas�ld���nda
        {
            if (hornAudio != null)
            {
                hornAudio.Play(); // Korona sesini �al
                Debug.Log("Korona �al�nd�!");
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
            CheckCarPosition(); // Denge kontrol�
            StabilizeCar(); // Denge sa�lama
        }
        else
        {
            StopCar();
        }
    }

    private void StabilizeCar()
    {
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

        if (tiltAngle > 30f) // E�er e�im a��s� �ok fazlaysa
        {
            rb.angularVelocity = Vector3.zero; // D�nd�rme hareketini s�f�rla
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Arac� dik konuma getir
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

        // H�z s�n�r�n� uygula
        float speed = rb.velocity.magnitude * 3.6f; // H�z� km/h'ye �evir
        if (speed > 130f) // E�er h�z 140 km/h'yi ge�erse
        {
            rb.velocity = rb.velocity.normalized * (130f / 3.6f); // Maksimum h�z� s�n�rla
        }
    }



    [SerializeField] private AudioSource brakeAudio; // Fren sesi kayna��

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
        isPaused = true; // Duraklat�ld���n� i�aretle
        Time.timeScale = 0f; // Oyun zaman�n� durdur
        ESC.SetActive(true); // Pause ekran�n� aktif et
    }

    public void ResumeGame()
    {
        isPaused = false; // Oyunun devam etti�ini i�aretle
        Time.timeScale = 1f; // Oyun zaman�n� ba�lat
        ESC.SetActive(false); // Pause ekran�n� kapat
    }

    private void HandleSteering()
    {
        // D�n�� a��s�n� h�zla orant�l� olarak azalt
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
    [SerializeField] private AudioSource crashAudio; // �arp��ma sesi kayna��

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "engel") // �arp��ma engelle
        {
            if (crashAudio != null && !crashAudio.isPlaying)
            {
                crashAudio.Play(); // �arp��ma sesi �al
            }

            // �arp��ma sonras� arabay� biraz geri �ek ve h�z�n� s�f�rla
            //MoveCarBackAndStop();
        }

        if (collision.collider.tag == "coin") // Coin toplama
        {
            score++;
            scoreText.text = score.ToString(); // UI g�ncelleme
            Destroy(collision.gameObject);
            coinAudio.Play(); // �arp��ma sesi �al

        }
        if (collision.collider.tag == "bitis")
        {
            isTimerActive = false; // Zamanlay�c�y� durdur
            StopCar(); // Arabay� durdur
            Destroy(collision.gameObject);

            Invoke("ShowResultPanel", 3f); // 3 saniye sonra sonucu g�ster
        }
    }
    [SerializeField] private TextMeshProUGUI resultText; // Sonu� ekran� i�in yaz�
    [SerializeField] private TextMeshProUGUI s�reText; // Sonu� ekran� i�in yaz�
    [SerializeField] private TextMeshProUGUI kumsaatiText; // Sonu� ekran� i�in yaz�

    [SerializeField] private GameObject resultPanel; // Biti� paneli


    private void ShowResultPanel()
    {
        Time.timeScale = 0f; // Oyunu durdur

        int totalScore = Mathf.Max(0, Mathf.FloorToInt(elapsedTime) - score); // Skordan s�reyi ��kar ve s�f�r�n alt�na d��mesini engelle
        resultPanel.gameObject.SetActive(true); // Sonu� yaz�s�n� g�ster
        resultText.text = $" {totalScore} saniye"; // Sonu� ekran�
        kumsaatiText.text = score.ToString(); // Sonu� ekran�
        s�reText.text = Mathf.FloorToInt(elapsedTime).ToString();

        StopCar(); // Arabay� durdur

    }

    private void MoveCarBackAndStop()
    {
        // Rigidbody bile�enine eri�
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // H�z� ve a��sal h�z� s�f�rla
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Araban�n pozisyonunu ve rotas�n� ayarla
        Vector3 newPosition = transform.position - transform.forward * -16f; // Arabay� 2 birim geri �ek
        transform.position = newPosition;

        // Araban�n dik durmas�n� sa�la
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        Debug.Log("Araba geri �ekildi ve durduruldu.");
    }



    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isGameOver = false;
        score = 0; // Puan s�f�rlan�r
        elapsedTime = 0f; // S�re s�f�rlan�r
        isTimerActive = true; // Zamanlay�c� yeniden ba�lat�l�r
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
        // Rigidbody h�z�n� al ve konsola yazd�r
        float speed = rb.velocity.magnitude * 3.6f; // H�z� km/h cinsine �evir
        hiz2 = speed.ToString("F0") + " km/h";
    }

    private void CheckCarPosition()
    {
        // Arac�n dikey a��s�n� kontrol et (devrilme kontrol�)
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

        // E�er arac�n e�im a��s� 45 dereceden b�y�kse devrilmi� kabul edilir
        if (tiltAngle > 45f)
        {
            Debug.Log("Araba devrildi! Yeniden ba�lat�l�yor...");
            Restart();
        }

        // Araba yerin alt�na d��erse yeniden ba�lat
        if (transform.position.y < -5f)
        {
            Debug.Log("Araba �ok d���k bir pozisyonda! Yeniden ba�lat�l�yor...");
            Restart();
        }
    }

    private void CorrectCarPosition()
    {
        // Araba devrildi�inde d�zelt
        Debug.Log("Araba d�zeltildi.");
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        rb.velocity = Vector3.zero; // Hareketi s�f�rla
        rb.angularVelocity = Vector3.zero; // D�nme hareketini s�f�rla
    }
}
