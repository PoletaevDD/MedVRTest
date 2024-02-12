using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI m_Text;
	[SerializeField]
	TextMeshProUGUI m_NpcText;

	float deltaTime = 0.0f;
	float fps;

	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

		fps = 1.0f / deltaTime;

		m_Text.text = $"{(int)fps} FPS";
		m_NpcText.text = $"{NPCController.NpcCount} NPC in Scene";
	}
}
