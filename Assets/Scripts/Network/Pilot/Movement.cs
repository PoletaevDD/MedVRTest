using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    [SerializeField]
    Transform m_CameraTransform;
    [SerializeField]
    float m_MoveSpeed = 10;
    [SerializeField]
    float m_GravityForce = 9;

    public UnityEvent OnTouch;

    CharacterController m_CharacterController;
    bool m_IsMoving;

    void OnValidate()
    {
        m_MoveSpeed = Mathf.Clamp(m_MoveSpeed, 0, float.MaxValue);
        m_GravityForce = Mathf.Clamp(m_GravityForce, 0, float.MaxValue);
    }

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        Input.gyro.enabled = true;
    }

    private void Update()
    {
        HandleMove();
        HandleTouch();
        HandleGyro();
    }

    public void SwitchGyroMode()
    {
        Input.gyro.enabled = !Input.gyro.enabled;
    }

    void HandleGyro()
    {
        if (Input.gyro.enabled)
        {
            Vector3 rotateAngles = -Input.gyro.rotationRateUnbiased;
            rotateAngles.z *= -1;
            m_CameraTransform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.z);
        }
    }

    void HandleMove()
    {
        if (m_CharacterController == null)
        {
            Debug.LogError($"CharacterController on {gameObject} is null!");
            return;
        }

        Vector3 moveVector = Vector3.down * m_GravityForce;
        if (m_IsMoving)
        {
            moveVector += m_CameraTransform.forward * m_MoveSpeed;
        }

        m_CharacterController.Move(moveVector * Time.deltaTime);
    }

    public void SetMovingState(bool moving)
    {
        m_IsMoving = moving;
    }

    // Check if we touch any UI and if dont --> Shoot
    void HandleTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (!EventSystem.current.IsPointerOverGameObject(id) && touch.phase == TouchPhase.Began)
            {
                OnTouch?.Invoke();
            }
        }

#if UNITY_EDITOR
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            OnTouch?.Invoke();
#endif
    }
}
