using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/*--------------------------------------------------------------------*\
|* Metodos / Funciones Propias
|*
|* Este si que es propio
|* - debemos tener:
|*      + Animator
\*--------------------------------------------------------------------*/
namespace TauriLand.Tps.Character
{
    public class cMyPersonAnimator : cThirdPersonController
    {
        #region Variables
        //----------------------------------------------------------------------
        // Variables
        //----------------------------------------------------------------------
        bool isInitiated = false;
        public bool isRunning = false;
        public bool isPaused = false;
        //----------------------------------------------------------------------
        #endregion


        #region Metodos Propios
        /*--------------------------------------------------------------------*\
        |* Metodos / Funciones Propias
        \*--------------------------------------------------------------------*/
        //------------------------------------------------------------------
        // Intentamos que el init solo se lance una vez
        //------------------------------------------------------------------
        public void ReInit()
        {
            if (!isInitiated)
            {
                Init();
            }
            isInitiated = true;
        }

        //------------------------------------------------------------------
        // Funcion de re-inicio
        // - no es init.
        // + deja al personaje parado y sin saltar.
        //------------------------------------------------------------------
        public void reset()
        {
            // Estamos parados y no estamos saltando:
            animator.speed = 0;
            animator.SetBool("IsSprinting", false);
            animator.SetFloat("InputMagnitude", 0f);
            animator.Update(0f);
            animator.SetBool("IsGrounded", true);
            // Ya hay un isJumping en cThirdPersonMotor/vThirdPersonMotor
            isJumping = false;
            isRunning = false;
            isPaused = false;
            animator.speed = 1;
        }

        public void run()
        {
            // Nada mas pulsarlo epezamos a correr
            if (!isRunning)
            {
                animator.SetBool("IsSprinting", true);
                animator.SetFloat("InputMagnitude", 1f);
                isRunning = true;
            }
        }

        public void walk(float velWalking = .5f)
        {
            // Vamos caminando:
            animator.SetBool("IsSprinting", false);
            animator.SetFloat("InputMagnitude", velWalking);
            isRunning = false;
        }

        public void jump()
        {
            if (!isJumping)
            {
                Jump();
                animator.SetBool("IsGrounded", false);
            }
        }

        //----------------------------------------------------------------------
        // Stop walking or running
        //----------------------------------------------------------------------
        public void stop()
        {
            // Vamos caminando:
            animator.SetBool("IsSprinting", false);
            animator.SetFloat("InputMagnitude", 0);
            isRunning = false;
        }

        //----------------------------------------------------------------------
        // Stop jumping, when we collision with ground
        // - to be called from cMyPersonMotor when we collide with ground
        //----------------------------------------------------------------------
        public void stopJump()
        {
            isJumping = false;
            animator.SetBool("IsGrounded", true);
        }

        //----------------------------------------------------------------------
        // Toggle de pausa de la animacion
        //----------------------------------------------------------------------
        public void togglePause()
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                animator.speed = 0f;
            }
            else
            {
                animator.speed = 1f;
            }
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
