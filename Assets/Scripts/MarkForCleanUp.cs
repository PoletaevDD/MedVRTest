using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MarkForCleanUp : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();

        SceneCleanUp.AddObject(gameObject);
    }

    private void OnDestroy()
    {
        if (isServer)
            SceneCleanUp.RemoveObject(gameObject);
    }
}
