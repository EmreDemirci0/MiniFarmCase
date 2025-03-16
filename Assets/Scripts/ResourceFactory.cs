using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class ResourceFactory : MonoBehaviour
{
    //[SerializeField] private ResourceType producedResource = ResourceType.Wheat; // Üretilecek kaynak türü
    //[SerializeField] private int productionTime = 20; // Bir kaynağın üretim süresi (saniye)
    //[SerializeField] private int maxCapacity = 5; // Fabrikanın kapasitesi

    //private ReactiveProperty<int> storedResources = new ReactiveProperty<int>(0); // Depodaki kaynak sayısı

    //public int MaxCapacity => maxCapacity; // Eklendi
    //public int ProductionTime => productionTime; // Eklendi

    //private bool isProducing = false;

    //public IReadOnlyReactiveProperty<int> StoredResources => storedResources; // Dışarıdan okunabilir

    //private void Start()
    //{
    //    StartProduction().Forget(); // Üretim sürecini başlat
    //}
    //public event Action<int, bool> OnProductionTimeChanged;

    //private async UniTaskVoid StartProduction()
    //{
    //    while (true) // Sonsuz bir döngü ile sürekli üretim kontrolü
    //    {
    //        Debug.Log("While Girdi");
    //        bool isFull = storedResources.Value >= maxCapacity; // Depo dolu mu kontrol et

    //        if (!isFull) // Kapasite dolu değilse üret
    //        {
    //            for (int i = productionTime; i > 0; i--)
    //            {
    //                OnProductionTimeChanged?.Invoke(i, false); // UI'ye zaman güncellemesi gönder
    //                await UniTask.Delay(TimeSpan.FromSeconds(1)); // 1 saniye bekle
    //            }

    //            storedResources.Value++; // Üretilen kaynağı ekle
    //            Debug.Log($"Üretildi: {producedResource}, Mevcut Stok: {storedResources.Value}");
    //        }
    //        else
    //        {
    //            Debug.Log("Kapasite dolu, üretim durduruldu.");
    //            OnProductionTimeChanged?.Invoke(0, true); // Depo dolu mesajı gönder
    //            await UniTask.WaitUntil(() => storedResources.Value < maxCapacity); // Kapasite boşalana kadar bekle
    //        }
    //    }
    //}


    //public int CollectResources()
    //{
    //    int collected = storedResources.Value;
    //    storedResources.Value = 0; // Kaynakları sıfırla
    //    Debug.Log($"Kaynaklar toplandı: {collected}");
    //    return collected;
    //}
}
