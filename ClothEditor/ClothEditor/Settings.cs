using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace ClothEditor
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public static Settings Instance { get; set; }

        // ----- Start Set KeyBindings ------
        public KeyBinding Hotkey = new KeyBinding { keyCode = KeyCode.C };

        private static readonly KeyCode[] keyCodes = Enum.GetValues(typeof(KeyCode))
            .Cast<KeyCode>()
            .Where(k => ((int)k < (int)KeyCode.Mouse0))
            .ToArray();

        // Get Key on KeyPress
        public static KeyCode? GetCurrentKeyDown()
        {
            if (!Input.anyKeyDown)
            {
                return null;
            }

            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKey(keyCodes[i]))
                {
                    return keyCodes[i];
                }
            }
            return null;
        }

        // Gets children of an object (true for recursive)
        public List<Transform> GetChildren(Transform parent, bool recursive)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform child in parent)
            {
                children.Add(child);
                if (recursive)
                {
                    children.AddRange(GetChildren(child, true));
                }
            }
            return children;
        }

        // ----- End Set KeyBindings ------

        public Color BGColor = new Color(0.0f, 0.0f, 0.0f);
        public bool enabled = false;

        public string cloth_target = "None";
        public string PlayerCloth_target = "None";
        //public string BoneTarget = "Skater_pelvis";

        //----- Toggles ----------
        public bool HotKeyToggle = false;
        public bool ClothUIToggle = false;
        public bool GetClothesToggle = false;
        public bool AddClothToggle = false;
        public bool ArmupToggle = false;
        public bool GizmosToggle = false;

        // ---- Cloth Settings ----
        public float ClothGravity_x = 0.0f;
        public float ClothGravity_y = -9.81f;
        public float ClothGravity_z = 0.0f;

        public float Clothacc_x = 0.0f;
        public float Clothacc_y = 0.0f;
        public float Clothacc_z = 0.0f;

        //DefaultValues
        public float DefaultDamping = 0.01f;
        public float DefaultSolverFreq = 120f;
        public float DefaultFriction = 0.1f;
        public float DefaultBendingStiff = 0.85f;
        public float DefaultSleepThreshold = 0.10f;
        public float DefaultStiffnessFreq = 10f;
        public float DefaultStretchingStiff = 0.75f;
        public float DefaultWorldAcc = 0.05f;
        public float DefaultWorldVel = 0.40f;

        [SerializeField]
        public float DampingFlt;
        [SerializeField]
        public float SolverFreqFlt;
        [SerializeField]
        public float FrictionFlt;
        [SerializeField]
        public float BendingStiffFlt;
        [SerializeField]
        public float SleepThresholdFlt;
        [SerializeField]
        public float StiffnessFreqFlt;
        [SerializeField]
        public float StretchingStiffFlt;
        [SerializeField]
        public float WorldAccFlt;
        [SerializeField]
        public float WorldVelFlt;
        [SerializeField]
        public float ClothMaxDistance = 1.0f;
        [SerializeField]
        public float ClothSphereDistance = 0.10f;
        [SerializeField]   
        public float GradientHeight = 0.0f;

        public float ArmHeight = 1.0f;

        public float GradientMin_x = 0.0f;
        public float GradientMin_y = 0.0f;
        public float GradientMin_z = 0.0f;
        public float GradientMax_x = 0.0f;
        public float GradientMax_y = 0.0f;
        public float GradientMax_z = 0.0f;

        public void resetToggles()
        {
            //ClothUIToggle = false;
            GetClothesToggle = false;
            AddClothToggle = false;
            GizmosToggle = false;
            ArmupToggle = false;
            cloth_target = "None";
            PlayerCloth_target = "None";

    }
        public void OnChange()
        {
            throw new NotImplementedException();
        }
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            //Save(this, modEntry);
        }
    }
}
