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

    public static CivilizationRawData[] GetData()
    {
        if (Instance.RawCivilizationData == null)
            return null;

        return Instance.RawCivilizationData.GetAvailableCivilizationRawData();
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
        Addressables.Release(loadedHandler);
    }
}
