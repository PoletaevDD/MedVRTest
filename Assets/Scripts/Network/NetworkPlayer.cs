using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Pilot References")]
    [SerializeField]
    PlayerPilotBase m_LocalPlayerPilot;
    [SerializeField]
    PlayerPilotBase m_RemotePlayerPilot;

    [Header("Transforms References")]
    [SerializeField]
    Transform m_HeadTransform;
    public Transform HeadTransform => m_HeadTransform;

    [Header("Spawnable Prefabs")]
    [SerializeField]
    GameObject m_ProjectilePrefab;
    [SerializeField]
    GameObject m_NPCPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();

        InitPilot();
    }

    void InitPilot()
    {
        var pilot = Instantiate(isLocalPlayer ? m_LocalPlayerPilot : m_RemotePlayerPilot, transform.position, transform.rotation, transform);
        pilot.Init(this);
    }

    [Command]
    public void CmdSpawnProjectile(Vector3 position, Quaternion rotation)
    {
        GameObject projectile = Instantiate(m_ProjectilePrefab, position, rotation);
        NetworkServer.Spawn(projectile);
    }

    [Command]
    public void CmdSpawnNPC(Vector3 position, Quaternion rotation)
    {
        GameObject projectile = Instantiate(m_NPCPrefab, position, rotation);
        NetworkServer.Spawn(projectile);
    }
}
