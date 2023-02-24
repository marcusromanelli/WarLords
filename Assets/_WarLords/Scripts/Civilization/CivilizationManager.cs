using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CivilizationManager : Singleton<CivilizationManager>
{
	[SerializeField] DataReferenceLibrary RawCivilizationData;

    AsyncOperationHandle<DataReferenceLibrary> loadedHandler;

    bool initialized = false;
    RawBundleData[] civilizationDataArray;

    public static RawBundleData GetData(string id)
    {
        if (Instance.RawCivilizationData == null)
            return default;

        return Instance.RawCivilizationData.GetCivilization(id);
    }
    public static RawBundleData[] GetAll()
    {
        return Instance.getAll();
    }
    RawBundleData[] getAll()
    {
        if (Instance.RawCivilizationData == null)
            return new RawBundleData[0];

        if (civilizationDataArray == null || civilizationDataArray.Length == 0)
        {
            var data = Instance.RawCivilizationData?.GetCivilizations();

            civilizationDataArray = data;
        }

        return civilizationDataArray;
    }
    public static void SetData(AsyncOperationHandle<DataReferenceLibrary> civilizationHandler)
    {
        Instance.setData(civilizationHandler);
    }
    public void setData(AsyncOperationHandle<DataReferenceLibrary> civilizationHandler)
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
