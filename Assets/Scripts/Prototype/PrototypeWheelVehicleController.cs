using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PrototypeWheelVehicleController : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] driveWheels;
        [SerializeField] private WheelCollider[] steerWheels;
        [SerializeField] private float motorTorque = 1450f;
        [SerializeField] private float brakeTorque = 2200f;
        [SerializeField] private float reverseTorque = 650f;
        [SerializeField] private float maxSteerAngle = 28f;
        [SerializeField] private float reverseSpeedThreshold = 0.35f;

        private Rigidbody body;

        public static PrototypeDriveIntent ResolveDriveIntent(float forwardInput, float forwardSpeed, float reverseThreshold)
        {
            return PrototypeVehicleController.ResolveDriveIntent(forwardInput, forwardSpeed, reverseThreshold);
        }

        public void ConfigureForTests(WheelCollider[] nextDriveWheels, WheelCollider[] nextSteerWheels)
        {
            driveWheels = nextDriveWheels;
            steerWheels = nextSteerWheels;
        }

        public void ApplyDriveInput(Vector2 move)
        {
            EnsureInitialized();
            var forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);
            var intent = ResolveDriveIntent(move.y, forwardSpeed, reverseSpeedThreshold);
            var torque = intent.Throttle * motorTorque - intent.Reverse * reverseTorque;
            var brake = intent.Brake * brakeTorque;

            foreach (var wheel in driveWheels)
            {
                if (wheel == null)
                {
                    continue;
                }

                wheel.motorTorque = torque;
                wheel.brakeTorque = brake;
            }

            foreach (var wheel in steerWheels)
            {
                if (wheel != null)
                {
                    wheel.steerAngle = move.x * maxSteerAngle;
                }
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody>();
                body.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}
