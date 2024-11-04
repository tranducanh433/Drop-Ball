using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallZone : MonoBehaviour, IZoneNode
{
    [SerializeField] SpriteRenderer m_sr;
    [SerializeField] CapsuleCollider2D m_collider;



    public void Init(IDData idData, float width, float angle)
    {
        Vector2 _size = new Vector2(width + 0.2f, m_sr.size.y);
        m_sr.size = _size;
        m_collider.size = _size;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
