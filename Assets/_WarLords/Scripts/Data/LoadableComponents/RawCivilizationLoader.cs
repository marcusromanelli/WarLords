using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class RawCivilizationLoader : ILoadableComponentBaseClass
{
    AsyncOperationHandle<DataReferenceLibrary> loadHandler;

    public override bool HasLoaded()
    {
        return loadHandler.Status == AsyncOperationStatus.Succeeded;
    }
    public override void Load()
    {
        StartCoroutine(LoadRawCivilizationData());
    }
    public IEnumerator LoadRawCivilizationData()
    {
        loadHandler = Addressables.LoadAssetAsync<DataReferenceLibrary>("Available-Civilizations");

        yield return loadHandler;

        if (loadHandler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Error downloading civilization data: " + loadHandler.OperationException.ToString());
            yield break;
        }

        CivilizationManager.SetData(loadHandler);

        Debug.Log("Civilizations loaded");
    }
}
