using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerPilotBase : MonoBehaviour
{
    protected NetworkPlayer m_NetworkPlayer;

    public virtual void Init(NetworkPlayer networkPlayer)
    {
        m_NetworkPlayer = networkPlayer;
    }

    protected void SetConstraint(Transform from, Transform to)
    {
        from.position = to.position;
        from.rotation = to.rotation;

        if (!from.TryGetComponent(out ParentConstraint parentConstraint))
            parentConstraint = from.gameObject.AddComponent<ParentConstraint>();

        for (int i = 0; i < parentConstraint.sourceCount; i++)
        {
            parentConstraint.RemoveSource(i);
        }

        ConstraintSource source = new()
        {
            sourceTransform = to,
            weight = 1
        };

        parentConstraint.AddSource(source);
        parentConstraint.constraintActive = true;
    }
}
