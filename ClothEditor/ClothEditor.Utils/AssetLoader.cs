using System.Reflection;
using UnityEngine;
using GameManagement;
using System.IO;
using System.Collections;
using System;

namespace ClothEditor.Utils
{
    
    public static class AssetLoader
    {
        public static GameObject GradientObject;
        public static AssetBundle assetBundle;

        public static void LoadBundles()
        {
            // Check if a type from the Unity assembly has been loaded
            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null)
            {
                // start asset loading
                PlayerController.Instance.StartCoroutine(LoadAssetBundle()); // 1.2.2.8
                //PlayerController.Instance.StartCoroutine(LoadAssetBundle()); // 1.2.6.0             
            }
        }

        private static IEnumerator LoadAssetBundle()
        {
            AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(ExtractResources("ClothEditor.Resources.visualassets"));
            yield return abCreateRequest;
            AssetBundleCreateRequest currentBundleRequest = abCreateRequest;
            assetBundle = currentBundleRequest != null ? currentBundleRequest.assetBundle : null;

            GradientObject = assetBundle.LoadAsset<GameObject>("GradientObject");
        }

        private static byte[] ExtractResources(string filename)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (manifestResourceStream == null)
                    return (byte[])null;
                byte[] buffer = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
