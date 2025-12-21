using UnityEngine;

/*--------------------------------------------------------------------*\
|* Metodos / Funciones Propias
|*
|* Este codigo sale de  Invector.vCharacterController
\*--------------------------------------------------------------------*/
namespace TauriLand.Tps.Character
{
    public class cThirdPersonMotor : MonoBehaviour
    {
        #region Inspector Variables

        [Header("- Movement")]

        [Tooltip("Desactivelo si tiene animaciones \"en el lugar\" y use estos valores anteriores para mover el personaje, o uselo con movimiento de raiz como velocidad adicional")]
        public bool useRootMotion = false;
        [Tooltip("Use esto para rotar el personaje usando el eje del mundo, o falso para usar el eje de la camara - VERIFICAR para camara isometrica")]
        public bool rotateByWorld = false;
        [Tooltip("Marque esta opcion para usar el sprint al presionar el boton para que su personaje corra hasta que se acabe la resistencia o se detenga el movimiento.\n Si desmarca esta opcion, su personaje correra mientras se presione el botón SprintInput o se acabe la resistencia.")]
        public bool useContinuousSprint = true;
        [Tooltip("Marca esto para correr siempre con libertad de movimiento.")]
        public bool sprintOnlyFree = true;
        public enum LocomotionType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree,
        }
        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;

        public vMovementSpeed freeSpeed, strafeSpeed;

        [Header("- Aerotransportado")]

        [Tooltip("Utilice la velocidad actual del cuerpo rigido para influir en la distancia del salto")]
        public bool jumpWithRigidbodyForce = false;
        [Tooltip("Girar/rotar o no mientras se esta en el aire")]
        public bool jumpAndRotate = true;
        [Tooltip("Cuanto tiempo estara saltando el personaje?")]
        public float jumpTimer = 0.3f;
        [Tooltip("Agregue altura de salto adicional, si desea saltar solo con Root Motion, deje el valor en 0.")]
        public float jumpHeight = 4f;

        [Tooltip("Velocidad con la que se movera el personaje mientras este en el aire")]
        public float airSpeed = 5f;
        [Tooltip("Suavidad de la direccion en el aire")]
        public float airSmooth = 6f;
        [Tooltip("Aplicar gravedad adicional cuando el personaje no este en tierra.")]
        public float extraGravity = -10f;
        [HideInInspector]
        public float limitFallVelocity = -15f;

        [Header("- Suelo")]
        [Tooltip("Capas sobre las que el personaje puede caminar")]
        public LayerMask groundLayer = 1 << 0;
        [Tooltip("La distancia hasta llegar a no estar conectada a tierra")]
        public float groundMinDistance = 0.25f;
        public float groundMaxDistance = 0.5f;
        [Tooltip("Angulo maximo para caminar")]
        [Range(30, 80)] public float slopeLimit = 75f;
        #endregion


        #region Components

        internal Animator animator;
        internal Rigidbody _rigidbody;                                                      // access the Rigidbody component
        internal PhysicsMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;         // create PhysicMaterial for the Rigidbody
        internal CapsuleCollider _capsuleCollider;                                          // access CapsuleCollider information

        #endregion


        #region Internal Variables

        // movement bools
        internal bool isJumping;
        internal bool isStrafing
        {
            get
            {
                return _isStrafing;
            }
            set
            {
                _isStrafing = value;
            }
        }
        internal bool isGrounded { get; set; }
        internal bool isSprinting { get; set; }
        public bool stopMove { get; protected set; }

        internal float inputMagnitude;                      // sets the inputMagnitude to update the animations in the animator controller
        internal float verticalSpeed;                       // set the verticalSpeed based on the verticalInput
        internal float horizontalSpeed;                     // set the horizontalSpeed based on the horizontalInput       
        internal float moveSpeed;                           // set the current moveSpeed for the MoveCharacter method
        internal float verticalVelocity;                    // set the vertical velocity of the rigidbody
        internal float colliderRadius, colliderHeight;      // storage capsule collider extra information        
        internal float heightReached;                       // max height that character reached in air;
        internal float jumpCounter;                         // used to count the routine to reset the jump
        internal float groundDistance;                      // used to know the distance from the ground
        internal RaycastHit groundHit;                      // raycast to hit the ground 
        internal bool lockMovement = false;                 // lock the movement of the controller (not the animation)
        internal bool lockRotation = false;                 // lock the rotation of the controller (not the animation)        
        internal bool _isStrafing;                          // internally used to set the strafe movement                
        internal Transform rotateTarget;                    // used as a generic reference for the camera.transform
        internal Vector3 input;                             // generate raw input for the controller
        internal Vector3 colliderCenter;                    // storage the center of the capsule collider info                
        internal Vector3 inputSmooth;                       // generate smooth input based on the inputSmooth value       
        internal Vector3 moveDirection;                     // used to know the direction you're moving 

        #endregion


        #region Metodos Propios
        /*--------------------------------------------------------------------*\
        |* Metodos / Funciones Propias
        \*--------------------------------------------------------------------*/
        public void Init()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.Fixed;

            // slides the character through walls and edges
            frictionPhysics = new PhysicsMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicsMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicsMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicsMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider info
            _capsuleCollider = GetComponent<CapsuleCollider>();

            // save your collider preferences 
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;

            isGrounded = true;
        }

        public virtual void UpdateMotor()
        {
            CheckGround();
            CheckSlopeLimit();
            ControlJumpBehaviour();
            AirControl();
        }


        #region Locomotion
        //----------------------------------------------------------------------
        public virtual void SetControllerMoveSpeed(vMovementSpeed speed)
        {
            if (speed.walkByDefault)
                moveSpeed = Mathf.Lerp(moveSpeed, isSprinting ? speed.runningSpeed : speed.walkSpeed, speed.movementSmooth * Time.deltaTime);
            else
                moveSpeed = Mathf.Lerp(moveSpeed, isSprinting ? speed.sprintSpeed : speed.runningSpeed, speed.movementSmooth * Time.deltaTime);
        }

        public virtual void MoveCharacter(Vector3 _direction)
        {
            // calculate input smooth
            inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

            if (!isGrounded || isJumping) return;

            _direction.y = 0;
            _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
            _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);
            // limit the input
            if (_direction.magnitude > 1f)
                _direction.Normalize();

            Vector3 targetPosition = (useRootMotion ? animator.rootPosition : _rigidbody.position) + _direction * (stopMove ? 0 : moveSpeed) * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            bool useVerticalVelocity = true;
            if (useVerticalVelocity) targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = targetVelocity;
        }

        public virtual void CheckSlopeLimit()
        {
            if (input.sqrMagnitude < 0.1) return;

            RaycastHit hitinfo;
            var hitAngle = 0f;

            if (Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), transform.position + moveDirection.normalized * (_capsuleCollider.radius + 0.2f), out hitinfo, groundLayer))
            {
                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                var targetPoint = hitinfo.point + moveDirection.normalized * _capsuleCollider.radius;
                if ((hitAngle > slopeLimit) && Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), targetPoint, out hitinfo, groundLayer))
                {
                    hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                    if (hitAngle > slopeLimit && hitAngle < 85f)
                    {
                        stopMove = true;
                        return;
                    }
                }
            }
            stopMove = false;
        }

        public virtual void RotateToPosition(Vector3 position)
        {
            Vector3 desiredDirection = position - transform.position;
            RotateToDirection(desiredDirection.normalized);
        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            RotateToDirection(direction, isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed);
        }

        public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            if (!jumpAndRotate && !isGrounded) return;
            direction.y = 0f;
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime, .1f);
            Quaternion _newRotation = Quaternion.LookRotation(desiredForward);
            transform.rotation = _newRotation;
        }
        //----------------------------------------------------------------------
        #endregion


        #region Jump Methods
        //----------------------------------------------------------------------
        protected virtual void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }
            // apply extra force to the jump height   
            var vel = _rigidbody.linearVelocity;
            vel.y = jumpHeight;
            _rigidbody.linearVelocity = vel;
        }

        public virtual void AirControl()
        {
            if ((isGrounded && !isJumping)) return;
            if (transform.position.y > heightReached) heightReached = transform.position.y;
            inputSmooth = Vector3.Lerp(inputSmooth, input, airSmooth * Time.deltaTime);

            if (jumpWithRigidbodyForce && !isGrounded)
            {
                _rigidbody.AddForce(moveDirection * airSpeed * Time.deltaTime, ForceMode.VelocityChange);
                return;
            }

            moveDirection.y = 0;
            moveDirection.x = Mathf.Clamp(moveDirection.x, -1f, 1f);
            moveDirection.z = Mathf.Clamp(moveDirection.z, -1f, 1f);

            Vector3 targetPosition = _rigidbody.position + (moveDirection * airSpeed) * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, targetVelocity, airSmooth * Time.deltaTime);
        }

        protected virtual bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }
        //----------------------------------------------------------------------
        #endregion


        #region Ground Check                
        //----------------------------------------------------------------------
        protected virtual void CheckGround()
        {
            CheckGroundDistance();
            ControlMaterialPhysics();

            if (groundDistance <= groundMinDistance)
            {
                isGrounded = true;
                if (!isJumping && groundDistance > 0.05f)
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

                heightReached = transform.position.y;
            }
            else
            {
                if (groundDistance >= groundMaxDistance)
                {
                    // set IsGrounded to false 
                    isGrounded = false;
                    // check vertical velocity
                    verticalVelocity = _rigidbody.linearVelocity.y;
                    // apply extra gravity when falling
                    if (!isJumping)
                    {
                        _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                    }
                }
                else if (!isJumping)
                {
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }

        protected virtual void ControlMaterialPhysics()
        {
            // change the physics material to very slip when not grounded
            _capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (isGrounded && input == Vector3.zero)
                _capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && input != Vector3.zero)
                _capsuleCollider.material = frictionPhysics;
            else
                _capsuleCollider.material = slippyPhysics;
        }

        protected virtual void CheckGroundDistance()
        {
            if (_capsuleCollider != null)
            {
                // radius of the SphereCast
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;
                // ray for RayCast
                Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
                // raycast for check the ground distance
                if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
                    dist = transform.position.y - groundHit.point.y;
                // sphere cast around the base of the capsule to check the ground distance
                if (dist >= groundMinDistance)
                {
                    Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                    Ray ray = new Ray(pos, -Vector3.up);
                    if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                    {
                        Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                        float newDist = transform.position.y - groundHit.point.y;
                        if (dist > newDist) dist = newDist;
                    }
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        public virtual float GroundAngleFromDirection()
        {
            var dir = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;
            var movementAngle = Vector3.Angle(dir, groundHit.normal) - 90;
            return movementAngle;
        }
        //----------------------------------------------------------------------
        #endregion

        //----------------------------------------------------------------------
        #endregion


        #region Serializable Class vMovementSpeed
        //----------------------------------------------------------------------
        [System.Serializable]
        public class vMovementSpeed
        {
            [Range(1f, 20f)]
            public float movementSmooth = 6f;
            [Range(0f, 1f)]
            public float animationSmooth = 0.2f;
            [Tooltip("Velocidad de rotacion del personaje")]
            public float rotationSpeed = 16f;
            [Tooltip("El personaje limitara el movimiento a caminar en lugar de correr.")]
            public bool walkByDefault = false;
            [Tooltip("Gire/rote con la camara hacia adelante cuando este inactivo")]
            public bool rotateWithCamera = false;
            [Tooltip("Velocidad para caminar usando cuerpo rigido o velocidad adicional si estas usando RootMotion")]
            public float walkSpeed = 2f;
            [Tooltip("Velocidad para correr usando cuerpo rigido o velocidad adicional si estas usando RootMotion")]
            public float runningSpeed = 4f;
            [Tooltip("Velocidad para Sprint usando rigidbody o velocidad extra si estas usando RootMotion")]
            public float sprintSpeed = 6f;
        }
        //----------------------------------------------------------------------
        #endregion
    }
}

