using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in gerekli

public class oyna : MonoBehaviour
{
    public GameObject AnaEkran; // Ana ekran paneli
    public GameObject ayarlar; // Ayarlar paneli
    public GameObject nasiloyanir; // Nas�l oynan�r paneli
    public GameObject araba; // Araba nesnesi
    public Transform startPosition; // Araban�n ba�lang�� pozisyonu
    public GameObject carchoose;
    public GameObject car2;
    public GameObject car1;
    public bool isGameStarted = false; // Oyunun ba�lam�� oldu�unu kontrol etmek i�in de�i�ken
    private bool isMuted = false; // Sesin kapal� olup olmad���n� kontrol etmek i�in de�i�ken
    public AudioSource backgroundMusic; // Ana ekran m�zi�i
    public MeshRenderer meshRenderer;


    public arabahareket ah;
    // Ba�lang�� ayarlar�
    void Start()
    {
        Time.timeScale = 0f; // Oyun ba�lang��ta durdurulur
        isGameStarted = false; // Oyun ba�lamad�
        AnaEkran.SetActive(true); // Ana ekran aktif
        ayarlar.SetActive(false); // Ayarlar ekran� kapal�
        nasiloyanir.SetActive(false); // Nas�l oynan�r ekran� kapal�
                                      // M�zik otomatik �als�n
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne y�klendi�inde �a�r�lan olay
    }

    // Oyun ba�latma
    public void oyna_button()
    {
        AnaEkran.SetActive(false); // Ana ekran� gizle
        isGameStarted = true; // Oyunun ba�lad���n� i�aretle
        Time.timeScale = 1f; // Oyun zaman�n� ba�lat
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
        ah.motorAudio.Play();


    }
    public void QuitGame()
    {
        Debug.Log("Oyun kapat�l�yor..."); // Konsolda bir mesaj g�ster (sadece edit�rde g�r�n�r)
        Application.Quit(); // Oyunu kapat
    }


    // Oyunu s�f�rlama ve hemen ba�latma
    public void again_button()
    {
        isGameStarted = true; // Oyunun ba�lad���n� i�aretle
        Time.timeScale = 1f; // Oyun zaman�n� ba�lat
        AnaEkran.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    // Oyunu s�f�rlama ve ana ekrana d�nme
    public void Main_button()
    {
        isGameStarted = false; // Oyunun durdu�unu i�aretle
        Time.timeScale = 0f; // Oyun zaman�n� durdur
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Sahneyi yeniden y�kle
    }

    // Ayarlar ekran�n� a�ma
    public void ayarlar_button()
    {
        ayarlar.SetActive(true);
    }

    // Nas�l oynan�r ekran�n� a�ma
    public void nasil_button()
    {
        nasiloyanir.SetActive(true);
    }

    public void arabasecim()
    {
        carchoose.SetActive(true);
    }

    // Geri butonu (ayarlar veya nas�l oynan�r ekran�ndan d�n��)
    public void back_button()
    {
        nasiloyanir.SetActive(false);
        ayarlar.SetActive(false);
    }

    public void arabasec1_button()
    {
        carchoose.SetActive(false);
        // E�er MeshRenderer mevcutsa devre d��� b�rak
      
         meshRenderer.enabled = false; // MeshRenderer'� devre d��� b�rak
        car1.gameObject.SetActive(true);


    }
    public void arabasec2_button()
    {
        car1.gameObject.SetActive(false);
        carchoose.SetActive(false);
        meshRenderer.enabled = true; // MeshRenderer'� devre d��� b�rak


    }
    // Sesi kapatma/a�ma
    public void mute_button()
    {
        isMuted = !isMuted; // Ses durumunu ters �evir
        AudioListener.volume = isMuted ? 0f : 1f; // Ses seviyesini ayarla
    }

    // Sahne y�klendi�inde �a�r�l�r
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isGameStarted)
        {
            // Araban�n pozisyonunu ve rotasyonunu s�f�rla
            araba.transform.position = startPosition.position;
            araba.transform.rotation = startPosition.rotation;

            // Araban�n hareketini s�f�rla
            Rigidbody rb = araba.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // E�er ana ekrana d�n�ld�yse, AnaEkran'� aktif yap
            AnaEkran.SetActive(true);
        }
    }

    // Olay kald�rma (Bellek s�z�nt�s�n� �nlemek i�in)
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
