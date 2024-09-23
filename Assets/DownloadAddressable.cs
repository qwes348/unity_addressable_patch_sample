using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;

public class DownloadAddressable : MonoBehaviour
{
    [SerializeField] private string labelForDownload;
    [SerializeField] private AssetReferenceSprite assetForLoad;

    [Title("UI")]
    [SerializeField] private Text sizeText;
    [SerializeField] private Text percentText;
    [SerializeField] private Image resultImage;

    private AsyncOperationHandle currentHandle;

    public void StartDownload()
    {
        currentHandle = Addressables.DownloadDependenciesAsync(labelForDownload);
        currentHandle.Completed += (handle) =>
        {
            Addressables.Release(handle);
            Addressables.LoadAssetAsync<Sprite>(assetForLoad).Completed += handle =>
            {
                resultImage.sprite = handle.Result;
            };
        };

        StartCoroutine(PercentageCor());
    }

    IEnumerator PercentageCor()
    {
        while (!currentHandle.IsDone)
        {
            percentText.text = string.Format("{0}%", (currentHandle.PercentComplete * 100f).ToString("N0"));
            yield return null;
        }
    }

    public void CheckSize()
    {
        Addressables.GetDownloadSizeAsync(labelForDownload).Completed += (sizeHandle) =>
        {
            Debug.Log(sizeHandle.PercentComplete);
            string size = string.Concat(sizeHandle.Result, " byte");
            sizeText.text = size;

            Addressables.Release(sizeHandle);
        };

    }
}
