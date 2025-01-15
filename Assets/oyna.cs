using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class oyna : MonoBehaviour
{
    public GameObject AnaEkran; // Ana ekran paneli
    public GameObject ayarlar; // Ayarlar paneli
    public GameObject nasiloyanir; // Nasýl oynanýr paneli
    public GameObject araba; // Araba nesnesi
    public Transform startPosition; // Arabanýn baþlangýç pozisyonu
    public GameObject carchoose;
    public GameObject car2;
    public GameObject car1;
    public bool isGameStarted = false; // Oyunun baþlamýþ olduðunu kontrol etmek için deðiþken
    private bool isMuted = false; // Sesin kapalý olup olmadýðýný kontrol etmek için deðiþken
    public AudioSource backgroundMusic; // Ana ekran müziði
    public MeshRenderer meshRenderer;


    public arabahareket ah;
    // Baþlangýç ayarlarý
    void Start()
    {
        Time.timeScale = 0f; // Oyun baþlangýçta durdurulur
        isGameStarted = false; // Oyun baþlamadý
        AnaEkran.SetActive(true); // Ana ekran aktif
        ayarlar.SetActive(false); // Ayarlar ekraný kapalý
        nasiloyanir.SetActive(false); // Nasýl oynanýr ekraný kapalý
                                      // Müzik otomatik çalsýn
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne yüklendiðinde çaðrýlan olay
    }

    // Oyun baþlatma
    public void oyna_button()
    {
        AnaEkran.SetActive(false); // Ana ekraný gizle
        isGameStarted = true; // Oyunun baþladýðýný iþaretle
        Time.timeScale = 1f; // Oyun zamanýný baþlat
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
        ah.motorAudio.Play();


    }
    public void QuitGame()
    {
        Debug.Log("Oyun kapatýlýyor..."); // Konsolda bir mesaj göster (sadece editörde görünür)
        Application.Quit(); // Oyunu kapat
    }


    // Oyunu sýfýrlama ve hemen baþlatma
    public void again_button()
    {
        isGameStarted = true; // Oyunun baþladýðýný iþaretle
        Time.timeScale = 1f; // Oyun zamanýný baþlat
        AnaEkran.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    // Oyunu sýfýrlama ve ana ekrana dönme
    public void Main_button()
    {
        isGameStarted = false; // Oyunun durduðunu iþaretle
        Time.timeScale = 0f; // Oyun zamanýný durdur
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Sahneyi yeniden yükle
    }

    // Ayarlar ekranýný açma
    public void ayarlar_button()
    {
        ayarlar.SetActive(true);
    }

    // Nasýl oynanýr ekranýný açma
    public void nasil_button()
    {
        nasiloyanir.SetActive(true);
    }

    public void arabasecim()
    {
        carchoose.SetActive(true);
    }

    // Geri butonu (ayarlar veya nasýl oynanýr ekranýndan dönüþ)
    public void back_button()
    {
        nasiloyanir.SetActive(false);
        ayarlar.SetActive(false);
    }

    public void arabasec1_button()
    {
        carchoose.SetActive(false);
        // Eðer MeshRenderer mevcutsa devre dýþý býrak
      
         meshRenderer.enabled = false; // MeshRenderer'ý devre dýþý býrak
        car1.gameObject.SetActive(true);


    }
    public void arabasec2_button()
    {
        car1.gameObject.SetActive(false);
        carchoose.SetActive(false);
        meshRenderer.enabled = true; // MeshRenderer'ý devre dýþý býrak


    }
    // Sesi kapatma/açma
    public void mute_button()
    {
        isMuted = !isMuted; // Ses durumunu ters çevir
        AudioListener.volume = isMuted ? 0f : 1f; // Ses seviyesini ayarla
    }

    // Sahne yüklendiðinde çaðrýlýr
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isGameStarted)
        {
            // Arabanýn pozisyonunu ve rotasyonunu sýfýrla
            araba.transform.position = startPosition.position;
            araba.transform.rotation = startPosition.rotation;

            // Arabanýn hareketini sýfýrla
            Rigidbody rb = araba.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // Eðer ana ekrana dönüldüyse, AnaEkran'ý aktif yap
            AnaEkran.SetActive(true);
        }
    }

    // Olay kaldýrma (Bellek sýzýntýsýný önlemek için)
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
