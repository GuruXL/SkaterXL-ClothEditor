using System.Reflection;
using UnityEngine;
using GameManagement;
using System.IO;
using ClothEditor.Controller;

namespace ClothEditor.Utils
{   
    public class GradientViewer : MonoBehaviour
    {
        public GameObject activeGradient;

        public void Awake()
        {
            if (AssetLoader.GradientObject != null)
            {
                SpawnGradientView();
            }   
        }

        public void SpawnGradientView()
        {
            activeGradient = Instantiate(AssetLoader.GradientObject);
            activeGradient.transform.SetParent(Main.ScriptManager.transform);
            DontDestroyOnLoad(activeGradient);
        }

        public void UnloadAssetBundle()
        {
            if (AssetLoader.assetBundle != null)
            {
                AssetLoader.assetBundle.Unload(true);
                AssetLoader.assetBundle = null;
            }
            Destroy(activeGradient);
            Destroy(AssetLoader.GradientObject);
        }
        public void SetGradientViewPos()
        {

            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PinMovementState))
            {
                activeGradient.SetActive(false);
                Main.settings.GizmosToggle = false;
            }

            Vector3 GradientPos = new Vector3(Main.Clothctrl.Skater.position.x, Main.Clothctrl.Skater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.Skater.position.z);
            Vector3 GradientReplayPos = new Vector3(Main.Clothctrl.ReplaySkater.position.x, Main.Clothctrl.ReplaySkater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.ReplaySkater.position.z);

            activeGradient.transform.position = (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState) ? GradientReplayPos : GradientPos);
        }  
    }
    
}
