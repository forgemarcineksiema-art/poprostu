using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PrototypeVehicleController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float reverseAcceleration = 8f;
        [SerializeField] private float maxSpeed = 17f;
        [SerializeField] private float turnRate = 92f;
        [SerializeField] private float drag = 1.2f;
        [SerializeField] private float handbrakeDrag = 4.5f;

        private Rigidbody body;
        private PrototypePlayerController driver;

        public bool HasDriver => driver != null;
        public Transform CameraPivot => cameraPivot != null ? cameraPivot : transform;

        public void Enter(PrototypePlayerController player)
        {
            driver = player;
            PrototypeDebugState.Focus = "Vehicle";
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (cameraPivot == null)
            {
                cameraPivot = transform;
            }
        }

        private void Update()
        {
            if (driver == null)
            {
                return;
            }

            PrototypeDebugState.Mode = "Driving";
            PrototypeDebugState.Focus = "Vehicle";
            PrototypeDebugState.Speed = body.linearVelocity.magnitude;
            PrototypeDebugState.Interaction = "E / South Button: exit car";

            if (PrototypeInput.InteractPressedThisFrame)
            {
                ExitDriver();
            }
        }

        private void FixedUpdate()
        {
            if (driver == null)
            {
                ApplyDrag(drag);
                return;
            }

            var move = PrototypeInput.Move;
            var forwardInput = move.y;
            var steerInput = move.x;
            var force = forwardInput >= 0f ? acceleration : reverseAcceleration;

            if (body.linearVelocity.magnitude < maxSpeed || Vector3.Dot(body.linearVelocity, transform.forward * forwardInput) < 0f)
            {
                body.AddForce(transform.forward * (forwardInput * force), ForceMode.Acceleration);
            }

            var speedFactor = Mathf.InverseLerp(0.5f, maxSpeed, body.linearVelocity.magnitude);
            var steering = steerInput * turnRate * Mathf.Clamp01(speedFactor + 0.25f) * Time.fixedDeltaTime;
            body.MoveRotation(body.rotation * Quaternion.Euler(0f, steering, 0f));

            ApplyDrag(PrototypeInput.HandbrakeHeld ? handbrakeDrag : drag);
        }

        public void ExitDriver()
        {
            if (driver == null)
            {
                return;
            }

            var exit = exitPoint != null ? exitPoint : transform;
            var exitPosition = exit.position;
            var exitRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            var exitingDriver = driver;
            driver = null;
            exitingDriver.ExitVehicle(exitPosition, exitRotation);
        }

        private void ApplyDrag(float dragAmount)
        {
            var horizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            var dragForce = -horizontalVelocity * dragAmount;
            body.AddForce(dragForce, ForceMode.Acceleration);
        }
    }
}
