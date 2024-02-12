using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class LocalPilot : PlayerPilotBase
{
    [SerializeField]
    Transform m_CameraTransform;
    [SerializeField]
    Transform m_ShootPointTransform;
    [SerializeField]
    LayerMask m_SpawnNpcLayers;

    public override void Init(NetworkPlayer networkPlayer)
    {
        base.Init(networkPlayer);

        SetConstraint(networkPlayer.HeadTransform, m_CameraTransform);
    }

    public void Shoot()
    {
        if(m_NetworkPlayer == null)
        {
            Debug.LogError($"NetworkPlayer on {gameObject} is null!");
            return;
        }

        m_NetworkPlayer.CmdSpawnProjectile(m_ShootPointTransform.position, m_ShootPointTransform.rotation);
    }

    public void SpawnNPC()
    {
        if (m_NetworkPlayer == null)
        {
            Debug.LogError($"NetworkPlayer on {gameObject} is null!");
            return;
        }

        if (Physics.Raycast(m_CameraTransform.position + m_CameraTransform.forward * 2f, Vector3.down, out RaycastHit hit, float.MaxValue, m_SpawnNpcLayers)) 
        {
            m_NetworkPlayer.CmdSpawnNPC(hit.point, Quaternion.LookRotation(-transform.forward));
        }
    }
}
