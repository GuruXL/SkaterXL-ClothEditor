using System.Reflection;
using UnityEngine;
using GameManagement;
using ClothEditor.Utils;

namespace ClothEditor.Utils
{   
    public class GradientViewer : MonoBehaviour
    {
        public void SetGradientViewPos()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PinMovementState))
            {
                AssetLoader.activeGradient.SetActive(false);
                Main.settings.GizmosToggle = false;
            }

            Vector3 GradientPos = new Vector3(Main.Clothctrl.Skater.position.x, Main.Clothctrl.Skater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.Skater.position.z);
            Vector3 GradientReplayPos = new Vector3(Main.Clothctrl.ReplaySkater.position.x, Main.Clothctrl.ReplaySkater.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.ReplaySkater.position.z);

            AssetLoader.activeGradient.transform.position = (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState) ? GradientReplayPos : GradientPos);
        }  
    }
    
}
