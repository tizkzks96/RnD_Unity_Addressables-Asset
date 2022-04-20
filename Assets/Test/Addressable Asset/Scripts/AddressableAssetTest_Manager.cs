using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssetTest_Manager : MonoBehaviour
{
    private static string runtimeRemoteLoadPath = "https://";

    string catalogCar = $"https://";
    string key = "Bear";
    //string key = "OSEVPE_2022_CAR";
    public static string RuntimeRemoteLoadPath
    {
        set { runtimeRemoteLoadPath = value; }
        get
        {
            Debug.Log($"RuntimeRemoteLoadPath {runtimeRemoteLoadPath}");
            return runtimeRemoteLoadPath;
        }
    }
    private AsyncOperationHandle aohDownload;

    private string downloadingKey = string.Empty;

    // Start is called before the first frame update
    async void Start()
    {
        LoadContentCatalog();
        //LoadScene_ori(key);
    }
    public void DownLoad(string key)
    {
        LoadScene_ori(key);

    }
    async void LoadScene_ori(string key)
    {
        




        await LoadContentCatalog();


        await LoadEachDependencies(key);

    }


    async UniTask LoadContentCatalog()
    {
        await Addressables.InitializeAsync().Task;

        Addressables.ClearResourceLocators();

        AsyncOperationHandle<IResourceLocator> handle;

        
        handle = Addressables.LoadContentCatalogAsync(catalogCar, true);
        await handle.Task;

        Debug.Log($"{handle.Status} {handle.Task.Result}");
    }



    async UniTask LoadEachDependencies(string key)
    {
        //this is static label for AAS


        await DownloadKey(key);


    }

    public void ClearAllCache()
    {
        //StartCoroutine(CheckCatalogs());
        //Addressables.UpdateCatalogs(true).Completed += (result) =>
        //{
        //    Debug.Log($"Clear All Cache : {result.Status}");
        //};
        Addressables.CleanBundleCache().Completed += (result) =>
        {
            Debug.Log($"Clear All Cache : {result.Status}");
        };
    }

    IEnumerator CheckCatalogs()
    {
        List<string> catalogsToUpdate = new List<string>();
        AsyncOperationHandle<List<string>> checkForUpdateHandle
            = Addressables.CheckForCatalogUpdates();
        checkForUpdateHandle.Completed += op =>
        {
            catalogsToUpdate.AddRange(op.Result);
        };

        yield return checkForUpdateHandle;

        if (catalogsToUpdate.Count > 0)
        {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle
                = Addressables.UpdateCatalogs(true, catalogsToUpdate);
            yield return updateHandle;
            Addressables.Release(updateHandle);
        }

        //Addressables.Release(checkForUpdateHandle);
    }

    public void RemoveDependencies(string key)
    {
        Addressables.ClearDependencyCacheAsync(key, false).Completed += (result) =>
        {
            Debug.Log($"{{{key}}} remove result : {result.Status}");
        };
    }

    async UniTask DownloadKey(string key)
    {
        AsyncOperationHandle<long> aohSize = Addressables.GetDownloadSizeAsync(key);
        await aohSize.Task;
        Debug.Log($"GetDownloadSizeAsync {key} {aohSize.Status} \n {aohSize.Result}\n {RuntimeRemoteLoadPath}");

        if (aohSize.Result > 0)
        {
            Debug.Log($"Download Asset {key}");
            await UniTask.Delay(100);
            aohDownload = Addressables.DownloadDependenciesAsync(key);

            downloadingKey = key;

            aohDownload.Completed += (s) => {
                Debug.Log($"{ToString()} DownloadDependenciesAsync {aohDownload.IsDone} {aohDownload.Status} {aohDownload.PercentComplete}");

            };
        }

        if (aohSize.IsValid())
            Addressables.Release(aohSize);

        if (aohDownload.IsValid())
            Addressables.Release(aohDownload);

    }

}
