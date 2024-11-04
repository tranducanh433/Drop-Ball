using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpZone : MonoBehaviour, IZoneNode
{
    [Header("Up Zone Setting")]
    [SerializeField] float m_minAngle;
    [SerializeField] float m_maxAngle;
    [SerializeField] float m_pushForce;

    [Header("Component")]
    [SerializeField] SpriteRenderer m_sr;
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] Transform m_wallLeft;
    [SerializeField] Transform m_wallRight;



    public void Init(IDData idData, float width, float angle)
    {
        m_pushForce = idData.value;
        Vector2 _size = new Vector2(width, m_sr.size.y);
        m_sr.size = _size;
        m_collider.size = _size;
        m_wallLeft.position = new Vector2(transform.position.x - _size.x / 2, transform.position.y);
        m_wallRight.position = new Vector2(transform.position.x + _size.x / 2, transform.position.y);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Ball _ball = collision.GetComponent<Ball>();
            if (_ball.haveBounced)
                return;


            float angleInRadians = Random.Range(m_minAngle, m_maxAngle) * Mathf.Deg2Rad;
            float vx = m_pushForce * Mathf.Cos(angleInRadians);
            float vy = m_pushForce * Mathf.Sin(angleInRadians);
            _ball.Bounce( new Vector2(vx, vy));
        }


    }
}
