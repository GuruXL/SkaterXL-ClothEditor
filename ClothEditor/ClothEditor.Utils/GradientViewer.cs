﻿using System.Reflection;
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

            Vector3 GradientPos = new Vector3(Main.Clothctrl.Skater_ClothParent.position.x, Main.Clothctrl.Skater_ClothParent.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.Skater_ClothParent.position.z);
            Vector3 GradientReplayPos = new Vector3(Main.Clothctrl.ReplaySkater_ClothParent.position.x, Main.Clothctrl.ReplaySkater_ClothParent.position.y + (Main.settings.GradientHeight / 100), Main.Clothctrl.ReplaySkater_ClothParent.position.z);

            AssetLoader.activeGradient.transform.position = (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState) ? GradientReplayPos : GradientPos);
        }  
    }
    
}
