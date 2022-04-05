using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField]
    float walkSpeed = 5;
    [SerializeField]
    float runSpeed = 10;
    [SerializeField]
    float jumpForce = 5;
    [SerializeField] 
    private float test = 0;

    private PlayerController playerController;
    Rigidbody rigidbody;
    Animator playerAnimator;
    public GameObject followTarget;

    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector2.zero;

    float lookUpMax = 180;
    float lookUpMin = -180;
    float lookUpClamp;

    public float aimSensitivity = 0.2f;

    [SerializeField] 
    private CinemachineVirtualCamera cinemachineAimCamera;

    public readonly int movementXHash = Animator.StringToHash("MovementX");
    public readonly int movementYHash = Animator.StringToHash("MovementY");
    public readonly int isJumpingHash = Animator.StringToHash("isJumping");
    public readonly int isRunningHash = Animator.StringToHash("isRunning");
    public readonly int isFiringHash = Animator.StringToHash("isFiring");
    public readonly int isReloadingHash = Animator.StringToHash("isReloading");
    public readonly int AimVerticalHash = Animator.StringToHash("AimVertical");

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        //cinemachineAimCamera.gameObject.SetActive(false);

        if (!GameManager.Instance.cursorActive)
        {
            AppEvents.InvokeOnMouseCursorEnable(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        lookUpClamp = Mathf.Clamp(0.0f, lookUpMin, lookUpMax);
    }

    // Update is called once per frame
    void Update()
    {
        followTarget.transform.rotation *= Quaternion.AngleAxis(lookInput.x * aimSensitivity, Vector3.up);
        followTarget.transform.rotation *= Quaternion.AngleAxis(lookInput.y * aimSensitivity, Vector3.left);

        var angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTarget.transform.localEulerAngles.x;

        float min = -60;
        float max = 70.0f;
        float range = max - min;
        float offsetToZero = 0 - min;
        float aimAngle = followTarget.transform.localEulerAngles.x;
        aimAngle = (aimAngle > 180) ? aimAngle - 360 : aimAngle;
        float val = (aimAngle + offsetToZero) / (range);
        //print(val);
        playerAnimator.SetFloat(AimVerticalHash, val);

        if (angle > 180 && angle < 300)
        {
            angles.x = 300;
        }
        else if (angle < 180 && angle > 70)
        {
            angles.x = 70;
        }
        lookUpClamp += lookInput.y;
        float lookParameter = Mathf.InverseLerp(lookUpMin, lookUpMax, lookUpClamp);

        playerAnimator.SetFloat(AimVerticalHash, lookParameter);

        followTarget.transform.localEulerAngles = angles;

        transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);

        followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        if (playerController.isJumping) return;
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;
        float currentSpeed = playerController.isRunning ? runSpeed : walkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);
        
        transform.position += movementDirection;

    }

    public void OnMovement(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        playerAnimator.SetFloat(movementXHash, inputVector.x);
        playerAnimator.SetFloat(movementYHash, inputVector.y);
    }
    public void OnRun(InputValue value)
    {
        playerController.isRunning = value.isPressed;
        playerAnimator.SetBool(isRunningHash, playerController.isRunning);
    }
    public void OnJump(InputValue value)
    {
        if ( playerController.isJumping)
        {
            return;
        }

        playerController.isJumping = value.isPressed;
        rigidbody.AddForce((transform.up + moveDirection) * jumpForce, ForceMode.Impulse);
        playerAnimator.SetBool(isJumpingHash, playerController.isJumping);
    }

    public void OnAim(InputValue value)
    {
        playerController.isAiming = value.isPressed;
        AimCamera(value.isPressed);
        Debug.Log("Aiming");
    }
    private void AimCamera(bool isAimPressed)
    {
        if (isAimPressed)
        {
            cinemachineAimCamera.gameObject.SetActive(true);
        }
        else
        {
            cinemachineAimCamera.gameObject.SetActive(false);
        }
    }
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        playerAnimator.SetFloat(AimVerticalHash, lookInput.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") && !playerController.isJumping) return;

        playerController.isJumping = false;
        playerAnimator.SetBool(isJumpingHash, false);

    }
}
