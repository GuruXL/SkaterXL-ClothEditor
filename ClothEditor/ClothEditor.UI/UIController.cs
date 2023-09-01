using UnityEngine;
using RapidGUI;
using GameManagement;
using ClothEditor.Utils;

namespace ClothEditor.UI
{
    public class UItab // UI dropdown tabs class
    {
        public bool reference;
        public string text;
        public int font;

        public UItab(bool reference, string text, int font)
        {
            this.reference = reference;
            this.text = text;
            this.font = font;
        }
    }

    public class UIController : MonoBehaviour
    {
        private Rect WindowBox = new Rect(20, 20, Screen.width / 8, 20);
        bool showMainMenu = false;

        public static UIController Instance { get; private set; }

        public UItab Cloth_Tab = new UItab(true, "Cloth Editor", 14);
        public UItab MainSettings_Tab = new UItab(true, "Main Settings", 14);
        public UItab Gravity_Tab = new UItab(true, "Gravity", 14);
        public UItab RandomAcc_Tab = new UItab(true, "Random Acceleration", 14);
        public UItab Constraints_Tab = new UItab(true, "Constraint Settings", 14);
        public UItab Presets_Tab = new UItab(true, "Presets", 14);

        public string white = "#e6ebe8";
        public string Grey = "#adadad";
        public string LightBlue = "#30e2e6";
        public string DarkBlue = "#3347ff";
        public string green = "#7CFC00";
        public string red = "#b71540";
        public string TabColor;

        public void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(Main.settings.Hotkey.keyCode))
            {
                if (showMainMenu)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }

            if (showMainMenu)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Open()
        {
            showMainMenu = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Close()
        {
            showMainMenu = false;
            Cursor.visible = false;
            Main.settings.Save(Main.modEntry);
        }

        public void OnGUI()
        {
            if (!showMainMenu)
                return;
            GUI.backgroundColor = Main.settings.BGColor;
            WindowBox = GUILayout.Window(744, WindowBox, MainWindow, "<b> Cloth Editor by Guru </b>");

        }
        // Creates the GUI window
        public void MainWindow(int windowID)
        {
            GUI.backgroundColor = Main.settings.BGColor;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            MainUI();
            if (!Main.settings.enabled)
                return;

            ClothUI();

        }

        void MainUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.settings.enabled ? "<b><color=#7CFC00> Enabled </color></b>" : "<b><color=#b71540> Disabled </color></b>");
            if (RGUI.Button(Main.settings.enabled, ""))
            {
                Main.settings.enabled = !Main.settings.enabled;
            }
            GUILayout.EndHorizontal();

            // Resets Toggles and Active prefabs
            if (!Main.settings.enabled)
            {
                Main.Clothctrl.ResetTargets();
            }

        }
        public void Tabs(UItab obj, string color = "#e6ebe8")
        {
            if (GUILayout.Button($"<size={obj.font}><color={color}>" + (obj.reference ? "-" : "<b>+</b>") + obj.text + "</color>" + "</size>", "Label"))
            {
                obj.reference = !obj.reference;
                WindowBox.height = 20;
                WindowBox.width = Screen.width / 8;

            }
        }
        void TextSwitch(UItab Tab)
        {
            switch (Tab.reference)
            {
                case true:
                    TabColor = Grey;
                    break;

                case false:
                    TabColor = LightBlue;
                    break;
            }
        }

        public void ClothUI()
        {
            TextSwitch(Cloth_Tab);
            Tabs(Cloth_Tab, TabColor);
            if (!Cloth_Tab.reference)
            {
                GUILayout.BeginVertical("Box");
                /*
                if (GUILayout.Button("Apply Mask", RGUIStyle.button, GUILayout.Width(128)))
                {
                    PlayerController.Instance.characterCustomizer.ApplyMasksTo(Main.Clothctrl.Skater.gameObject);
                }
                */
                if (RGUI.Button(Main.settings.ClothUIToggle, "Cloth Settings"))
                {
                    Main.settings.ClothUIToggle = !Main.settings.ClothUIToggle;
                }
                if (Main.settings.ClothUIToggle)
                {
                    TextSwitch(Gravity_Tab);
                    Tabs(Gravity_Tab, TabColor);
                    if (!Gravity_Tab.reference)
                    {
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("Cloth Gravity");
                        Main.settings.ClothGravity_x = RGUI.SliderFloat(Main.settings.ClothGravity_x, -40.0f, 40.0f, 0.0f, " cloth x");
                        Main.settings.ClothGravity_y = RGUI.SliderFloat(Main.settings.ClothGravity_y, -40.0f, 40.0f, -9.81f, " cloth y");
                        Main.settings.ClothGravity_z = RGUI.SliderFloat(Main.settings.ClothGravity_z, -40.0f, 40.0f, 0.0f, " cloth z");
                        GUILayout.EndVertical();
                    }              

                    GUILayout.BeginVertical("Box");
                    if (RGUI.Button(Main.settings.AddClothToggle, "Add Cloth"))
                    {
                        Main.settings.AddClothToggle = !Main.settings.AddClothToggle;

                        if (!Main.settings.AddClothToggle)
                        {
                            Main.settings.PlayerCloth_target = "None";
                        }
                    }
                    GUILayout.EndVertical();

                    if (Main.settings.AddClothToggle)
                    {
                        GUILayout.BeginVertical("Box");
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Scan For Gear", RGUIStyle.button, GUILayout.Width(128)))
                        {
                            if (Main.Clothctrl.state != typeof(ReplayState))
                            {
                                Main.Clothctrl.PlayerGearList = Main.Clothctrl.GetGear(Main.Clothctrl.Skater);
                            }
                            else
                            {
                                Main.Clothctrl.PlayerGearList = Main.Clothctrl.GetGear(Main.Clothctrl.ReplaySkater);
                            }
                        }

                        if (Main.Clothctrl.PlayerGearList != null)
                        {
                            GUILayout.BeginHorizontal();
                            Main.settings.PlayerCloth_target = RGUI.SelectionPopup(Main.settings.PlayerCloth_target, Main.Clothctrl.GetGearNames(Main.Clothctrl.PlayerGearList));
                            GUILayout.Space(4);
                            GUI.backgroundColor = Color.red;
                            if (GUILayout.Button("Add Cloth", RGUIStyle.button, GUILayout.Width(84)))
                            {
                                Main.Clothctrl.AddClothComponent();
                            }
                            //GUILayout.Space(4);
                            //Main.settings.BoneTarget = RGUI.Field(Main.settings.BoneTarget, "");
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
             
                    GUILayout.BeginVertical("Box");
                    if (RGUI.Button(Main.settings.GetClothesToggle, "Cloth Editor"))
                    {
                        Main.settings.GetClothesToggle = !Main.settings.GetClothesToggle;

                        if (!Main.settings.GetClothesToggle)
                        {
                            Main.settings.cloth_target = "None";
                        }
                    }
                    GUILayout.EndVertical();
                    if (Main.settings.GetClothesToggle)
                    {
                        GUILayout.BeginVertical("Box");
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Scan For Cloth", RGUIStyle.button, GUILayout.Width(128)))
                        {
                            if (Main.Clothctrl.state != typeof(ReplayState))
                            {
                                Main.Clothctrl.ClothComponentList = Main.Clothctrl.GetClothComponents(Main.Clothctrl.Skater);
                            }
                            else
                            {
                                Main.Clothctrl.ClothComponentList = Main.Clothctrl.GetClothComponents(Main.Clothctrl.ReplaySkater);
                            }

                        }
                        if (Main.Clothctrl.ClothComponentList != null)
                        {
                            GUILayout.BeginHorizontal();
                            Main.settings.cloth_target = RGUI.SelectionPopup(Main.settings.cloth_target, Main.Clothctrl.GetClothNames(Main.Clothctrl.ClothComponentList));
                            GUILayout.Space(4);
                            GUI.backgroundColor = Color.red;
                            if (GUILayout.Button("Remove Cloth", RGUIStyle.button, GUILayout.Width(100)))
                            {
                                Main.Clothctrl.RemoveClothComponent();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(4);
                            if (Main.Clothctrl.activeClothComponent != null)
                            {
                                TextSwitch(MainSettings_Tab);
                                Tabs(MainSettings_Tab, TabColor);
                                if (!MainSettings_Tab.reference)
                                {
                                    Main.settings.DampingFlt = RGUI.SliderFloat(Main.settings.DampingFlt, 0.0f, 1.0f, Main.settings.DefaultDamping, " Damping");
                                    Main.settings.SolverFreqFlt = Mathf.RoundToInt(RGUI.SliderFloat(Main.settings.SolverFreqFlt, 0, 500, Main.settings.DefaultSolverFreq, " Solver Frequency"));
                                    Main.settings.FrictionFlt = RGUI.SliderFloat(Main.settings.FrictionFlt, 0, 1, Main.settings.DefaultFriction, " Friction");
                                    Main.settings.BendingStiffFlt = RGUI.SliderFloat(Main.settings.BendingStiffFlt, 0.0f, 1.0f, Main.settings.DefaultBendingStiff, " Bending Stiffness");
                                    Main.settings.SleepThresholdFlt = RGUI.SliderFloat(Main.settings.SleepThresholdFlt, 0.0f, 10.0f, Main.settings.DefaultSleepThreshold, " Sleep Threshold");
                                    Main.settings.StiffnessFreqFlt = RGUI.SliderFloat(Main.settings.StiffnessFreqFlt, 0.0f, 50.0f, Main.settings.DefaultStiffnessFreq, " Stiffness Frequency");
                                    Main.settings.StretchingStiffFlt = RGUI.SliderFloat(Main.settings.StretchingStiffFlt, 0.0f, 1.0f, Main.settings.DefaultStretchingStiff, " Stretching Stiffness");
                                    Main.settings.WorldAccFlt = RGUI.SliderFloat(Main.settings.WorldAccFlt, 0.0f, 10.0f, Main.settings.DefaultWorldAcc, " World Acceleration");
                                    Main.settings.WorldVelFlt = RGUI.SliderFloat(Main.settings.WorldVelFlt, 0.0f, 10.0f, Main.settings.DefaultWorldVel, " World Velocity");
                                }

                                TextSwitch(Constraints_Tab);
                                Tabs(Constraints_Tab, TabColor);
                                if (!Constraints_Tab.reference)
                                {
                                    GUILayout.BeginVertical("Box");
                                    GUILayout.Label("Constraints");
                                    Main.settings.ClothMaxDistance = RGUI.SliderFloat(Main.settings.ClothMaxDistance, 0.0f, 10.0f, 1.0f, "Max Distance");
                                    Main.settings.ClothSphereDistance = RGUI.SliderFloat(Main.settings.ClothSphereDistance, 0.0f, 10.0f, 0.10f, "Sphere Distance");
                                    //Main.settings.Buffer = Mathf.RoundToInt(RGUI.SliderFloat(Main.settings.Buffer, 0, Main.settings.DefaultBuffer, Main.settings.DefaultBuffer, "Buffer"));
                                    Main.settings.GradientHeight = RGUI.SliderFloat(Main.settings.GradientHeight, -100.0f, 100.0f, 0.0f, "GradientHeight");

                                    /*
                                    Main.settings.GradientMin_x = RGUI.SliderFloat(Main.settings.GradientMin_x, -1.0f, 1.0f, 0.0f, "GradientMin_x");
                                    Main.settings.GradientMax_x = RGUI.SliderFloat(Main.settings.GradientMax_x, -1.0f, 1.0f, 0.0f, "GradientMax_x");
                                    GUILayout.Space(4);
                                    Main.settings.GradientMin_y = RGUI.SliderFloat(Main.settings.GradientMin_y, -1.0f, 1.0f, 0.0f, "GradientMin_y");
                                    Main.settings.GradientMax_y = RGUI.SliderFloat(Main.settings.GradientMax_y, -1.0f, 1.0f, 0.0f, "GradientMax_y");
                                    GUILayout.Space(4);
                                    Main.settings.GradientMin_z = RGUI.SliderFloat(Main.settings.GradientMin_z, -1.0f, 1.0f, 0.0f, "GradientMin_z");
                                    Main.settings.GradientMax_z = RGUI.SliderFloat(Main.settings.GradientMax_z, -1.0f, 1.0f, 0.0f, "GradientMax_z");
                                    */
                                    
                                    GUILayout.BeginVertical("Box");
                                    if (RGUI.Button(Main.settings.GizmosToggle, "Draw Gradient Visuals"))
                                    {
                                        Main.settings.GizmosToggle = !Main.settings.GizmosToggle;

                                        if (Main.settings.GizmosToggle)
                                        {
                                            AssetLoader.activeGradient.SetActive(true);
                                        }
                                        if (!Main.settings.GizmosToggle)
                                        {
                                            AssetLoader.activeGradient.SetActive(false);
                                        }
                                    }
                                    GUILayout.EndVertical();
                                    
                                    // lift arms button
                                    GUILayout.BeginVertical("Box");
                                    if (RGUI.Button(Main.settings.ArmupToggle, "Lift Arms"))
                                    {
                                        Main.settings.ArmupToggle = !Main.settings.ArmupToggle;

                                        if (!Main.settings.ArmupToggle)
                                        {
                                            PlayerController.Main.gameplay.ragdollController.puppetMaster.muscles[6].Reset();
                                            PlayerController.Main.gameplay.ragdollController.puppetMaster.muscles[9].Reset();
                                        }
                                    }
                                    GUILayout.EndVertical();
                                    if (Main.settings.ArmupToggle)
                                    {
                                        GUILayout.BeginVertical("Box");
                                        Main.settings.ArmHeight = RGUI.SliderFloat(Main.settings.ArmHeight, 0.0f, 2.0f, 1.0f, "Arm Height");
                                        GUILayout.EndVertical();
                                    }
                                    GUILayout.Space(6);
                                    GUI.backgroundColor = Color.red;
                                    if (GUILayout.Button("Apply Constraints", RGUIStyle.button, GUILayout.Width(128)))
                                    {
                                        Main.Clothctrl.SetConstraints(Main.Clothctrl.activeClothComponent, 1);
                                        //Main.Clothctrl.SetConstraintsWithBuffer(Main.Clothctrl.activeClothComponent);
                                    }
                                    GUILayout.EndVertical();
                                }

                                TextSwitch(RandomAcc_Tab);
                                Tabs(RandomAcc_Tab, TabColor);
                                if (!RandomAcc_Tab.reference)
                                {
                                    GUILayout.BeginVertical("Box");
                                    GUILayout.Label("Random Acceleration");
                                    Main.settings.Clothacc_x = RGUI.SliderFloat(Main.settings.Clothacc_x, -40.0f, 40.0f, 0.0f, " x");
                                    Main.settings.Clothacc_y = RGUI.SliderFloat(Main.settings.Clothacc_y, -40.0f, 40.0f, 0.0f, " y");
                                    Main.settings.Clothacc_z = RGUI.SliderFloat(Main.settings.Clothacc_z, -40.0f, 40.0f, 0.0f, " z");
                                    GUILayout.EndVertical();
                                }

                                TextSwitch(Presets_Tab);
                                Tabs(Presets_Tab, TabColor);
                                if (!Presets_Tab.reference)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUI.backgroundColor = Color.cyan;
                                    if (GUILayout.Button("Save Preset", RGUIStyle.button, GUILayout.Width(98)))
                                    {
                                        Main.PresetCtrl.SavePreset();
                                    }
                                    GUI.backgroundColor = Color.cyan;
                                    Main.PresetCtrl.PresetName = RGUI.Field(Main.PresetCtrl.PresetName, "");
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUI.backgroundColor = Color.red;
                                    if (GUILayout.Button("Apply Preset", RGUIStyle.button, GUILayout.Width(98)))
                                    {
                                        Main.PresetCtrl.ApplyPreset();
                                        Main.Clothctrl.SetConstraints(Main.Clothctrl.activeClothComponent, 1);
                                        Main.PresetCtrl.ResetPresetList();
                                    }
                                    Main.PresetCtrl.PresetToLoad = RGUI.SelectionPopup(Main.PresetCtrl.PresetToLoad, Main.PresetCtrl.GetPresetNames());
                                    GUILayout.EndHorizontal();
                                }

                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();

            }
        }

    }
}
