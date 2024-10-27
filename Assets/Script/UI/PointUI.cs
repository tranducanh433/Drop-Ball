using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_pointText;

    public void SetPoint(int point)
    {
        m_pointText.text = point.ToString();
    }
}
