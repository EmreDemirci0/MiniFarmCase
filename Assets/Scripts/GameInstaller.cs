using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Container.Bind<ResourceFactory>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<ResourceUIController>().FromComponentInHierarchy().AsSingle();

        //Container.BindFactory<ResourceBase, HayFactory>().AsSingle();
        //Container.Bind<HayFactory>().AsSingle();
        Container.BindFactory<ResourceBase, HayFactory>().To<HayResource>();
    }
}


public abstract class ResourceBase
{
    public abstract UniTask Produce(); // Her kaynak t�r� farkl� �retim yapacak.
}
public class HayFactory : PlaceholderFactory<ResourceBase>
{

}



public class HayResource : ResourceBase
{
    private ReactiveProperty<int> storedResources = new ReactiveProperty<int>(0); // Depodaki kaynak say�s�
    private int productionTime = 10; // �retim s�resi (saniye)
    private int maxCapacity = 5; // Fabrikan�n kapasitesi

    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    public int MaxCapacity => maxCapacity;
    public int ProductionTime => productionTime;

    private bool isProducing = false;

    // Yeni event
    public event Action<int, bool> OnProductionTimeChanged;

    // Produce metodunu async void olarak de�i�tiriyoruz
    public override async UniTask Produce()
    {
        if (isProducing)
            return;
        Debug.Log("�retim baslad�");
        if (storedResources.Value >= maxCapacity)
        {
            Debug.Log("Kapasite dolu, �retim durduruldu.");
            OnProductionTimeChanged?.Invoke(0, true);
            await UniTask.WaitUntil(() => storedResources.Value < maxCapacity);
            Debug.Log("BEKELD�");
            
            return;
        }

        isProducing = true;

        // �retim s�resi boyunca her saniye bekle
        for (int i = productionTime; i > 0; i--)
        {
            // Event tetikle, UI'y� bilgilendir
            Debug.Log("Bu ne i�e yar�yor.");
            OnProductionTimeChanged?.Invoke(i, false);
            await UniTask.Delay(1000); // 1 saniye bekle
        }

        storedResources.Value++; // Kayna�� art�r
        Debug.Log($"Hay �retildi! Mevcut Stok: {storedResources.Value}");

        // �retim bitti�inde event'i tetikle
        OnProductionTimeChanged?.Invoke(0, false); // �retim tamamland�
        isProducing = false;

        await Produce();
    }

    public int CollectResources()
    {
        int collected = storedResources.Value;
        storedResources.Value = 0; // Kaynaklar� s�f�rla
        Debug.Log($"Kaynaklar topland�: {collected}");

        Produce().Forget();
        return collected;
    }
}
