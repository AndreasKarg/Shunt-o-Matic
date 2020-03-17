using System;
using UnityEngine;

namespace ShunterPhysics
{
    public class ShunterLocoSimulation
    {
        private const float MaxTractiveEffort = 120.0e3f; // Newtons
        private const float MaxPower = 450.0e3f; // Watts

        private const float IdleEngineAngularSpeed = (float)(350.0 * (2.0 * Math.PI / 60.0)); // rpm with conversion to rad/sec in brackets
        private const float MaxEngineAngularSpeed = (float)(850.0 * (2.0 * Math.PI / 60.0)); // rpm with conversion to rad/sec in brackets

        public float Tick(float throttle, float speed)
        {
            var maxTractiveEffortAtCurrentSpeedAndMaxPower = MaxPower / speed;
            if (float.IsNaN(maxTractiveEffortAtCurrentSpeedAndMaxPower))
                maxTractiveEffortAtCurrentSpeedAndMaxPower = float.MaxValue;

            var actualMaxTractiveEffort =
                Mathf.Clamp(maxTractiveEffortAtCurrentSpeedAndMaxPower, 0.0f, MaxTractiveEffort);

            var actualTractiveEffort = throttle * actualMaxTractiveEffort;

            return actualTractiveEffort;
        }
    }
}
