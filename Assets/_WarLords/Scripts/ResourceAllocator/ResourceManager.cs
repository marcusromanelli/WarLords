using System;
using UnityEngine.AddressableAssets;

public class ResourceManager : Singleton<ResourceManager>
{
    public ResourceAllocator<Card> cardResourceAllocator = new ResourceAllocator<Card>();


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
}