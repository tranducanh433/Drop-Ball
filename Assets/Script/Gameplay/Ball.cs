using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] string m_baseLayer;
    [SerializeField] string m_bounceLayer;
    [SerializeField] Rigidbody2D m_rb;

    List<MultipleZone> m_interactedMultibleZones = new List<MultipleZone>();
    bool m_haveBounced = false;

    public MultipleZone[] lastMultibleZones { get { return m_interactedMultibleZones.ToArray(); } }
    public bool haveBounced { get { return m_haveBounced; } }
    public Vector2 vel { get { return GetComponent<Rigidbody2D>().velocity; } }

    public void Init(Ball ballToClone)
    {
        for (int i = 0; i < ballToClone.lastMultibleZones.Length; i++)
        {
            m_interactedMultibleZones.Add(ballToClone.lastMultibleZones[i]);
        }
        m_haveBounced = ballToClone.haveBounced;
        GetComponent<Rigidbody2D>().velocity = ballToClone.vel;
    }

    public bool Interact(MultipleZone zone)
    {
        if (!m_interactedMultibleZones.Contains(zone))
        {
            m_interactedMultibleZones.Add(zone);
            return true;
        }
        return false;
    }

    public void Bounce(Vector2 force)
    {
        gameObject.layer = LayerMask.NameToLayer(m_bounceLayer);
        m_haveBounced = true;
        m_interactedMultibleZones.Clear();
        StartCoroutine(CheckFallingCO(force));
    }

    IEnumerator CheckFallingCO(Vector2 force)
    {
        yield return null;
        m_rb.velocity = force;
        while (true)
        {
            if(m_rb.velocity.y < 0)
            {
                gameObject.layer = LayerMask.NameToLayer(m_baseLayer);
                break;
            }
            yield return null;
        }
    }
}
