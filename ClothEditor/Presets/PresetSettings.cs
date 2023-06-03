using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ClothEditor
{
    [Serializable]
    public class PresetSettings : MonoBehaviour
    {
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
        public float ClothMaxDistance;
        [SerializeField]
        public float ClothSphereDistance;
        [SerializeField]
        public float GradientHeight;
    }
}
