using System;
using UnityEngine;

public static class PlayerPrefsHelper
{
    // String veri kaydetme ve yükleme
    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;
    }

    // Int veri kaydetme ve yükleme
    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
    }

    // Float veri kaydetme ve yükleme
    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    // DateTime veri kaydetme ve yükleme
    public static void SaveDateTime(string key, DateTime dateTime)
    {
        string dateTimeString = dateTime.ToString("o"); // ISO 8601 formatý
        SaveString(key, dateTimeString);
    }

    public static DateTime LoadDateTime(string key, DateTime defaultValue = default)
    {
        string dateTimeString = LoadString(key);
        if (DateTime.TryParse(dateTimeString, out DateTime result))
        {
            //Debug.Log("Kaydedilen Tarih: "+result);
            return result;
        }
        return defaultValue;
    }

    // Veri silme
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    // Tüm verileri temizleme
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}