using TauriLand.Tps.Camera;
using UnityEngine;

/*--------------------------------------------------------------------*\
|* Metodos / Funciones Propias
|*
|*  Este codigo sale de  Invector.vCharacterController
\*--------------------------------------------------------------------*/
namespace TauriLand.Tps.Character
{
    public class cThirdPersonInput : MonoBehaviour
    {
        #region Variables       
        //----------------------------------------------------------------------
        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public cThirdPersonController cc;
        [HideInInspector] public cThirdPersonCamera tpCamera;
        [HideInInspector] public UnityEngine.Camera cameraMain;
        //----------------------------------------------------------------------
        #endregion


        #region MonoBehaviour
        /*====================================================================*\
        |* Funciones MonoBehaviour
        |* - Vamos a introducir Tool como componente.
        \*====================================================================*/
        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }
        //----------------------------------------------------------------------
        #endregion


        #region Metodos Propios
        /*--------------------------------------------------------------------*\
        |* Metodos / Funciones Propias
        \*--------------------------------------------------------------------*/
        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<cThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                // tpCamera = FindObjectOfType<cThirdPersonCamera>();   // Obsoleto a sustituir por FindFirstObjectByType<>
                tpCamera = FindFirstObjectByType<cThirdPersonCamera>();

                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        public virtual void MoveInput()
        {
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.z = Input.GetAxis(verticallInput);
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!UnityEngine.Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = UnityEngine.Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var X = Input.GetAxis(rotateCameraXInput);
            var Y = Input.GetAxis(rotateCameraYInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                cc.Jump();
        }
        //----------------------------------------------------------------------
        #endregion

        //----------------------------------------------------------------------
        #endregion
    }
}
