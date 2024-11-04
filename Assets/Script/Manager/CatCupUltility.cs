using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CatCup
{
    #region Prefab Path
    public const string BASE_LEVEL_SETUP_PATH = "Cat Cup Prefab/Base Level Setup";
    public const string LOCK_ZONE_PATH = "Cat Cup Prefab/Lock Zone";
    public const string MULTIPLE_ZONE_PATH = "Cat Cup Prefab/Multiple";
    public const string WALL_PATH = "Cat Cup Prefab/Wall";
    public const string Up_ZONE_PATH = "Cat Cup Prefab/Up";
    #endregion

    public const float MIN_X = -4;
    public const float MIN_Y = -3.5f;
    public const float MAX_X = 4;
    public const float MAX_Y = 4.5f;

    public static string levelPath
    {
        get
        {
            return Application.dataPath + "/Resources/Levels/";
        }
    }

    public static GameObject GetPrefab(ZONE_TYPE type)
    {
        switch (type)
        {
            case ZONE_TYPE.WALL:
                return Resources.Load<GameObject>(WALL_PATH);
            case ZONE_TYPE.MULTIPLE:
            case ZONE_TYPE.HIDDEN_MULTIPLE:
                return Resources.Load<GameObject>(MULTIPLE_ZONE_PATH);
            case ZONE_TYPE.LOCK:
                return Resources.Load<GameObject>(LOCK_ZONE_PATH);
            case ZONE_TYPE.UP_ZONE:
                return Resources.Load<GameObject>(Up_ZONE_PATH);
            default:
                return null;
        }
    }

    public static void CreateJsonFile(LevelData levelData, string saveFileName)
    {
        string json = JsonConvert.SerializeObject(levelData, Formatting.Indented);
        string filePath = levelPath + saveFileName + ".json";
        File.WriteAllText(filePath, json);
    }
    public static LevelData LoadJsonFile(string loadFileName)
    {
        LevelData rs = new LevelData();
        string filePath = Application.dataPath + "/Resources/Levels/" + loadFileName + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            rs = JsonConvert.DeserializeObject<LevelData>(json);
        }
        else
        {
            Debug.LogError("Start Level Failed, Could not find JSON file name: \"" + loadFileName + "\"!");
        }
        return rs;
    }


}



public class LevelData
{
    private List<List<ZoneNode>> m_nodeLst = new List<List<ZoneNode>>();
    private List<IDData> m_idDatas = new List<IDData>();

    public List<List<ZoneNode>> nodeLst { get { return m_nodeLst; } set { m_nodeLst = value; } }
    public List<IDData> idDatas { get { return m_idDatas; } set { m_idDatas = value; } }

    public void SetSize(Vector2 newSize)
    {
        for (int row = 0; row < newSize.x; row++)
        {
            if (row >= m_nodeLst.Count)
            {
                m_nodeLst.Add(new List<ZoneNode>());
            }

            for (int column = 0; column < newSize.y; column++)
            {
                if (column >= m_nodeLst[row].Count)
                {
                    m_nodeLst[row].Add(new ZoneNode());
                }

                if (column == newSize.y - 1)
                {
                    while (newSize.y < m_nodeLst[row].Count)
                    {
                        m_nodeLst[row].RemoveAt(m_nodeLst[row].Count - 1);
                    }
                }
            }

            if (row == newSize.x - 1)
            {
                while (newSize.x < m_nodeLst.Count)
                {
                    m_nodeLst.RemoveAt(m_nodeLst.Count - 1);
                }
            }
        }
    }

    public ZoneNode[] FindCouple(IDData idData)
    {
        List<ZoneNode> rs = new List<ZoneNode>();
        for (int i = 0; i < m_nodeLst.Count; i++)
        {
            for (int j = 0; j < m_nodeLst[i].Count; j++)
            {
                if (m_nodeLst[i][j].HaveThisID(idData.id))
                {
                    rs.Add(m_nodeLst[i][j]);
                }

                if (rs.Count >= 2)
                    break;
            }

            if (rs.Count >= 2)
                break;
        }

        return rs.ToArray();
    }
}

public class ZoneNode
{
    private string m_id;
    private float m_gridX;
    private float m_gridY;

    public string id { get { return m_id; } set { m_id = value; } }
    public float gridX { get { return m_gridX; } set { m_gridX = value; } }
    public float gridY { get { return m_gridY; } set { m_gridY = value; } }

    public ZoneNode()
    {
        m_id = "";
    }

    public bool HaveThisID(string id)
    {
        string[] idLst = m_id.Split(',');
        for (int i = 0; i < idLst.Length; i++)
        {
            if (idLst[i] == id)
                return true;
        }
        return false;
    }
}

public class IDData
{
    private string m_id = "";
    private ZONE_TYPE m_type = ZONE_TYPE.NONE;
    private int m_value = 0;

    public string id { get { return m_id; } set { m_id = value; } }
    public ZONE_TYPE type { get { return m_type; } set { m_type = value; } }
    public int value { get { return m_value; } set { m_value = value; } }

}

public enum ZONE_TYPE
{
    NONE,
    WALL,
    MULTIPLE,
    HIDDEN_MULTIPLE,
    LOCK,
    UP_ZONE,
}
