using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Test: MonoBehaviour
{
    public Button saveButton; // Sahnede atayaca��n�z buton
    public TextMeshProUGUI timeDifferenceText; // Zaman fark�n� g�sterece�imiz Text UI element'i

    private DateTime lastSavedTime; // Kaydedilen zaman
    private const string LastSavedTimeKey = "LastSavedTime";

    private void Start()
    {
        Debug.Log("ttest metodu calus�yr");
        // Butona t�klama olay�n� dinliyoruz
        saveButton.onClick.AddListener(SaveCurrentTime);
        LoadSavedTime();
    }

    private void Update()
    {
        // E�er bir zaman kaydedildiyse, g�ncel zamanla fark� hesaplay�p g�steriyoruz
        if (lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - lastSavedTime;
            string formattedTime = FormatTimeSpan(timeDifference);
            timeDifferenceText.text = $"Ge�en S�re: {formattedTime}";
        }
    }

    // Butona t�kland���nda bu metot �al��acak
    private void SaveCurrentTime()
    {
        lastSavedTime = DateTime.Now; // Mevcut zaman� kaydediyoruz
        PlayerPrefs.SetString(LastSavedTimeKey, lastSavedTime.ToString()); // PlayerPrefs'e kaydediyoruz
        PlayerPrefs.Save(); // Kaydetmeyi unutmayal�m!
      //  Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    }
    private void LoadSavedTime()
    {
        if (PlayerPrefs.HasKey(LastSavedTimeKey))
        {
            string savedTimeString = PlayerPrefs.GetString(LastSavedTimeKey);
            if (DateTime.TryParse(savedTimeString, out DateTime savedTime))
            {
                lastSavedTime = savedTime; // PlayerPrefs'ten kaydedilen zaman� y�kledik
               // Debug.Log("Kaydedilen Zaman Y�klendi: " + lastSavedTime);
            }
        }
    }
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Days} g�n {timeSpan.Hours} saat {timeSpan.Minutes} dk {timeSpan.Seconds} saniye";
    }
}
