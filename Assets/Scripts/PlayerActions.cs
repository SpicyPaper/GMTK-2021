using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{

    public CharacterController2D controller;
    public MagnetFieldHandler magnetFieldHandler;
    public MagnetLauncherManager magnetLauncherLeft;
    public MagnetLauncherManager magnetLauncherRight;
    bool jump = false;

    [Header("Input Settings")]
    public PlayerInput playerInput;
    private Vector3 rawInputMovement;

    private void Start()
    {
        magnetLauncherLeft.isControlledByGamepad = playerInput.currentControlScheme == "Gamepad";
        magnetLauncherRight.isControlledByGamepad = playerInput.currentControlScheme == "Gamepad";
    }


    void FixedUpdate()
    {
        if (!GameManager.Instance.waitingForPlayers)
        {
            // Move our character
            controller.Move(rawInputMovement.x * Time.fixedDeltaTime, jump);
        }
        jump = false;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            jump = true;
        }
    }

    public void OnAim(InputAction.CallbackContext value)
    {
        magnetLauncherLeft.Aim(value.ReadValue<Vector2>());
        magnetLauncherRight.Aim(value.ReadValue<Vector2>());
    }

    public void OnAction1(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            magnetLauncherLeft.Pressed(true);
        }
        if (value.canceled)
        {
            magnetLauncherLeft.Released(true);
        }
    }

    public void OnAction2(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            magnetLauncherRight.Pressed(true);
        }
        if (value.canceled)
        {
            magnetLauncherRight.Released(true);
        }
    }


    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            // TODO attack
            magnetFieldHandler.Attack();
        }
    }

}