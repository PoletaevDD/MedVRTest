using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SceneCleanUp : NetworkBehaviour
{
    static HashSet<GameObject> m_AllDynamicObjects = new HashSet<GameObject>();

    [Server]
    public static void AddObject(GameObject obj)
    {
        m_AllDynamicObjects.Add(obj);
    }

    [Server]
    public static void RemoveObject(GameObject obj)
    {
        m_AllDynamicObjects.Remove(obj);
    }

    public void CleanUp()
    {
        if (authority)
            ServerCleanUpAll();
        else
            CmdCleanUp();
    }

    [Command(requiresAuthority = false)]
    public void CmdCleanUp()
    {
        ServerCleanUpAll();
    }

    [Server]
    public void ServerCleanUpAll()
    {
        foreach (var obj in m_AllDynamicObjects)
        {
            NetworkServer.Destroy(obj);
        }

        m_AllDynamicObjects.Clear();
    }
}
