using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject m_ballPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(m_ballPrefab, spawnPos, Quaternion.identity);
        }
    }
}
