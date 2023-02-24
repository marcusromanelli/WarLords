using System;
using UnityEngine.AddressableAssets;

public class ResourceManager : Singleton<ResourceManager>
{
    public ResourceAllocator<Card> cardResourceAllocator = new ResourceAllocator<Card>();
    public ResourceAllocator<CivilizationData> civilizationResourceAllocator = new ResourceAllocator<CivilizationData>();


    public static void AllocateCard(AssetReference assetReference, Action<Card> onFinishLoad)
    {
        Instance.allocateCard(assetReference, onFinishLoad);
    }
    public static void DeallocateCard(AssetReference assetReference)
    {
        Instance.deallocateCard(assetReference);
    }

    void allocateCard(AssetReference assetReference, Action<Card> onFinishLoad)
    {
        cardResourceAllocator.Load(assetReference, onFinishLoad);
    }
    void deallocateCard(AssetReference assetReference)
    {
        cardResourceAllocator.Unload(assetReference);
    }


    public static void AllocateCivilization(AssetReference assetReference, Action<CivilizationData> onFinishLoad)
    {
        Instance.allocateCivilization(assetReference, onFinishLoad);
    }
    public static void DeallocateCivilization(AssetReference assetReference)
    {
        Instance.deallocateCivilization(assetReference);
    }

    void allocateCivilization(AssetReference assetReference, Action<CivilizationData> onFinishLoad)
    {
        civilizationResourceAllocator.Load(assetReference, onFinishLoad);
    }
    void deallocateCivilization(AssetReference assetReference)
    {
        civilizationResourceAllocator.Unload(assetReference);
    }
}