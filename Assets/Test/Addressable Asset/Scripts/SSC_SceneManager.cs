using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSC_SceneManager : MonoBehaviour
{
    private static string runtimeRemoteLoadPath = "";

    public static string RuntimeRemoteLoadPath
    {
        set { runtimeRemoteLoadPath = value; }
        get
        {
            Debug.Log($"RuntimeRemoteLoadPath {runtimeRemoteLoadPath}");
            return runtimeRemoteLoadPath;
        }
    }
}
