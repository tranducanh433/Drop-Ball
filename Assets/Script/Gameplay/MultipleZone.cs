using CatCup.Tool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleZone : MonoBehaviour, IZoneNode
{
    [Header("Zone Setting")]
    [SerializeField] int m_multiple = 2;
    [SerializeField] bool m_isHidden = false;

    [Header("Color Setting")]
    [SerializeField] Color m_baseColor;
    [SerializeField] Color m_hiddenColor;

    [Header("Component")]
    [SerializeField] TextMeshProUGUI m_multiText;
    [SerializeField] SpriteRenderer m_sr;
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] Transform m_wallLeft;
    [SerializeField] Transform m_wallRight;

    public void Init(IDData idData, float width, float angle)
    {
        m_multiple = idData.value;
        Vector2 _size = new Vector2(width, m_sr.size.y);
        m_sr.size = _size;
        m_collider.size = _size;
        m_wallLeft.position = new Vector2(transform.position.x - _size.x / 2, transform.position.y);
        m_wallRight.position = new Vector2(transform.position.x + _size.x / 2, transform.position.y);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (idData.type == ZONE_TYPE.HIDDEN_MULTIPLE)
        {
            DisplayHidden();
        }
        else
        {
            DisplayNormal();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            DisplayNormal();
            Ball _ball = collision.GetComponent<Ball>();
            if (!_ball.Interact(this))
                return;

            for (int i = 0; i < m_multiple - 1; i++)
            {
                Vector2 randPos = new Vector2(Random.Range(_ball.transform.position.x - 0.1f, _ball.transform.position.x + 0.1f)
                                                , Random.Range(_ball.transform.position.y - 0.1f, _ball.transform.position.y + 0.1f));
                GameObject _ballObj = Instantiate(_ball.gameObject, randPos, Quaternion.identity);
                _ballObj.GetComponent<Ball>().Init(_ball);
            }
        }
    }

    private void DisplayNormal()
    {
        m_multiText.text = "x" + m_multiple.ToString();
        m_sr.color = m_baseColor;
    }
    private void DisplayHidden()
    {
        m_multiText.text = "?";
        m_sr.color = m_hiddenColor;
    }

}
