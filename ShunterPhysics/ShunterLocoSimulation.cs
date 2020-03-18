using System;
using UnityEngine;
using UnityEngine.VR;

namespace ShunterPhysics
{
    public class ShunterLocoSimulation
    {
        private const float MaxTractiveEffort = 120.0e3f; // Newtons
        private const float MaxPower = 450.0e3f; // Watts

        public const float IdleEngineRPM = 350.0f; // rpm
        public const float MaxEngineRPM = 850.0f; // rpm
        public const float EngineRPMChangePerSecond = 100.0f; // rpm/s

        private const float EngineSpeedChangePerSecondAsFractionOfRpmRange = EngineRPMChangePerSecond / (MaxEngineRPM - IdleEngineRPM);

        public float EngineAngularSpeedAsFractionOfRange { get; private set; } = 0.0f; // offset by IdleEngineRPM for display
        public float EngineActualRpm => EngineAngularSpeedAsFractionOfRange * (MaxEngineRPM - IdleEngineRPM) + IdleEngineRPM;

        public float Tick(float throttle, float speed, float deltaT)
        {
            UpdateEngineSpeed(throttle, deltaT);
            return CalculateTractiveEffort(speed);
        }

        private void UpdateEngineSpeed(float throttle, float deltaT)
        {
            var engineSpeedError = throttle - EngineAngularSpeedAsFractionOfRange;
            var possibleEngineSpeedChangeThisTick =
                deltaT * EngineSpeedChangePerSecondAsFractionOfRpmRange;

            var actualEngineSpeedChange = Math.Min(Math.Abs(engineSpeedError), possibleEngineSpeedChangeThisTick);

            var newEngineSpeedUnclamped = EngineAngularSpeedAsFractionOfRange + Math.Sign(engineSpeedError)* actualEngineSpeedChange;
            EngineAngularSpeedAsFractionOfRange = Mathf.Clamp01(newEngineSpeedUnclamped);
        }

        private float CalculateTractiveEffort(float speed)
        {
            var maxTractiveEffortAtCurrentSpeedAndMaxPower = MaxPower / speed;
            if (float.IsNaN(maxTractiveEffortAtCurrentSpeedAndMaxPower))
                maxTractiveEffortAtCurrentSpeedAndMaxPower = float.MaxValue;

            var actualMaxTractiveEffort =
                Mathf.Clamp(maxTractiveEffortAtCurrentSpeedAndMaxPower, 0.0f, MaxTractiveEffort);

            var actualTractiveEffort = EngineAngularSpeedAsFractionOfRange * actualMaxTractiveEffort;

            return actualTractiveEffort;
        }

        public void Reset()
        {
            EngineAngularSpeedAsFractionOfRange = 0.0f;
        }
    }
}
