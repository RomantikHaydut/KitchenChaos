using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    //public static Player Instance { get; private set; }

    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;

    public event EventHandler OnPickSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private bool isWalking = false;

    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private LayerMask counterLayerMask;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Debug.LogError("There are more than 1 players !!!");
        //}

    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleMovement();
            HandleInteractions();
        }
    }

    private void HandleMovementServerAuth()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        HandleMovementServerRpc(inputVector);


    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        isWalking = moveDir != Vector3.zero;

        if (!canMove)
        {
            // Can not move towards to moveDir.

            // Attemt only X movement.
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDirX.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                // Can only move on the X axis.
                moveDir = moveDirX;
            }
            else
            {
                // Can not move on the X axis.

                // Attempt only Z axis.
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDirZ.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can only move on the Z axis.
                    moveDir = moveDirZ;
                }
                else
                {
                    isWalking = false;
                }

            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        Rotation(moveDir);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        isWalking = moveDir != Vector3.zero;

        if (!canMove)
        {
            // Can not move towards to moveDir.

            // Attemt only X movement.
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDirX.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                // Can only move on the X axis.
                moveDir = moveDirX;
            }
            else
            {
                // Can not move on the X axis.

                // Attempt only Z axis.
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDirZ.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can only move on the Z axis.
                    moveDir = moveDirZ;
                }
                else
                {
                    isWalking = false;
                }

            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        Rotation(moveDir);
    }

    private void Rotation(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        if (Physics.Raycast(transform.position, /*lastInteractDirection*/ transform.forward, out RaycastHit rayCastHit, interactDistance, counterLayerMask))
        {
            if (rayCastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has Clear Counter
                if (selectedCounter != baseCounter)
                {
                    SetSelectedCounter(baseCounter);
                }

            }
            else
            {
                SetSelectedCounter(null);
            }

        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter,

        });


    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return (kitchenObject != null);
    }
}
