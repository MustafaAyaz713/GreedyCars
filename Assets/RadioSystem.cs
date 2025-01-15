using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [SerializeField] private AudioSource radioAudioSource; // Radyo için AudioSource
    [SerializeField] private List<AudioClip> musicTracks; // Müzik listesi
    private int currentTrackIndex = 0; // Þu anki þarký indeksi
    private bool isRadioOn = false; // Radyo açýk mý?
    [SerializeField] private TextMeshProUGUI radioText; // Sol alttaki radyo bilgisi
    [SerializeField] private GameObject radioTextContainer; // Text ve image'in bulunduðu GameObject

    private void Start()
    {
        radioTextContainer.SetActive(false); // Baþlangýçta text ve image'i gizle
    }

    private void Update()
    {
        HandleRadioToggle(); // Radyoyu aç/kapa
        if (isRadioOn)
        {
            HandleMouseScroll(); // Þarkýlar arasýnda geçiþi kontrol et
        }
    }

    private void HandleRadioToggle()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRadioOn = !isRadioOn; // Radyo durumunu deðiþtir

            if (isRadioOn)
            {
                PlayTrack(currentTrackIndex); // Ýlk þarkýyý çalmaya baþla
                Debug.Log("Radyo Açýldý.");
            }
            else
            {
                radioAudioSource.Pause(); // Radyoyu durdur
                Debug.Log("Radyo Kapandý.");
            }
        }
    }

    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // Yukarý kaydýrma
        {
            NextTrack();
        }
        else if (scroll < 0f) // Aþaðý kaydýrma
        {
            PreviousTrack();
        }
    }

    private void NextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= musicTracks.Count)
        {
            currentTrackIndex = 0; // Liste baþýna dön
        }
        PlayTrack(currentTrackIndex);
    }

    private void PreviousTrack()
    {
        currentTrackIndex--;
        if (currentTrackIndex < 0)
        {
            currentTrackIndex = musicTracks.Count - 1; // Liste sonuna dön
        }
        PlayTrack(currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        if (radioAudioSource != null && musicTracks.Count > 0)
        {
            radioAudioSource.clip = musicTracks[index];
            radioAudioSource.Play(); // Þarkýyý çal
            Debug.Log("Çalan Þarký: " + musicTracks[index].name);
            radioText.text = musicTracks[index].name; // UI güncelleme
            radioTextContainer.SetActive(true); // Text ve image'i göster

            // Start coroutine to hide text and image after 10 seconds
            StartCoroutine(HideRadioTextAfterDelay(10f));
        }
    }

    private IEnumerator HideRadioTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        radioTextContainer.SetActive(false); // Text ve image'i gizle
    }
}