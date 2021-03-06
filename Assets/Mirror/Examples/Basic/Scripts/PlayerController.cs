﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Smooth;

namespace App.Player
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : NetworkBehaviour
    {
        /// <summary>
        /// Speed of the Object's movement
        /// </summary>
        [SerializeField]
        private float _Speed = 0.5f;

        /// <summary>
        /// Speed of Camera Rotation
        /// </summary>
        [SerializeField]
        private float _CameraTurnSpeed = 0.5f;

        /// <summary>
        /// PlayerMotor
        /// </summary>
        private PlayerMotor _PlayerMotor;

        /// <summary>
        /// Check to see if the player is paused
        /// </summary>
        private bool _IsPaused = false;

        /// <summary>
        /// Amount of Force going upward
        /// </summary>
        public float JumpPower = 10.0f;

        /// <summary>
        /// Start this instance
        /// </summary>
        private void Start()
        {
            this._PlayerMotor = this.GetComponent<PlayerMotor>();
            this._IsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Update every frame
        /// </summary>
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                this._IsPaused = !this._IsPaused;
                HUDManager.Instance.MessageToggle(this._IsPaused);
                Cursor.lockState = this._IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = this._IsPaused;
            }

            if (this._IsPaused)
                return;

            //Spawn objects
            if(Input.GetKeyDown(KeyCode.E))
            {
                CmdRequestObjectSpawn();
                HUDManager.Instance.CommandSent(this.GetComponent<NetworkIdentity>().netId);
            }

            //Jump Up
            if(Input.GetKeyDown(KeyCode.Space))
            {
                this._PlayerMotor.Jump(this.JumpPower);
            }

            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
  
            Vector3 moveSideways = this.transform.right * moveX;
            Vector3 moveForward = this.transform.forward * moveY;

            Vector3 velocity = (moveSideways + moveForward).normalized * this._Speed;

            this._PlayerMotor.Move(velocity);

            float rotateY = Input.GetAxisRaw("Mouse X");

            Vector3 rotationY = new Vector3(0.0f, rotateY, 0.0f) * this._CameraTurnSpeed;

            this._PlayerMotor.Rotate(rotationY);

            float rotateX = Input.GetAxisRaw("Mouse Y");

            Vector3 rotationX = new Vector3(rotateX, 0.0f, 0.0f) * this._CameraTurnSpeed;

            this._PlayerMotor.RotateCamera(rotationX);
        }

        [Command]
        public void CmdRequestObjectSpawn()
        {
            SpawnPhysicsObject.Instance.SpawnObjects();
        }
    }
}

