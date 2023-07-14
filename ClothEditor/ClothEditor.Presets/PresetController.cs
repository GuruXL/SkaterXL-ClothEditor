using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ModIO.UI;

namespace ClothEditor.Presets
{
    public class PresetController : MonoBehaviour
    {
        public PresetSettings savePreset;
        public PresetSettings loadedPreset;
        public GameObject savePresetGO;
        public GameObject loadedPresetGO;

        private string mainPath;
        public string PresetName = "";
        public string PresetToLoad = "Select Preset to Load";
        string LastPresetLoaded = "Select Preset to Load";

        public void Awake()
        {
            mainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SkaterXL\\ClothEditor\\");

            if (!Directory.Exists(mainPath + "ClothPresets"))
            {
                Directory.CreateDirectory(mainPath + "ClothPresets");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"ClothPresets folder created", 2.5f);
                Main.Logger.Log("No ClothPresets Folder Found: new folder has been created");
            }

            CreatePresets();
        }

        public void Update()
        {
            if (PresetToLoad != "Select Preset to Load")
            {
                LoadPreset();
            }
        }

        void CreatePresets()
        {
            savePresetGO = new GameObject("SavePresetSettings");
            loadedPresetGO = new GameObject("LoadPresetSettings");
            savePresetGO.transform.SetParent(Main.PresetManager.transform);
            loadedPresetGO.transform.SetParent(Main.PresetManager.transform);
            savePreset = savePresetGO.AddComponent<PresetSettings>();
            loadedPreset = loadedPresetGO.AddComponent<PresetSettings>();
            DontDestroyOnLoad(savePresetGO);
            DontDestroyOnLoad(loadedPresetGO);
        }

        public void SavePreset()
        {
            savePreset.DampingFlt = Main.settings.DampingFlt;
            savePreset.SolverFreqFlt = Main.settings.SolverFreqFlt;
            savePreset.FrictionFlt = Main.settings.FrictionFlt;
            savePreset.BendingStiffFlt = Main.settings.BendingStiffFlt;
            savePreset.SleepThresholdFlt = Main.settings.SleepThresholdFlt;
            savePreset.StiffnessFreqFlt = Main.settings.StiffnessFreqFlt;
            savePreset.StretchingStiffFlt = Main.settings.StretchingStiffFlt;
            savePreset.WorldAccFlt = Main.settings.WorldAccFlt;
            savePreset.WorldVelFlt = Main.settings.WorldVelFlt;
            savePreset.ClothMaxDistance = Main.settings.ClothMaxDistance;
            savePreset.ClothSphereDistance = Main.settings.ClothSphereDistance;
            savePreset.GradientHeight = Main.settings.GradientHeight;

            string json = JsonUtility.ToJson(savePreset);
            File.WriteAllText(mainPath + "ClothPresets\\" + $"{PresetName}.json", json);

            MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{PresetName} Preset Created", 2.5f);

            PresetName = "";
        }

        public void ResetPresetList()
        {
            PresetToLoad = "Select Preset to Load";
            LastPresetLoaded = "Select Preset to Load";
        }

        public void LoadPreset()
        {
            if (LastPresetLoaded != PresetToLoad)
            {
                string json = File.ReadAllText(mainPath + "ClothPresets\\" + $"{PresetToLoad}.json");
                JsonUtility.FromJsonOverwrite(json, loadedPreset);

                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"{PresetToLoad} Preset Loaded", 2.5f);

                LastPresetLoaded = PresetToLoad;
            }
        }

        public void ApplyPreset()
        {
            if (PresetToLoad != "Select Preset to Load")
            {
                Main.settings.DampingFlt = loadedPreset.DampingFlt;
                Main.settings.SolverFreqFlt = loadedPreset.SolverFreqFlt;
                Main.settings.FrictionFlt = loadedPreset.FrictionFlt;
                Main.settings.BendingStiffFlt = loadedPreset.BendingStiffFlt;
                Main.settings.SleepThresholdFlt = loadedPreset.SleepThresholdFlt;
                Main.settings.StiffnessFreqFlt = loadedPreset.StiffnessFreqFlt;
                Main.settings.StretchingStiffFlt = loadedPreset.StretchingStiffFlt;
                Main.settings.WorldAccFlt = loadedPreset.WorldAccFlt;
                Main.settings.WorldVelFlt = loadedPreset.WorldVelFlt;
                Main.settings.ClothMaxDistance = loadedPreset.ClothMaxDistance;
                Main.settings.ClothSphereDistance = loadedPreset.ClothSphereDistance;
                Main.settings.GradientHeight = loadedPreset.GradientHeight;

                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{PresetToLoad} Preset Applied", 2.5f);
            }
        }

        public string[] GetPresetNames()
        {
            string[] NullState = new string[] { "Select Preset to Load" };

            if (mainPath != null)
            {
                string[] jsons = Directory.GetFiles(mainPath + "ClothPresets\\", "*.json");
                string[] names = new string[jsons.Length];

                int i = 0;
                foreach (string name in jsons)
                {
                    names[i] = Path.GetFileNameWithoutExtension(name);
                    i++;
                }
                if (i > 0)
                {
                    return names;
                }
                else
                {
                    return NullState;
                }
            }
            return NullState;
        }
    }
}
