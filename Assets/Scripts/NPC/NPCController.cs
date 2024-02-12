using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : NetworkBehaviour, IDamageable
{
    [Header("Speed Settings")]
    [SerializeField]
    float m_MaxSpeed = 5;
    [SerializeField]
    float m_SpeedChangeInSeconds = 1;
    [SerializeField]
    float m_SpeedChangeAfterMin = 1;
    [SerializeField]
    float m_SpeedChangeAfterMax = 5;

    [Header("NavMeshAgent Settings")]
    [SerializeField]
    float m_DestinationDistanceTreshold = 0.5f;
    [SerializeField]
    float m_NavMeshPickDestinationRadius = 5;

    Animator m_Animator;
    NavMeshAgent m_Agent;

    [SyncVar]
    float m_CurrentSpeed;
    float m_SpeedChangeTimer;

    Vector3 m_CurrentDestination;

    const string m_AnimatorSpeedParameter = "Speed";

    public static int NpcCount; 

    protected override void OnValidate()
    {
        base.OnValidate();

        m_NavMeshPickDestinationRadius = Mathf.Clamp(m_NavMeshPickDestinationRadius, 0, float.MaxValue);
        m_DestinationDistanceTreshold = Mathf.Clamp(m_DestinationDistanceTreshold, 0, float.MaxValue);
        m_SpeedChangeInSeconds = Mathf.Clamp(m_SpeedChangeInSeconds, 0, float.MaxValue);
        m_SpeedChangeAfterMin = Mathf.Clamp(m_SpeedChangeAfterMin, 0, float.MaxValue);
        m_SpeedChangeAfterMax = Mathf.Clamp(m_SpeedChangeAfterMax, 0, float.MaxValue);
        m_MaxSpeed = Mathf.Clamp(m_MaxSpeed, 0, float.MaxValue);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_NavMeshPickDestinationRadius);
    }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        NpcCount++;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (TryGetComponent(out m_Agent))
        {
            if (isServer)
                SetNewDestination(transform.position);
            else
                m_Agent.enabled = false;
        }
    }

    void OnDestroy()
    {
        NpcCount--;
    }

    private void Update()
    {
        UpdateAnimator();

        if (isServer)
        {
            HandleSpeedChange();
            HandleMovement();
        }
    }

    [Server]
    void HandleMovement()
    {
        if(m_Agent == null)
        {
            Debug.LogError($"NavMeshAgent on {gameObject} is null!");
            return;
        }

        m_Agent.speed = m_CurrentSpeed;

        if (Vector3.Distance(transform.position, m_CurrentDestination) <= m_DestinationDistanceTreshold)
        {
            if (!TryGetRandomPointOnNavMesh(out var newPoint))
                return;

            SetNewDestination(newPoint);
        }
    }

    void SetNewDestination(Vector3 newPoint)
    {
        m_CurrentDestination = newPoint;
        m_Agent.destination = m_CurrentDestination;
    }

    bool TryGetRandomPointOnNavMesh(out Vector3 point)
    {
        point = Vector3.zero;

        Vector3 posInRadius = Random.insideUnitSphere * m_NavMeshPickDestinationRadius + transform.position;
        if (NavMesh.SamplePosition(posInRadius, out NavMeshHit hit, m_NavMeshPickDestinationRadius, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }

        return false;
    }

    [Server]
    void HandleSpeedChange()
    {
        if(m_SpeedChangeTimer > 0)
        {
            m_SpeedChangeTimer -= Time.deltaTime;
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ChangeSpeedRoutine(Random.Range(0, m_MaxSpeed)));

        m_SpeedChangeTimer = Random.Range(m_SpeedChangeAfterMin, m_SpeedChangeAfterMax);
    }

    IEnumerator ChangeSpeedRoutine(float to)
    {
        float timer = 0;
        float from = m_CurrentSpeed;
        while (timer <= m_SpeedChangeInSeconds)
        {
            m_CurrentSpeed = Mathf.Lerp(from, to, timer / m_SpeedChangeInSeconds);

            timer += Time.deltaTime;
            yield return null;
        }

        m_CurrentSpeed = to;
    }

    void UpdateAnimator()
    {
        if(m_Animator == null)
        {
            Debug.LogError($"Animator on {gameObject} is null!");
            return;
        }

        m_Animator.SetFloat(m_AnimatorSpeedParameter, m_CurrentSpeed / m_MaxSpeed);
    }
}
