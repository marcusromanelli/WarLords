using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CivilizationManager : Singleton<CivilizationManager>
{
	[SerializeField] CivilizationCollection RawCivilizationData;

    AsyncOperationHandle<CivilizationCollection> loadedHandler;

    bool initialized = false;

    public static RawBundleData[] GetData()
    {
        if (Instance.RawCivilizationData == null)
            return null;

        return Instance.RawCivilizationData.GetAvailableCivilizationRawData();
    }    
    public static RawBundleData? GetData(string Id)
    {
        if (Instance.RawCivilizationData == null)
            return null;

        return Instance.RawCivilizationData.GetCivilizationRawData(Id);
    }    
    public static void SetData(AsyncOperationHandle<CivilizationCollection> civilizationHandler)
    {
        Instance.setData(civilizationHandler);
    }
    public void setData(AsyncOperationHandle<CivilizationCollection> civilizationHandler)
    {
        if (initialized)
            return;

        loadedHandler = civilizationHandler;
        RawCivilizationData = civilizationHandler.Result;
        initialized = true; 
    }
    void OnDestroy()
    {
        if(loadedHandler.IsValid())
            Addressables.Release(loadedHandler);
    }
}
