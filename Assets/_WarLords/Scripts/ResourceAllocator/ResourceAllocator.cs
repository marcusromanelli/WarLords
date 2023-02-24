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

public class AssetHandleReference<T>
{
    public AssetReference AssetReference;
    public T Data => AssetOperation.Result;

    private Action<T> OnFinishLoading;
    private int useNumber;
    private AsyncOperationHandle<T> AssetOperation;

    public bool HasLoaded()
    {
        return AssetOperation.IsValid() && AssetOperation.Status == AsyncOperationStatus.Succeeded;
    }
    public bool IsLoading()
    {
        return AssetOperation.IsValid() && AssetOperation.Status != AsyncOperationStatus.Succeeded;
    }

    public void Load(Action<T> OnFinishLoading)
    {
        useNumber++;

        if (HasLoaded())
        {
            OnFinishLoading?.Invoke(Data);
            return;
        }

        this.OnFinishLoading += OnFinishLoading;

        if (IsLoading())
            return;

        AssetOperation = Addressables.LoadAssetAsync<T>(AssetReference);

        AssetOperation.Completed += OnOperationFinishes;
    }

    void OnOperationFinishes(AsyncOperationHandle<T> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
            return;

        OnFinishLoading?.Invoke(handle.Result);

        AssetOperation.Completed -= OnOperationFinishes;
        OnFinishLoading = null;
    }

    public void Unload()
    {
        useNumber--;

        var isValid = AssetOperation.IsValid();

        if (isValid && useNumber <= 0)
        {
            useNumber = 0;
            Addressables.Release(AssetOperation);
        }
    }
}

public class ResourceAllocator<T>
{
    private HashSet<AssetHandleReference<T>> datahandler = new HashSet<AssetHandleReference<T>>(new AssetReferenceComparer<T>());

    public void Load(AssetReference assetReference, Action<T> onLoadFinish)
    {
        AssetHandleReference<T> searchData = new AssetHandleReference<T>() { AssetReference = assetReference };

        AssetHandleReference<T> foundData = searchData;

        if (!datahandler.TryGetValue(searchData, out foundData))
        {
            datahandler.Add(searchData);

            foundData = searchData;
        }

        foundData.Load(onLoadFinish);
    }

    public void Unload(AssetReference assetReference)
    {
        AssetHandleReference<T> data = new AssetHandleReference<T>() { AssetReference = assetReference };

        datahandler.TryGetValue(data, out data);

        data.Unload();
    }
}
