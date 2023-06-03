using HarmonyLib;
using System.Reflection;
using RapidGUI;
using UnityEngine;
using GameManagement;
using UnityModManagerNet;
using ModIO.UI;
using System.IO;

namespace ClothEditor
{
    
    public class GradientViewer : MonoBehaviour
    {
        public AssetBundle bundle;
        public GameObject GradientObject;
        public GameObject activeGradientObj;

        public void Awake()
        {
            //finds asset bundle from install path
            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "visualassets"));

            if (bundle != null)
            {
                //load asset prefabs
                LoadGradientAsset();
                SpawnGradientView();
            }
        }

        public void LoadGradientAsset()
        {
            GradientObject = bundle.LoadAsset<GameObject>("GradientObject");
        }
        
        public void SpawnGradientView()
        {
            activeGradientObj = Instantiate(GradientObject);
            activeGradientObj.transform.SetParent(Main.ScriptManager.transform);
            DontDestroyOnLoad(activeGradientObj);
        }

        public void UnloadAssetBundle()
        {
            if (bundle != null)
            {
                bundle.Unload(true);
                bundle = null;
            }
            Destroy(activeGradientObj);
            Destroy(GradientObject);
        }
        public void SetGradientViewPos()
        {

            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PinMovementState))
            {
                activeGradientObj.SetActive(false);
                Main.settings.GizmosToggle = false;
            }

            Vector3 GradientPos = new Vector3(Main.Clothctrl.Skater.position.x, Main.Clothctrl.Skater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.Skater.position.z);
            Vector3 GradientReplayPos = new Vector3(Main.Clothctrl.ReplaySkater.position.x, Main.Clothctrl.ReplaySkater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.ReplaySkater.position.z);

            activeGradientObj.transform.position = (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState) ? GradientReplayPos : GradientPos);
        }  
    }
    
}
