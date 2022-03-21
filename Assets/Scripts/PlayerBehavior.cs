// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace NoSuchCompany.Games.SuperMario
{
    #region Class

    public class PlayerBehavior : MonoBehaviour, IPlayer
    {
        public bool _jumpTriggered;

        public bool _isGrounded;

        public bool _isDead;

        public float jumpForce;

        public float moveSpeed;

        public Vector2 Position => transform.position;

        public Rigidbody2D playerRigidbody;

        public Transform groundCheck;

        public float groundCheckRadius;

        public LayerMask collisionLayers;

        public Animator playerAnimator;

        public SpriteRenderer spriteRenderer;

        private float _horizontalMovement;

        private Vector3 _velocity = Vector3.zero;

        public PlayerBehavior()
        {
            _jumpTriggered = false;
        }

        public void Update()
        {
            if (_isDead)
            {
                _horizontalMovement = 0f;
                return;
            }

            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            bool isDPadLeftPressed = Gamepad.current.dpad.left.isPressed;
            bool isDPadRightPressed = Gamepad.current.dpad.right.isPressed;
            bool isJumpPressed = Gamepad.current.buttonSouth.isPressed;
            bool isRunningPressed = Gamepad.current.buttonWest.isPressed;

            float speedFactor = isRunningPressed ? 1.6f : 1f;
            float horizontalAxis = isDPadLeftPressed ? -1f : isDPadRightPressed ? 1f : 0f;
            float deltaTime = Time.fixedDeltaTime;

            _horizontalMovement = horizontalAxis * speedFactor * moveSpeed * deltaTime;

            if (isJumpPressed && _isGrounded && !_jumpTriggered)
            {
                AppLogger.Write(LogsLevels.PlayerControls, $"**** Player pressed jump: Is Grounded = {_isGrounded} -- {Time.frameCount}");
                _jumpTriggered = true;
            }
        }

        public void FixedUpdate()
        {
            //  Reserved for physics. No inputs check.
            if (Position.x < Configurations.MinimumLeftPositionForPlayer && _horizontalMovement < 0f)
                _horizontalMovement = 0f;
            
            Move(_horizontalMovement);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        public void Move(InputAction.CallbackContext value)
        {
            var moveVal = value.ReadValue<Vector2>();
            transform.Translate(new Vector3(moveVal.x, moveVal.y, 0));
        }

        private void Move(float horizontalMovement)
        {
            const float SmoothTime = 0.05f;

            Vector3 targetVelocity = new Vector2(horizontalMovement, playerRigidbody.velocity.y);

            playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, targetVelocity, ref _velocity, SmoothTime);

            if (_jumpTriggered)
            {
                AppLogger.Write(LogsLevels.PlayerControls, $"--> Adding vertical force to the player as Jump has been triggered {Time.frameCount} {playerRigidbody.inertia}");
                
                playerRigidbody.AddForce(new Vector2(0f, jumpForce));
                _jumpTriggered = false;
                
                AppLogger.Write(LogsLevels.PlayerControls, $"--> {playerRigidbody.inertia}");
            }

            Flip(playerRigidbody.velocity.x);

            playerAnimator.SetFloat("Speed", Math.Abs(playerRigidbody.velocity.x));
            playerAnimator.SetBool("IsJumping", !_isGrounded);
        }

        private void Flip(float characterVelocity)
        {
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }

        public async void Kill()
        {
            _isDead = true;
            enabled = false;
            playerAnimator.SetBool("IsDead", true);

            await Task.Delay(TimeSpan.FromSeconds(2d)).ConfigureAwait(true);
                
            LevelManager.Instance.ReloadCurrentLevel();
        }
    }

    #endregion
}