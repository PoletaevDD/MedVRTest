using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : NetworkBehaviour
{
    [SerializeField]
    float m_Force = 50;
    [SerializeField]
    float m_Lifetime = 5;

    Rigidbody m_Rigidbody;

    [SyncVar(hook = nameof(OnStuckChanged))]
    bool m_IsStuck;

    float m_LifetimeTimer;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer)
            OnStuckChanged(false, m_IsStuck);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Fire();
    }

    [Server]
    void Fire()
    {
        m_Rigidbody.AddForce(transform.forward * m_Force);
    }

    private void Update()
    {
        if (isServer && !m_IsStuck)
        {
            LifetimeHandle();
        }
    }

    [Server]
    void LifetimeHandle()
    {
        if(m_LifetimeTimer >= m_Lifetime)
        {
            NetworkServer.Destroy(gameObject);
            return;
        }

        m_LifetimeTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
            return;

        if(collision.transform.root.TryGetComponent(out IDamageable _))
        {
            transform.parent = collision.transform;
            m_Rigidbody.isKinematic = true;

            m_IsStuck = true;
        }
    }

    void OnStuckChanged(bool oldVar, bool newVar)
    {
        m_Rigidbody.isKinematic = m_IsStuck;
    }
}
