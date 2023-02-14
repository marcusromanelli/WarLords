using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class Initializer : MonoBehaviour {

    AsyncOperationHandle<CivilizationCollection> opHandle;

    public IEnumerator Start()
    {
        opHandle = Addressables.LoadAssetAsync<CivilizationCollection>("Available-Civilizations");
        yield return opHandle;

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            CivilizationCollection obj = opHandle.Result;

            if(obj.Civilizations.Length > 0)
            {
                Debug.Log("Available decks: ");

                foreach (var item in obj.Civilizations)
                {
                    Debug.Log(item.ToString() + ",  ");
                }
            }
        }
    }

    void OnDestroy()
    {
        Addressables.Release(opHandle);
    }
}
