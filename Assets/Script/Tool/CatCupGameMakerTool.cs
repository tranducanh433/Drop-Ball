using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace CatCup.Tool
{
    public class CatCupGameMakerTool : EditorWindow
    {
        List<List<ZoneData>> zoneLst = new List<List<ZoneData>>();

        private GameObject editLevelGO;
        private List<GameObject> spawnedZone = new List<GameObject>();
        private string saveFileName;
        private string loadFileName;


        private GUIStyle textStyle;

        const int MAX_ROW = 9;
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

            for (int row = 0; row < MAX_ROW; row++)
            {
                zoneLst.Add(new List<ZoneData>());
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            for (int row = 0; row < zoneLst.Count; row++)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("+", GUILayout.Width(45), GUILayout.Height(45)))
                {
                    zoneLst[row].Add(new ZoneData());
                    GUI.FocusControl(null);
                }

                for (int column = 0; column < zoneLst[row].Count; column++)
                {
                    EditorGUILayout.BeginVertical();
                    zoneLst[row][column].type = (ZONE_TYPE)EditorGUILayout.EnumPopup("", zoneLst[row][column].type, GUILayout.Width(80), GUILayout.Height(20));
                    zoneLst[row][column].value = EditorGUILayout.IntField(zoneLst[row][column].value, textStyle, GUILayout.Width(80), GUILayout.Height(20));
                    zoneLst[row][column].widthMulti = EditorGUILayout.IntField(zoneLst[row][column].widthMulti, textStyle, GUILayout.Width(80), GUILayout.Height(20));
                    EditorGUILayout.EndVertical();
                }

                if (zoneLst[row].Count > 0)
                {
                    if (GUILayout.Button("-", GUILayout.Width(45), GUILayout.Height(45)))
                    {
                        zoneLst[row].RemoveAt(zoneLst[row].Count - 1);
                        GUI.FocusControl(null);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("View", GUILayout.Height(30)))
            {
                ViewLevel();
                GUI.FocusControl(null);
            }


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

            Vector2 startAt = new Vector2(MIN_X, MAX_Y) + (Vector2)editLevelGO.transform.position;
            float stepY = (MAX_Y - MIN_Y) / MAX_ROW;
            for (int i = 0; i < zoneLst.Count; i++)
            {
                // Check if in this row have item
                int totalLength = 0;
                for (int j = 0; j < zoneLst[i].Count; j++)
                {
                    totalLength += zoneLst[i][j].widthMulti;
                }
                if (totalLength <= 0)
                    continue;


                float y = startAt.y - (stepY * i);
                float stepX = (MAX_X - MIN_X) / totalLength;
                float offsetX = stepX / 2f;
                float currentStep = 0;
                for (int j = 0; j < zoneLst[i].Count; j++)
                {
                    float x = startAt.x + stepX * currentStep + (offsetX * zoneLst[i][j].widthMulti);
                    Vector2 spawnPos = new Vector2 (x, y);
                    GameObject _prefab = GetPrefab(zoneLst[i][j].type);

                    if(_prefab == null) 
                        continue;

                    GameObject _spawn = Instantiate(_prefab, spawnPos, Quaternion.identity, editLevelGO.transform);
                    _spawn.GetComponent<IZoneNode>().Init(zoneLst[i][j], stepX);
                    spawnedZone.Add(_spawn);

                    currentStep += zoneLst[i][j].widthMulti;
                }
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
            string json = JsonConvert.SerializeObject(zoneLst, Formatting.Indented);
            string filePath = Application.dataPath + "/Resources/Levels/" + saveFileName + ".json";
            filePath = Application.dataPath + "/Resources/Levels/" + saveFileName + ".json";
            File.WriteAllText(filePath, json);
        }
        public void LoadJsonFile()
        {
            string filePath = Application.dataPath + "/Resources/Levels/" + loadFileName + ".json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                zoneLst = JsonConvert.DeserializeObject<List<List<ZoneData>>>(json);
                saveFileName = loadFileName;
                loadFileName = "";
            }

        }
    }
    public class ZoneData
    {
        private ZONE_TYPE m_type;
        private int m_value;
        private int m_widthMulti;

        public ZONE_TYPE type { get { return m_type; } set { m_type = value; } }
        public int value { get { return m_value; } set { m_value = value; } }
        public int widthMulti { get { return m_widthMulti; } set { m_widthMulti = value > 0 ? value : 1; } }

        public ZoneData() 
        {
            m_type = ZONE_TYPE.NONE;
            m_value = 0;
            m_widthMulti = 1;
        }
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

