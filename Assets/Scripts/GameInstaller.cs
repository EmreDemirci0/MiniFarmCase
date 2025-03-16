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
    public abstract UniTask Produce(); // Her kaynak türü farklý üretim yapacak.
}
public class HayFactory : PlaceholderFactory<ResourceBase>
{

}



public class HayResource : ResourceBase
{
    private ReactiveProperty<int> storedResources = new ReactiveProperty<int>(0); // Depodaki kaynak sayýsý
    private int productionTime = 10; // Üretim süresi (saniye)
    private int maxCapacity = 5; // Fabrikanýn kapasitesi

    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    public int MaxCapacity => maxCapacity;
    public int ProductionTime => productionTime;

    private bool isProducing = false;

    // Yeni event
    public event Action<int, bool> OnProductionTimeChanged;

    // Produce metodunu async void olarak deðiþtiriyoruz
    public override async UniTask Produce()
    {
        if (isProducing)
            return;
        Debug.Log("Üretim basladý");
        if (storedResources.Value >= maxCapacity)
        {
            Debug.Log("Kapasite dolu, üretim durduruldu.");
            OnProductionTimeChanged?.Invoke(0, true);
            await UniTask.WaitUntil(() => storedResources.Value < maxCapacity);
            Debug.Log("BEKELDÝ");
            
            return;
        }

        isProducing = true;

        // Üretim süresi boyunca her saniye bekle
        for (int i = productionTime; i > 0; i--)
        {
            // Event tetikle, UI'yý bilgilendir
            Debug.Log("Bu ne iþe yarýyor.");
            OnProductionTimeChanged?.Invoke(i, false);
            await UniTask.Delay(1000); // 1 saniye bekle
        }

        storedResources.Value++; // Kaynaðý artýr
        Debug.Log($"Hay üretildi! Mevcut Stok: {storedResources.Value}");

        // Üretim bittiðinde event'i tetikle
        OnProductionTimeChanged?.Invoke(0, false); // Üretim tamamlandý
        isProducing = false;

        await Produce();
    }

    public int CollectResources()
    {
        int collected = storedResources.Value;
        storedResources.Value = 0; // Kaynaklarý sýfýrla
        Debug.Log($"Kaynaklar toplandý: {collected}");

        Produce().Forget();
        return collected;
    }
}
