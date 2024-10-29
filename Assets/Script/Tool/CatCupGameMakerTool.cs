using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace CatCup.Tool
{
    public class CatCupGameMakerTool : EditorWindow
    {
        LevelData levelData = new LevelData();

        private GameObject editLevelGO;
        private List<GameObject> spawnedZone = new List<GameObject>();
        private string saveFileName;
        private string loadFileName;
        private int row;
        private int col;


        private GUIStyle textStyle;

        const float MIN_X = -4;
        const float MIN_Y = -3.5f;
        const float MAX_X = 4;
        const float MAX_Y = 4.5f;

        #region Prefab Path
        const string BASE_LEVEL_SETUP_PATH = "Cat Cup Prefab/Base Level Setup";
        const string LOCK_ZONE_PATH = "Cat Cup Prefab/Lock Zone";
        const string MULTIPLE_ZONE_PATH = "Cat Cup Prefab/Multiple";
        const string WALL_PATH = "Cat Cup Prefab/Wall";
        const string Up_ZONE_PATH = "Cat Cup Prefab/Up";
        #endregion



        [MenuItem("Cat Cup/Level Maker Tool")]
        static void OpenWindow()
        {
            CatCupGameMakerTool window = (CatCupGameMakerTool)GetWindow(typeof(CatCupGameMakerTool));
            window.minSize = new Vector2(600, 300);
            window.Show();
        }
        private void OnEnable()
        {
            textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Row and Column Setting
            EditorGUILayout.LabelField("Row");
            row = EditorGUILayout.IntField(row, GUILayout.Width(100));
            EditorGUILayout.LabelField("Column");
            col = EditorGUILayout.IntField(col, GUILayout.Width(100));

            if (GUILayout.Button("Create Grid", GUILayout.Width(100)))
            {
                levelData.SetSize(new Vector2(row, col));
                GUI.FocusControl(null);
            }

            GUILayout.Space(30);

            // Grid Setting
            List<List<ZoneNode>> nodeLst = levelData.nodeLst;

            for (int row = 0; row < nodeLst.Count; row++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int column = 0; column < nodeLst[row].Count; column++)
                {
                    nodeLst[row][column].gridX = column;
                    nodeLst[row][column].gridY = row;
                    EditorGUILayout.BeginVertical();
                    nodeLst[row][column].id = EditorGUILayout.TextField(nodeLst[row][column].id, textStyle, GUILayout.Width(80), GUILayout.Height(20));
                    EditorGUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }


            // Id Setting
            if(levelData.nodeLst.Count > 0)
            {
                EditorGUILayout.LabelField("ID Setting");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID", GUILayout.Width(100), GUILayout.Height(20));
                EditorGUILayout.LabelField("Type", GUILayout.Width(100), GUILayout.Height(20));
                EditorGUILayout.LabelField("Value", GUILayout.Width(100), GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();

                List<IDData> idDataLst = levelData.idDatas;
                for (int i = 0; i < idDataLst.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    idDataLst[i].id = EditorGUILayout.TextField(idDataLst[i].id, textStyle, GUILayout.Width(100), GUILayout.Height(20));
                    idDataLst[i].type = (ZONE_TYPE)EditorGUILayout.EnumPopup("", idDataLst[i].type, GUILayout.Width(100), GUILayout.Height(20));
                    idDataLst[i].value = EditorGUILayout.IntField(idDataLst[i].value, textStyle, GUILayout.Width(100), GUILayout.Height(20));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(45), GUILayout.Height(45)))
                {
                    idDataLst.Add(new IDData());
                    GUI.FocusControl(null);
                }
                if(idDataLst.Count > 0)
                {
                    if (GUILayout.Button("-", GUILayout.Width(45), GUILayout.Height(45)))
                    {
                        idDataLst.RemoveAt(idDataLst.Count - 1);
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }


            // View
            if (GUILayout.Button("View", GUILayout.Height(30)))
            {
                ViewLevel();
                GUI.FocusControl(null);
            }

            // Save and Load
            EditorGUILayout.BeginHorizontal();
            saveFileName = EditorGUILayout.TextField(saveFileName, GUILayout.Height(30));
            if (GUILayout.Button("Save as JSON", GUILayout.Height(30)))
            {
                CreateJsonFile();
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            loadFileName = EditorGUILayout.TextField(loadFileName, GUILayout.Height(30));
            if (GUILayout.Button("Load JSON", GUILayout.Height(30)))
            {
                LoadJsonFile();
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
        }

        private void ViewLevel()
        {
            if (editLevelGO == null)
            {
                GameObject prefab = Resources.Load<GameObject>(BASE_LEVEL_SETUP_PATH);

                GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Prefab");
                Selection.activeObject = spawnedObject;

                editLevelGO = spawnedObject;
            }

            for (int i = 0; i < spawnedZone.Count; i++)
            {
                DestroyImmediate(spawnedZone[i]);
            }
            spawnedZone.Clear();

            List<List<ZoneNode>> nodeLst = levelData.nodeLst;
            List<IDData> idDatas = levelData.idDatas;

            Vector2 startAt = new Vector2(MIN_X, MAX_Y) + (Vector2)editLevelGO.transform.position;
            float stepY = (MAX_Y - MIN_Y) / (nodeLst.Count - 1);
            float stepX = (MAX_X - MIN_X) / (nodeLst[0].Count - 1);
            for (int i = 0; i < idDatas.Count; i++)
            {
                ZoneNode[] couple = levelData.FindCouple(idDatas[i]);
                if (couple.Length < 2 || idDatas[i].id == "")
                    continue;

                Vector2 pos0 = new Vector2(startAt.x + couple[0].gridX * stepX
                                            , startAt.y - couple[0].gridY * stepY);
                Vector2 pos1 = new Vector2(startAt.x + couple[1].gridX * stepX
                                            , startAt.y - couple[1].gridY * stepY);


                Vector2 spawnPos = (pos0 + pos1) / 2f;
                float distance = Vector2.Distance(pos0, pos1);
                Vector2 direction = pos1 - pos0;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                GameObject prefab = GetPrefab(idDatas[i].type);
                if (prefab == null)
                    continue;
                GameObject spawn = Instantiate(prefab, spawnPos, Quaternion.identity, editLevelGO.transform);
                spawn.GetComponent<IZoneNode>().Init(idDatas[i], distance, angle);
                spawnedZone.Add(spawn);


            }
        }

        private GameObject GetPrefab(ZONE_TYPE type)
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

        private void CreateJsonFile()
        {
            string json = JsonConvert.SerializeObject(levelData, Formatting.Indented);
            string filePath = Application.dataPath + "/Resources/Levels/" + saveFileName + ".json";
            File.WriteAllText(filePath, json);
        }
        public void LoadJsonFile()
        {
            string filePath = Application.dataPath + "/Resources/Levels/" + loadFileName + ".json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                levelData = JsonConvert.DeserializeObject<LevelData>(json);
                saveFileName = loadFileName;
                loadFileName = "";
                row = levelData.nodeLst.Count;
                col = levelData.nodeLst.Count > 0 ? levelData.nodeLst[0].Count : 0;
            }

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
            for (int row = 0; row < newSize.y; row++)
            {
                if (row >= m_nodeLst.Count)
                {
                     m_nodeLst.Add(new List<ZoneNode>());
                }

                for (int column = 0; column < newSize.x; column++)
                {
                    if (column >= m_nodeLst[row].Count)
                    {
                        m_nodeLst[row].Add(new ZoneNode());
                    }

                    if(column == newSize.x - 1)
                    {
                        while (newSize.x < m_nodeLst[row].Count)
                        {
                            m_nodeLst[row].RemoveAt(m_nodeLst[row].Count - 1);
                        }
                    }
                }

                if (row == newSize.y - 1)
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
        private Vector2 m_indexPosition;
        private string m_id;
        private float m_gridX;
        private float m_gridY;

        public string id { get { return m_id; } set { m_id = value; } }
        public float gridX { get { return m_gridX; } set { m_gridX = value; } }
        public float gridY { get { return m_gridY; } set { m_gridY = value; } }

        public ZoneNode() 
        {
            m_indexPosition = Vector2.zero;
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
}

