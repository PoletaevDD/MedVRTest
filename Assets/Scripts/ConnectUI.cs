using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectUI : MonoBehaviour
{
    [SerializeField]
    NetworkManager m_NetworkManager;
    [SerializeField]
    Button m_ConnectButton;

    private void Awake()
    {
        m_ConnectButton.interactable = false;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void Connect()
    {
        if (m_NetworkManager == null)
        {
            Debug.LogError("NetworkManager is null!"); ;
            return;
        }

        m_NetworkManager.StartClient();
    }

    public void StartHost()
    {
        if(m_NetworkManager == null)
        {
            Debug.LogError("NetworkManager is null!"); ;
            return;
        }

        m_NetworkManager.StopClient();
        m_NetworkManager.StartHost();
    }

    public void OnServerAddressChanged(string newAddress)
    {
        m_NetworkManager.networkAddress = newAddress;

        string[] ipStringArr = newAddress.Split('.');

        m_ConnectButton.interactable = ipStringArr.Length == 4 && newAddress[newAddress.Length - 1] != '.';
    }
}
