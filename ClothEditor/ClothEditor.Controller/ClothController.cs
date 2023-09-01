using UnityEngine;
using ModIO.UI;
using GameManagement;
using System;
using ReplayEditor;
using ClothEditor.Utils;

namespace ClothEditor.Controller
{
    public class ClothController : MonoBehaviour
    {
        //public static ClothController Instance { get; set; }

        private string LastTarget = "None";
        private string LastPlayerTarget = "None";
        public Cloth[] ClothComponentList;
        public SkinnedMeshRenderer[] PlayerGearList;
        public Cloth activeClothComponent;
        public SkinnedMeshRenderer activeGear;
        private Cloth activeGearCloth;
        private Transform MasterPrefab;
        public Transform Skater_ClothParent = PlayerController.Main.characterCustomizer.ClothingParent;
        public Transform ReplaySkater;
        Transform replay;
        //Transform Replay_LeftArm;
        //Transform Replay_RightArm;
        public Type state;

        public ReplayPlaybackController ReplayCtrl;
        

        public void Awake()
        {
            MasterPrefab = PlayerController.Main.transform.parent;

            if (MasterPrefab != null)
            {
                //Skater = GetSkater();
                ReplaySkater = GetReplaySkater();
            }
        }

        public void Update()
        {
            // Gets the current game state;
            var currentstate = GameStateMachine.Instance.CurrentState.GetType();
            if (currentstate != state)
            {
                Type laststate = state;
                if (laststate == typeof(ReplayState))
                {
                    ResetTargets();
                }

                state = currentstate;

                if (state == typeof(GearSelectionState) || state == typeof(ReplayState))
                {
                    ResetTargets();
                }
            }

            if (Main.settings.ClothUIToggle)
            {
                SetClothGrav();

                if (Main.settings.GetClothesToggle)
                {
                    UpdateActiveCloth(ClothComponentList);

                    if (activeClothComponent != null)
                    {
                        SetClothValues();
                        SetRandomAcc();
                    }
                }

                if (Main.settings.AddClothToggle)
                {
                    UpdateActiveGear(PlayerGearList);
                }

            }
        }

        public void FixedUpdate()
        {
            if (Main.settings.ArmupToggle)
            {
                ArmsUp();
            }

            
            if (Main.settings.GizmosToggle)
            {
                Main.Gradientctrl.SetGradientViewPos();
            }
            
        }

        public void ResetTargets()
        {
            Main.settings.resetToggles();
            ClothComponentList = null;
            PlayerGearList = null;
            AssetLoader.activeGradient.SetActive(false);
            LastTarget = "None";
            LastPlayerTarget = "None";
        }
        Transform GetSkaterClothParent()
        {
            Transform Skater = PlayerController.Main.characterCustomizer.ClothingParent;
            if (Skater != null)
            {
                Main.Logger.Log("Skater Found");
            }

            return Skater;
        }
      
        Transform GetReplaySkater()
        {
            replay = MasterPrefab.Find("ReplayEditor");
            Transform playback = replay.Find("Playback Skater Root");
            Transform NewSkater = playback.Find("NewSkater");
            Transform ReplaySkater = NewSkater.Find("Skater");

            if (ReplaySkater != null)
            {
                Main.Logger.Log("ReplaySkater Found");
            }

            return ReplaySkater;
        }

        /*
        Transform GetReplaySkater()
        {
            replay = MasterPrefab.Find("ReplayEditor");
            Transform playback = replay.Find("Playback Skater Root");
            ReplayCtrl = playback.GetComponent<ReplayPlaybackController>();
            Transform NewSkater = playback.Find("NewSkater");
            Transform ReplaySkater = NewSkater.Find("Skater");

            if (ReplaySkater != null)
            {
                Main.Logger.Log("ReplaySkater Found");
            }

            return ReplaySkater;
        }
        */

        /*
        Transform GetBone(string Bone)
        {
            Transform joints = MasterPrefab.Find("Skater_Joints");
            Transform Targetbone = joints.FindChildRecursively(Bone); //"Skater_pelvis"

            return Targetbone;
        }
        */

        // -------------------------- Cloth Settings -------------------------------

        public Cloth[] GetClothComponents(Transform skater)
        {
            Cloth[] clothList = skater.GetComponentsInChildren<Cloth>();

            if (clothList != null)
            {
                if (clothList.Length < 1)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $" 0 Cloth Components Found", 2.5f);
                }
                else
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{clothList.Length} Cloth Components Found", 2.5f);
                }
                foreach (Cloth cloth in clothList)
                {
                    Main.Logger.Log("Name: " + cloth.name);
                }

                return clothList;
            }
            return null;

        }
        public SkinnedMeshRenderer[] GetGear(Transform skater)
        {

            SkinnedMeshRenderer[] GearList = skater.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (GearList != null)
            {
                if (GearList.Length < 1)
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $" 0 Gear Found", 2.5f);
                }
                else
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{GearList.Length} Gear Found", 2.5f);
                }
                foreach (SkinnedMeshRenderer cloth in GearList)
                {
                    Main.Logger.Log("Name: " + cloth.name);
                }

                return GearList;
            }
            return null;
        }
        public string[] GetClothNames(Cloth[] clothList)
        {
            string[] names = new string[clothList.Length];
            int i = 0;
            foreach (Cloth cloth in clothList)
            {
                names[i] = cloth.name;
                i++;
            }
            return names;
        }

        public string[] GetGearNames(SkinnedMeshRenderer[] GearList)
        {
            string[] names = new string[GearList.Length];
            int i = 0;
            foreach (SkinnedMeshRenderer cloth in GearList)
            {
                names[i] = cloth.name;
                i++;
            }
            return names;
        }

        public void SetDefaultValues(Cloth cloth)
        {
            cloth.damping = Main.settings.DefaultDamping;
            cloth.clothSolverFrequency = Main.settings.DefaultSolverFreq;
            cloth.friction = Main.settings.DefaultFriction;
            cloth.bendingStiffness = Main.settings.DefaultBendingStiff;
            cloth.sleepThreshold = Main.settings.DefaultSleepThreshold;
            cloth.stiffnessFrequency = Main.settings.DefaultStiffnessFreq;
            cloth.stretchingStiffness = Main.settings.DefaultStretchingStiff;
            cloth.worldAccelerationScale = Main.settings.DefaultWorldAcc;
            cloth.worldVelocityScale = Main.settings.DefaultWorldVel;
            cloth.randomAcceleration = new Vector3(0, 0, 0);
        }

        public void SetMainSettings(Cloth cloth)
        {
            Main.settings.DampingFlt = cloth.damping;
            Main.settings.SolverFreqFlt = cloth.clothSolverFrequency;
            Main.settings.FrictionFlt = cloth.friction;
            Main.settings.BendingStiffFlt = cloth.bendingStiffness;
            Main.settings.SleepThresholdFlt = cloth.sleepThreshold;
            Main.settings.StiffnessFreqFlt = cloth.stiffnessFrequency;
            Main.settings.StretchingStiffFlt = cloth.stretchingStiffness;
            Main.settings.WorldAccFlt = cloth.worldAccelerationScale;
            Main.settings.WorldVelFlt = cloth.worldVelocityScale;
        }

        void SetClothGrav()
        {
            // Gravity for all cloths
            Vector3 newGrav = new Vector3(Main.settings.ClothGravity_x, Main.settings.ClothGravity_y, Main.settings.ClothGravity_z);

            if (Physics.clothGravity != newGrav)
            {
                Physics.clothGravity = newGrav;
            }
        }

        void SetRandomAcc()
        {
            Vector3 newAcc = new Vector3(Main.settings.Clothacc_x, Main.settings.Clothacc_y, Main.settings.Clothacc_z);

            if (activeClothComponent.randomAcceleration != newAcc)
            {
                activeClothComponent.randomAcceleration = newAcc;
            }
        }

        public void UpdateActiveCloth(Cloth[] clothList)
        {
            if (Main.settings.cloth_target != "None" && clothList != null)
            {
                if (LastTarget != Main.settings.cloth_target)
                {
                    foreach (Cloth cloth in clothList)
                    {
                        if (cloth.name.Equals(Main.settings.cloth_target))
                        {
                            activeClothComponent = cloth;
                            SetMainSettings(activeClothComponent);
                            //Main.settings.Buffer = activeClothComponent.coefficients.Length;
                        }
                    }
                    LastTarget = Main.settings.cloth_target;
                }
            }
            else
            {
                LastTarget = "None";
            }
        }

        public void UpdateActiveGear(SkinnedMeshRenderer[] GearList)
        {
            if (Main.settings.PlayerCloth_target != "None" && GearList != null)
            {
                if (LastPlayerTarget != Main.settings.PlayerCloth_target)
                {
                    foreach (SkinnedMeshRenderer cloth in GearList)
                    {
                        if (cloth.name.Equals(Main.settings.PlayerCloth_target))
                        {
                            activeGear = cloth;
                        }
                    }

                    LastPlayerTarget = Main.settings.PlayerCloth_target;
                }
            }
            else
            {
                LastPlayerTarget = "None";
            }
        }

        public void AddClothComponent()
        {
            if (activeGear != null && Main.settings.PlayerCloth_target != "None")
            {
                activeGear.gameObject.AddComponent<Cloth>();
                activeGearCloth = activeGear.gameObject.GetComponent<Cloth>();
                if (activeGearCloth != null)
                {
                    //Main.settings.Buffer = activeGearCloth.coefficients.Length;
                    //Main.settings.DefaultBuffer = activeGearCloth.coefficients.Length;

                    SetDefaultValues(activeGearCloth);
                    SetConstraints(activeGearCloth, 2);

                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $" Cloth Added - " + Main.settings.PlayerCloth_target, 2.5f);
                }
                else
                {
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $" Adding Cloth Failed", 2.5f);
                }
            }
        }
        public void RemoveClothComponent()
        {
            if (activeClothComponent != null && Main.settings.cloth_target != "None")
            {
                SetConstraints(activeClothComponent, 3);

                //Destroy(activeGear.gameObject.GetComponent<Cloth>());
                //Destroy(activeGearCloth);
                Destroy(activeClothComponent);
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $" Cloth Removed - " + activeClothComponent.name, 2.5f);

                Main.settings.cloth_target = "None";
                Main.settings.PlayerCloth_target = "None";
                activeClothComponent = null;
                ClothComponentList = null;
                activeGear = null;
                //activeGearCloth = null;
            }
        }

        public void SetClothValues()
        {
            if (activeClothComponent.damping != Main.settings.DampingFlt)
            {
                activeClothComponent.damping = Main.settings.DampingFlt;
            }
            if (activeClothComponent.clothSolverFrequency != Main.settings.SolverFreqFlt)
            {
                activeClothComponent.clothSolverFrequency = Main.settings.SolverFreqFlt;
            }
            if (activeClothComponent.friction != Main.settings.FrictionFlt)
            {
                activeClothComponent.friction = Main.settings.FrictionFlt;
            }
            if (activeClothComponent.bendingStiffness != Main.settings.BendingStiffFlt)
            {
                activeClothComponent.bendingStiffness = Main.settings.BendingStiffFlt;
            }
            if (activeClothComponent.sleepThreshold != Main.settings.SleepThresholdFlt)
            {
                activeClothComponent.sleepThreshold = Main.settings.SleepThresholdFlt;
            }
            if (activeClothComponent.stiffnessFrequency != Main.settings.StiffnessFreqFlt)
            {
                activeClothComponent.stiffnessFrequency = Main.settings.StiffnessFreqFlt;
            }
            if (activeClothComponent.stretchingStiffness != Main.settings.StretchingStiffFlt)
            {
                activeClothComponent.stretchingStiffness = Main.settings.StretchingStiffFlt;
            }
            if (activeClothComponent.worldAccelerationScale != Main.settings.WorldAccFlt)
            {
                activeClothComponent.worldAccelerationScale = Main.settings.WorldAccFlt;
            }
            if (activeClothComponent.worldVelocityScale != Main.settings.WorldVelFlt)
            {
                activeClothComponent.worldVelocityScale = Main.settings.WorldVelFlt;
            }
        }

        /*
        public void SetConstraints(Cloth activeCloth)
        {
            if (activeCloth != null)
            {
                ClothSkinningCoefficient[] newConstraints = new ClothSkinningCoefficient[activeCloth.coefficients.Length];
                for (int i = 0; i < newConstraints.Length; i++)
                {
                    newConstraints[i].maxDistance = Main.settings.ClothMaxDistance / 100;
                    newConstraints[i].collisionSphereDistance = Main.settings.ClothSphereDistance / 100;
                }
                activeCloth.coefficients = newConstraints;
            }        
        }
        */

        
        public void SetConstraints(Cloth activeCloth, int options)
        {
            float gradient = Main.settings.GradientHeight / 100;

            if (activeCloth != null)
            {
                ClothSkinningCoefficient[] newConstraints = new ClothSkinningCoefficient[activeCloth.coefficients.Length];
                for (int i = 0; i < newConstraints.Length; i++)
                {
                    switch (options)
                    {
                        case 1: // Set Constraints
                            if (activeCloth.vertices[i].y <= gradient)
                            {
                                newConstraints[i].maxDistance = Main.settings.ClothMaxDistance / 100;
                                newConstraints[i].collisionSphereDistance = Main.settings.ClothSphereDistance / 100;
                            }
                            break;

                        case 2: // add Cloth
                            newConstraints[i].maxDistance = 0.01f;
                            newConstraints[i].collisionSphereDistance = 0.0f;
                            break;

                        case 3: // Remove Cloth
                            newConstraints[i].maxDistance = 0.0f;
                            newConstraints[i].collisionSphereDistance = 0.0f;
                            break;
                    }
                }
                activeCloth.coefficients = newConstraints;
            }
        }

        /* old constraint funtion, above soloution is better.
         * 
        public Vector3 min, max;
        public void SetConstraints(Cloth activeCloth, int options)
        {
            //float gradient = Main.settings.GradientHeight / 100;

            if (activeCloth != null)
            {
                min = new Vector3(Main.settings.GradientMin_x, Main.settings.GradientMin_y, Main.settings.GradientMin_z);
                max = new Vector3(Main.settings.GradientMax_x, Main.settings.GradientMax_y, Main.settings.GradientMax_z);

                ClothSkinningCoefficient[] newConstraints = new ClothSkinningCoefficient[activeCloth.coefficients.Length];
                for (int i = 0; i < newConstraints.Length; i++)
                {
                    switch (options)
                    {
                        case 1: // Set Constraints
                            if (activeCloth.vertices[i].y >= min.y && activeCloth.vertices[i].y <= max.y
                                && activeCloth.vertices[i].x >= min.y && activeCloth.vertices[i].y <= max.x
                                && activeCloth.vertices[i].z >= min.z && activeCloth.vertices[i].z <= max.z)
                            {
                                newConstraints[i].maxDistance = Main.settings.ClothMaxDistance / 100;
                                newConstraints[i].collisionSphereDistance = Main.settings.ClothSphereDistance / 100;
                            }
                            break;

                        case 2: // add Cloth
                            newConstraints[i].maxDistance = 0.01f;
                            newConstraints[i].collisionSphereDistance = 0.0f;
                            break;

                        case 3: // Remove Cloth
                            newConstraints[i].maxDistance = 0.0f;
                            newConstraints[i].collisionSphereDistance = 0.0f;
                            break;
                    }
                }
                activeCloth.coefficients = newConstraints;
            }
        }
        */

        public void ArmsUp()
        {
            // checks if the player is on the ground and not moving before lifing arms.
            if (PlayerController.Main.gameplay.playerSM.playerData.IsOnGroundState() && PlayerController.Main.gameplay.skaterController.skaterRigidbody.velocity.sqrMagnitude < 0.0001f)
            {
                LiftArms();
            }
        }

        void LiftArms()
        {
            var leftArm = PlayerController.Main.gameplay.ragdollController.puppetMaster.muscles[4];
            var rightArm = PlayerController.Main.gameplay.ragdollController.puppetMaster.muscles[7];

            if (leftArm.transform.localRotation.x != -Mathf.Abs(Main.settings.ArmHeight))
            {
                leftArm.transform.localRotation = new Quaternion(-Mathf.Abs(Main.settings.ArmHeight), leftArm.transform.localRotation.y, leftArm.transform.localRotation.z, 0);
            }

            if (rightArm.transform.localRotation.x != Main.settings.ArmHeight)
            {
                rightArm.transform.localRotation = new Quaternion(Main.settings.ArmHeight, rightArm.transform.localRotation.y, rightArm.transform.localRotation.z, 0);
            }
        }
    }
}