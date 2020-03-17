using System.Security.AccessControl;

namespace ShunterPhysics
{
    public class TrainSimulation
    {
        private const float TrainMass = 200.0e3f; // 200 metric tons

        public float Speed { get; private set; } = 0.0f;
        public float TractiveEffort { get; private set; } = 0.0f;
        public float Acceleration { get; private set; } = 0.0f;

        private ShunterLocoSimulation _shunterLocoSimulation;

        public TrainSimulation(ShunterLocoSimulation shunterLocoSimulation)
        {
            _shunterLocoSimulation = shunterLocoSimulation;
        }

        public void Tick(float throttle, float deltaT)
        {
            TractiveEffort = _shunterLocoSimulation.Tick(throttle, Speed, deltaT);

            Acceleration = TractiveEffort / TrainMass;

            Speed += Acceleration * deltaT;
        }

        public void Reset()
        {
            Speed = 0.0f;
            TractiveEffort = 0.0f;
            Acceleration = 0.0f;

            _shunterLocoSimulation.Reset();
        }
    }
}