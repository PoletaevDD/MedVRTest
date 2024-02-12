using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RemotePilot : PlayerPilotBase
{
    [SerializeField]
    Transform m_HeadCubeTransform;

    public override void Init(NetworkPlayer networkPlayer)
    {
        base.Init(networkPlayer);

        SetConstraint(m_HeadCubeTransform, networkPlayer.HeadTransform);
    }
}
