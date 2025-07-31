using UnityEngine;


public class PlayerController : MonoBehaviour
{
	public Vector3 movementVector;
	public float moveSpeed, rotationSpeed;
	public CharacterController controller;
	public AnimatorController animatorController;

	[Header("Grouding")]
	public Transform groundCheckPivot;
	public LayerMask groundMask;
	public float groundCheckRadius;

	public Camera playerCamera;

	private Vector3 relativeToCameraDirection, relativeMovementVector, velocity, lastRelativeMovementDirection;
	private float gravity = -20;
	private bool grounded, canRotate, canMove;
	private float cameraForward;

	private const int ENEMY_LAYER = 3;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		canRotate = true;
		canMove = true;
	}

	private void Update()
	{
		Movement();
	}

	private void Movement()
	{
		//Base Input reads
		cameraForward = playerCamera.transform.eulerAngles.y;
		movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		relativeToCameraDirection = (Quaternion.Euler(0, cameraForward, 0) * movementVector).normalized;
		relativeMovementVector = relativeToCameraDirection * moveSpeed;
		relativeMovementVector.y = velocity.y;

		if (movementVector.sqrMagnitude > 0)
		{
			lastRelativeMovementDirection = relativeMovementVector;
			if (canRotate)
			{
				Rotation(lastRelativeMovementDirection);
			}
		}

		Gravity();



		if (canMove)
		{
			controller.Move((relativeMovementVector) * Time.deltaTime);
		}

		
		animatorController?.SetMovementSpeed(new Vector3(movementVector.x, 0, movementVector.z).normalized);
		
	}

	private void Rotation(Vector3 rotationTarget)
	{
		//rotationSpeedMultiplier = Mathf.Clamp01(rotationSpeedMultiplier);
		rotationTarget.y = 0;
		Quaternion targetRotation = Quaternion.LookRotation(rotationTarget, Vector3.up); //relativeToCameraDirection
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (rotationSpeed) * Time.deltaTime);
		//Debug.Log("<color=lime>RegularRotaion</color>");
	}

	private void Gravity()
	{
		if (Grounded() && velocity.y < 0)
		{
			velocity.y = 0.1f;
		}
		else
		{
			velocity.y += gravity * Time.deltaTime;
		}
	}

	private bool Grounded()
	{
		grounded = Physics.OverlapSphere(groundCheckPivot.transform.position, groundCheckRadius, groundMask).Length > 0;

		//if (!airborneFlag && !grounded)
		//{
		//	onAirborneEvent?.Invoke();
		//	airborneFlag = true;
		//}

		//animatorController.SetAnimGrounded(grounded);
		return grounded;
	}

	public void ToggleRotation(bool value)
	{
		canRotate = value;
	}

	public void ToggleMovement(bool value)
	{
		canMove = value;
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.gameObject.layer != ENEMY_LAYER)
		{
			return;
		}

		Rigidbody rb = hit.collider.attachedRigidbody;

		if (rb != null)
		{
			rb.isKinematic = true;
		}
	}
}
