using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    enum State
    {
        Idle,
        Move,
        Attack,
        Dying
    }

    [SerializeField] float rotationSpeed = 10;
    [SerializeField] float moveSpeed = 5;
    //    [SerializeField] float stopDistance = 0.1f;

    public static Action<PlayerController> OnPlayerSpawn;
    public static Action<PlayerController> OnPlayerDespawn;
    public StatDisplayer statDisplayer;

    Vector3 targetPosition;
    Vector3 attackTargetPosition;
    State state;
    Animator animator;



    bool isWaitingAttackInput;

    void Start()
    {
        state= State.Idle;
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            OnPlayerSpawn?.Invoke(this);
        }

        if (!IsOwner) return;

        targetPosition = transform.position;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            isWaitingAttackInput = true;

        }

        if (Input.GetMouseButtonDown(0) && 
            isWaitingAttackInput && 
            (state == State.Idle || state == State.Move) &&
            GetComponent<ProjectileLauncher>().IsAvailable()
            )
        {
            isWaitingAttackInput = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Ground")))
            {
                if (GetComponent<MagicPoint>().UseMagic(20))
                {
                    attackTargetPosition = hit.point;
                    SetState(State.Attack);
                    targetPosition = transform.position;

                    GetComponent<ProjectileLauncher>().Attack(attackTargetPosition);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && (state==State.Idle || state==State.Move) )
        {
            isWaitingAttackInput = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1000,LayerMask.GetMask("Ground") ))
            {
                targetPosition = hit.point;
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (state!=State.Attack)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            if (direction.magnitude > moveSpeed * Time.fixedDeltaTime)
            {
                SetState(State.Move);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                Vector3 movementVector = direction.normalized * moveSpeed * Time.fixedDeltaTime;
                GetComponent<Rigidbody>().MovePosition(transform.position + movementVector);

            }
            else
            {
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

                SetState(State.Idle);
            }
        }
        else
        {
            Vector3 direction = attackTargetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        Camera.main.transform.position = transform.position + new Vector3(0, 9, -9);

    }

    public void AttackEnded()
    {
        SetState(State.Idle);
    }

    void SetState(State newState)
    {
        switch (state)
        {
            case State.Idle:
                if (newState == State.Attack)
                {
                    animator.SetTrigger("Attack");
                }
                else if (newState == State.Move)
                {
                    animator.SetTrigger("Move");
                }
                break;
            case State.Move:
                if (newState == State.Attack)
                {
                    animator.SetTrigger("Attack");
                }
                else if (newState == State.Idle)
                {
                    animator.SetTrigger("Idle");
                }
                break;
            case State.Attack:
                animator.SetTrigger("Idle");
                break;
        }

        state = newState;
    }
}
