using CatCup.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallZone : MonoBehaviour, IZoneNode
{
    [SerializeField] GameObject[] m_centerWall;
    [SerializeField] GameObject[] m_leftWall;
    [SerializeField] GameObject[] m_rightWall;



    public void Init(ZoneData zoneData, float width)
    {
        int wallID = zoneData.value;

        int totalLength = m_centerWall.Length + m_leftWall.Length + m_rightWall.Length;
        if (wallID >= totalLength)
            return;

        for (int i = 0; i < totalLength; i++)
        {
            if(i < m_centerWall.Length)
            {
                m_centerWall[i].SetActive(i == wallID);
            }
            else if (i < m_leftWall.Length + m_centerWall.Length)
            {
                m_leftWall[i - m_centerWall.Length].SetActive(i == wallID);
            }
            else if (i < m_rightWall.Length + m_centerWall.Length + m_leftWall.Length)
            {
                m_rightWall[i - m_centerWall.Length - m_leftWall.Length].SetActive(i == wallID);
            }
        }

        float _left = width * zoneData.widthMulti;
        for (int i = 0; i < m_leftWall.Length; i++)
        {
            m_leftWall[i].transform.position = new Vector2(transform.position.x - _left / 2, transform.position.y);
        }

        for (int i = 0; i < m_leftWall.Length; i++)
        {
            m_rightWall[i].transform.position = new Vector2(transform.position.x + _left / 2, transform.position.y);
        }
    }
}
