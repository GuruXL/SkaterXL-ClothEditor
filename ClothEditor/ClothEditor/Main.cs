using HarmonyLib;
using System.Reflection;
using RapidGUI;
using UnityEngine;
using UnityModManagerNet;
using ModIO.UI;

namespace ClothEditor
{
    //[EnableReloading]
    internal static class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static Harmony harmonyInstance;
        public static string modId = "ClothEditor";
        public static UnityModManager.ModEntry modEntry;
        public static GameObject ScriptManager;
        public static GameObject PresetManager;

        // Main classes
        public static ClothController Clothctrl;
        public static UIController UIctrl;

        // Preset classes
        public static PresetSettings presetSettings;
        public static PresetController PresetCtrl;

        //Utils
        public static GradientViewer Gradientctrl;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = new System.Action<UnityModManager.ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new System.Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
            modEntry.OnUnload = new System.Func<UnityModManager.ModEntry, bool>(Unload);
            Main.modEntry = modEntry;
            Logger.Log(nameof(Load));

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical(GUILayout.Width(256));
            if (RGUI.Button(settings.HotKeyToggle, "Change HotKey"))
            {
                settings.HotKeyToggle = !settings.HotKeyToggle;
            }

            if (settings.HotKeyToggle)
            {
                GUILayout.Label("<b>Press any Key to change HotKey</b>");
                GUILayout.Box("<b>Current HotKey: Ctrl + </b>" + settings.Hotkey.keyCode.ToString(""), GUILayout.Height(25f));
                if (Settings.GetCurrentKeyDown() != null)
                {
                    settings.Hotkey = new KeyBinding { keyCode = (KeyCode)Settings.GetCurrentKeyDown() };
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"ClothEditor HotKey Changed to: Ctrl + " + settings.Hotkey.keyCode.ToString(""), 2.5f);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(256));
            GUILayout.Box("<b>Background Colour</b>", GUILayout.Height(21f));
            settings.BGColor.r = RGUI.SliderFloat(settings.BGColor.r, 0.0f, 1f, 0.0f, "Red");
            settings.BGColor.g = RGUI.SliderFloat(settings.BGColor.g, 0.0f, 1f, 0.0f, "Green");
            settings.BGColor.b = RGUI.SliderFloat(settings.BGColor.b, 0.0f, 1f, 0.0f, "Blue");
            GUILayout.EndVertical();

        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            //settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            bool flag;
            if (enabled == value)
            {
                flag = true;
            }
            else
            {
                enabled = value;
                if (enabled)
                {
                    harmonyInstance = new Harmony((modEntry.Info).Id);
                    harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                    ScriptManager = new GameObject("ClothEditor");
                    PresetManager = new GameObject("PresetManager");
                    PresetManager.transform.SetParent(ScriptManager.transform);

                    Clothctrl = ScriptManager.AddComponent<ClothController>();
                    UIctrl = ScriptManager.AddComponent<UIController>();

                    presetSettings = PresetManager.AddComponent<PresetSettings>();
                    PresetCtrl = PresetManager.AddComponent<PresetController>();

                    Gradientctrl = ScriptManager.AddComponent<GradientViewer>();

                    Object.DontDestroyOnLoad(ScriptManager);
                    //Object.DontDestroyOnLoad(PresetManager);
                }
                else
                {
                    harmonyInstance.UnpatchAll(harmonyInstance.Id);
                    Gradientctrl.UnloadAssetBundle();
                    Object.Destroy(ScriptManager);
                    //Object.Destroy(PresetManager);
                }
                flag = true;
            }
            return flag;
        }
        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmonyInstance.UnpatchAll(harmonyInstance.Id);
            Object.Destroy(ScriptManager);
            Object.Destroy(PresetManager);
            Logger.Log(nameof(Unload));
            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
