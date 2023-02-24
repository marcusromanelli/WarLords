using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


class AssetReferenceComparer<T> : IEqualityComparer<AssetHandleReference<T>>
{
    public bool Equals(AssetHandleReference<T> x, AssetHandleReference<T> y)
    {
        return x.AssetReference.AssetGUID == y.AssetReference.AssetGUID;
    }

    public int GetHashCode(AssetHandleReference<T> obj)
    {
        return obj.AssetReference.AssetGUID.GetHashCode();
    }
}

public struct AssetHandleReference<T>
{
    public AssetReference AssetReference;
    public Action<T> OnFinishLoading;
    public T Data => AssetOperation.Result;


    private AsyncOperationHandle<T> AssetOperation;

    public bool HasLoaded()
    {
        return AssetOperation.IsValid() && AssetOperation.Status == AsyncOperationStatus.Succeeded;
    }
    public bool IsLoading()
    {
        return AssetOperation.IsValid() && AssetOperation.Status != AsyncOperationStatus.Succeeded;
    }

    public void Load()
    {
        AssetOperation = Addressables.LoadAssetAsync<T>(AssetReference);

        AssetHandleReference<T> obj = this; //derp

        AssetOperation.Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Error loading data: " + handle.OperationException.ToString());
                return;
            }

            obj.OnFinishLoading?.Invoke(handle.Result);
        };
    }

    public void Unload()
    {
        if (AssetOperation.IsValid())
            Addressables.Release(AssetOperation);
    }
}

public class ResourceAllocator<T>
{
    private HashSet<AssetHandleReference<T>> datahandler = new HashSet<AssetHandleReference<T>>(new AssetReferenceComparer<T>());

    public void Load(AssetReference assetReference, Action<T> onLoadFinish)
    {
        AssetHandleReference<T> data = new AssetHandleReference<T>() { AssetReference = assetReference };

        AssetHandleReference<T> foundData = data;

        datahandler.TryGetValue(data, out foundData);

        if(foundData.AssetReference != null)
            data = foundData;

        if (data.HasLoaded())
        {
            onLoadFinish?.Invoke(data.Data);
            return;
        }

        data.OnFinishLoading += onLoadFinish;

        if (data.IsLoading())
            return;

        data.Load();

        datahandler.Add(data);
    }

    public void Unload(AssetReference assetReference)
    {
        AssetHandleReference<T> data = new AssetHandleReference<T>() { AssetReference = assetReference };

        datahandler.TryGetValue(data, out data);

        data.Unload();
    }
}
