using System.Reflection;
using UnityEngine;
using GameManagement;
using System.IO;
using System.Collections;
using System;
using ModIO.UI;
using Object = UnityEngine.Object;

namespace ClothEditor
{
    public static class AssetLoader
    {
        public static GameObject GradientObject;
        public static GameObject activeGradient;
        public static AssetBundle assetBundle;
        private static bool prefabsLoaded = false;
        private static bool prefabsSpawned = false;

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

        public static GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject instance = Object.Instantiate(prefab);
            return instance;
        }

        private static byte[] ExtractResources(string filename)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (manifestResourceStream == null)
                    return null;

                byte[] buffer = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        private static IEnumerator LoadAssetBundle()
        {
            byte[] assetBundleData = ExtractResources("ClothEditor.Resources.visualassets");
            if (assetBundleData == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Failed to EXTRACT ClothEditor Asset Bundle", 2.5f);
                yield break;
            }
            AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
            yield return abCreateRequest;

            assetBundle = abCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Failed to LOAD ClothEditor Asset Bundle", 2.5f);
                yield break;
            }
            yield return PlayerController.Instance.StartCoroutine(LoadPrefabs());
            yield return new WaitUntil(() => prefabsLoaded == true);
            yield return PlayerController.Instance.StartCoroutine(InstantiatePrefabs());
        }

        private static IEnumerator LoadPrefabs()
        {
            if (assetBundle == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"ClothEditor Asset bundles are not loaded!", 2.5f);
                yield break;
            }
            yield return PlayerController.Instance.StartCoroutine(LoadGradientPrefab());

            // Wait for all prefabs to finish loading before setting prefabsLoaded to true
            while (GradientObject == null)
            {
                yield return null;
            }
            prefabsLoaded = true;
        }

        private static IEnumerator LoadGradientPrefab()
        {
            GradientObject = assetBundle.LoadAsset<GameObject>("GradientObject");
            yield return null;
        }

        private static IEnumerator InstantiatePrefabs()
        {
            activeGradient = InstantiatePrefab(GradientObject);
            activeGradient.transform.SetParent(Main.ScriptManager.transform);
            activeGradient.SetActive(false);

            // Wait until all prefabs are instantiated
            yield return new WaitUntil(() => activeGradient != null);
            prefabsSpawned = true;
        }

        public static void UnloadAssetBundle()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
            prefabsLoaded = false;
            prefabsSpawned = false;
        }

        private static void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}
