using UnityEngine;

namespace LessonFour
{
    public class PlayerCharacter : Character
    {
        [Range(0, 100)][SerializeField] private int health = 100;

        [Range(0.5f, 10.0f)][SerializeField] private float movingSpeed = 8.0f;
        [SerializeField] private float acceleration = 3.0f;
        private const float gravity = -9.8f;
        private CharacterController characterController;
        private MouseLook mouseLook;

        private Vector3 currentVelocity;
        protected override FireAction _fireAction { get; set; }


        protected override void Initiate()
        {
            base.Initiate();

            _fireAction = gameObject.AddComponent<RayShooter>();
            _fireAction.Reloading();

            characterController = GetComponentInChildren<CharacterController>();
            characterController ??= gameObject.AddComponent<CharacterController>();

            mouseLook = GetComponentInChildren<MouseLook>();
            mouseLook ??= gameObject.AddComponent<MouseLook>();
        }

        public override void Movement()
        {
            if (mouseLook != null && mouseLook.PlayerCamera != null)
            {
                mouseLook.PlayerCamera.enabled = hasAuthority;
            }

            if (hasAuthority)
            {
                float moveX = Input.GetAxis("Horizontal") * movingSpeed;
                float moveZ = Input.GetAxis("Vertical") * movingSpeed;

                var movement = new Vector3(moveX, 0, moveZ);
                movement = Vector3.ClampMagnitude(movement, movingSpeed);
                movement *= Time.deltaTime;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    movement *= acceleration;
                }

                movement.y = gravity;
                movement = transform.TransformDirection(movement);
                characterController.Move(movement);
                mouseLook.Rotation();
                CmdUpdatePosition(transform.position);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref currentVelocity,
                    movingSpeed * Time.deltaTime);
            }
        }

        private void Start()
        {
            Initiate();
        }

        private void OnGUI()
        {
            if (Camera.main == null)
            {
                return;
            }

            var info = $"Health: {health}\nClip: {_fireAction.BulletCount}";
            var size = 12;
            var bulletCountSize = 50;

            int posX = Camera.main.pixelWidth / 2 - size / 4;
            int posY = Camera.main.pixelHeight / 2 - size / 2;

            int posXBul = Camera.main.pixelWidth - bulletCountSize * 2;
            int posYBul = Camera.main.pixelHeight - bulletCountSize;

            GUI.Label(new Rect(posX, posY, size, size), "+");
            GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2, bulletCountSize * 2), info);
        }
    }
}