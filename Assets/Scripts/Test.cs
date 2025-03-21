using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Test: MonoBehaviour
{
    public Button saveButton; // Sahnede atayacaðýnýz buton
    public TextMeshProUGUI timeDifferenceText; // Zaman farkýný göstereceðimiz Text UI element'i

    private DateTime lastSavedTime; // Kaydedilen zaman
    private const string LastSavedTimeKey = "LastSavedTime";

    private void Start()
    {
        Debug.Log("ttest metodu calusýyr");
        // Butona týklama olayýný dinliyoruz
        saveButton.onClick.AddListener(SaveCurrentTime);
        LoadSavedTime();
    }

    private void Update()
    {
        // Eðer bir zaman kaydedildiyse, güncel zamanla farký hesaplayýp gösteriyoruz
        if (lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - lastSavedTime;
            string formattedTime = FormatTimeSpan(timeDifference);
            timeDifferenceText.text = $"Geçen Süre: {formattedTime}";
        }
    }

    // Butona týklandýðýnda bu metot çalýþacak
    private void SaveCurrentTime()
    {
        lastSavedTime = DateTime.Now; // Mevcut zamaný kaydediyoruz
        PlayerPrefs.SetString(LastSavedTimeKey, lastSavedTime.ToString()); // PlayerPrefs'e kaydediyoruz
        PlayerPrefs.Save(); // Kaydetmeyi unutmayalým!
      //  Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    }
    private void LoadSavedTime()
    {
        if (PlayerPrefs.HasKey(LastSavedTimeKey))
        {
            string savedTimeString = PlayerPrefs.GetString(LastSavedTimeKey);
            if (DateTime.TryParse(savedTimeString, out DateTime savedTime))
            {
                lastSavedTime = savedTime; // PlayerPrefs'ten kaydedilen zamaný yükledik
               // Debug.Log("Kaydedilen Zaman Yüklendi: " + lastSavedTime);
            }
        }
    }
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Days} gün {timeSpan.Hours} saat {timeSpan.Minutes} dk {timeSpan.Seconds} saniye";
    }
}
