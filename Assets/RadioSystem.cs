using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [SerializeField] private AudioSource radioAudioSource; // Radyo i�in AudioSource
    [SerializeField] private List<AudioClip> musicTracks; // M�zik listesi
    private int currentTrackIndex = 0; // �u anki �ark� indeksi
    private bool isRadioOn = false; // Radyo a��k m�?
    [SerializeField] private TextMeshProUGUI radioText; // Sol alttaki radyo bilgisi
    [SerializeField] private GameObject radioTextContainer; // Text ve image'in bulundu�u GameObject

    private void Start()
    {
        radioTextContainer.SetActive(false); // Ba�lang��ta text ve image'i gizle
    }

    private void Update()
    {
        HandleRadioToggle(); // Radyoyu a�/kapa
        if (isRadioOn)
        {
            HandleMouseScroll(); // �ark�lar aras�nda ge�i�i kontrol et
        }
    }

    private void HandleRadioToggle()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRadioOn = !isRadioOn; // Radyo durumunu de�i�tir

            if (isRadioOn)
            {
                PlayTrack(currentTrackIndex); // �lk �ark�y� �almaya ba�la
                Debug.Log("Radyo A��ld�.");
            }
            else
            {
                radioAudioSource.Pause(); // Radyoyu durdur
                Debug.Log("Radyo Kapand�.");
            }
        }
    }

    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // Yukar� kayd�rma
        {
            NextTrack();
        }
        else if (scroll < 0f) // A�a�� kayd�rma
        {
            PreviousTrack();
        }
    }

    private void NextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= musicTracks.Count)
        {
            currentTrackIndex = 0; // Liste ba��na d�n
        }
        PlayTrack(currentTrackIndex);
    }

    private void PreviousTrack()
    {
        currentTrackIndex--;
        if (currentTrackIndex < 0)
        {
            currentTrackIndex = musicTracks.Count - 1; // Liste sonuna d�n
        }
        PlayTrack(currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        if (radioAudioSource != null && musicTracks.Count > 0)
        {
            radioAudioSource.clip = musicTracks[index];
            radioAudioSource.Play(); // �ark�y� �al
            Debug.Log("�alan �ark�: " + musicTracks[index].name);
            radioText.text = musicTracks[index].name; // UI g�ncelleme
            radioTextContainer.SetActive(true); // Text ve image'i g�ster

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