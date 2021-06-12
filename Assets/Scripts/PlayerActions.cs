using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{

	public CharacterController2D controller;
	bool jump = false;

	[Header("Input Settings")]
	public PlayerInput playerInput;
	private Vector3 rawInputMovement;
	private Vector3 rawAimMovement;

	private bool action1 = false;
	private bool action2 = false;


	void FixedUpdate()
	{
		// Move our character
		controller.Move(rawInputMovement.x * Time.fixedDeltaTime, jump);
		// TODO
		// magnetController.Aim(rawAimMovement);
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
		Vector2 aimMovement = value.ReadValue<Vector2>();
		rawAimMovement = new Vector3(aimMovement.x, 0, aimMovement.y);
	}

	public void OnAction1(InputAction.CallbackContext value)
	{
		if (value.started)
			action1 = true;

		if (value.canceled)
			action1 = false;
	}

	public void OnAction2(InputAction.CallbackContext value)
	{
		if (value.started)
			action2 = true;

		if (value.canceled)
			action2 = false;
	}


	public void OnAttack(InputAction.CallbackContext value)
	{
		if (value.started)
        {
			// TODO attack
        }
	}

}