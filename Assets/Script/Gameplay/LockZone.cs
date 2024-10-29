using CatCup.Tool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockZone : MonoBehaviour, IZoneNode
{
    [SerializeField] int m_ballRequire = 10;

    [SerializeField] float m_startHeight = 0.2f;
    [SerializeField] float m_heightPerBall = 0.05f;
    [SerializeField] GameObject m_wall;

    [Header("Component")]
    [SerializeField] TextMeshProUGUI m_requireText;
    [SerializeField] SpriteRenderer m_sr;
    [SerializeField] BoxCollider2D m_checkCollider;
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] Transform m_wallLeft;
    [SerializeField] Transform m_wallRight;

    int m_currentBallNeed = 0;


    public void Init(IDData idData, float width, float angle)
    {
        m_ballRequire = idData.value;
        Vector2 _size = new Vector2(width, m_sr.size.y);
        m_sr.size = _size;
        m_collider.size = _size;
        m_checkCollider.size = new Vector2(_size.x, 0.5f);
        m_wallLeft.position = new Vector2(transform.position.x - _size.x / 2, transform.position.y);
        m_wallRight.position = new Vector2(transform.position.x + _size.x / 2, transform.position.y);
        m_requireText.text = m_ballRequire.ToString();

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Start()
    {
        m_currentBallNeed = m_ballRequire;
        m_requireText.text = m_currentBallNeed.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            m_currentBallNeed--;
            m_requireText.text = m_currentBallNeed.ToString();
            UpdateCheckColliderSize();


            if (m_currentBallNeed <= 0)
            {
                m_wall.SetActive(false);
                m_checkCollider.enabled = false;
            }
        }
    }
    
    private void UpdateCheckColliderSize()
    {
        float colliderHeight = m_collider.size.y / 2;
        int numOfBall = m_ballRequire - m_currentBallNeed;
        float height = 0.25f + (numOfBall * 0.05f);
        m_checkCollider.offset = new Vector2(0, height / 2);
        m_checkCollider.size = new Vector2(m_collider.size.x, height + colliderHeight);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            m_currentBallNeed++;
        }
    }
}
