using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AxiProjectTools : EditorWindow
{
	static string cachecfgPath = "Assets/AxiComToolCache.asset";
	static string toolSenceName = "AxiProjectTools";
	static string outCsDir = Application.dataPath + "/AxiCom/";
	static Dictionary<string, AxiPrefabCache_Com2GUID> ComType2GUID = new Dictionary<string, AxiPrefabCache_Com2GUID>();

	static void GoTAxiProjectToolsSence()
	{
		string[] sceneGuids = AssetDatabase.FindAssets("t:scene");
		foreach (string guid in sceneGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (path.Contains(toolSenceName))
			{
				EditorSceneManager.OpenScene(path);
				return;
			}
		}
	}

	[MenuItem("Axibug��ֲ����/[1]�ɼ�����Ԥ����ͳ����µ�UGUI���")]
	public static void Part1()
	{
		GoTAxiProjectToolsSence();
		ComType2GUID.Clear();
		string[] sceneGuids = AssetDatabase.FindAssets("t:scene");
		foreach (string guid in sceneGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (path.Contains(toolSenceName))
				continue;

			EditorSceneManager.OpenScene(path);

			// ����һ���б����洢���ڵ�
			List<GameObject> rootNodes = new List<GameObject>();

			// ���������е����ж���
			GameObject[] allObjects = FindObjectsOfType<GameObject>();
			foreach (GameObject obj in allObjects)
			{
				// �������Ƿ��и�����
				if (obj.transform.parent == null)
				{
					// ���û�и�����������һ�����ڵ�
					rootNodes.Add(obj);
				}
			}

			foreach (var node in rootNodes)
				LoopPrefabNode(path, node, 0);
		}


		string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
		foreach (string guid in prefabGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			GetPrefab(path);
		}

		AxiPrefabCache cache = ScriptableObject.CreateInstance<AxiPrefabCache>();
		foreach (var data in ComType2GUID)
			cache.caches.Add(data.Value);
		AssetDatabase.CreateAsset(cache, cachecfgPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		GoTAxiProjectToolsSence();
		Debug.Log("<Color=#FFF333>�������  [1]�ɼ�����Ԥ����ͳ����µ�UGUI���</color>");
	}

	static void GetPrefab(string path)
	{
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
		LoopPrefabNode(path, prefab.gameObject, 0);
	}
	static void LoopPrefabNode(string rootPath, GameObject trans, int depth)
	{
		string nodename = $"{rootPath}>{trans.name}";
#if UNITY_5_4_OR_NEWER && !UNITY_2018_4_OR_NEWER
        GameObject prefabRoot = trans.gameObject;
        int comCount = prefabRoot.GetComponentCount();
        for (int i = 0; i < comCount; i++)
        {
            var com = prefabRoot.GetComponentAtIndex(i);
            if (com == null)
                continue;

            if (com.name.Contains("VideoPlayer"))
            {
                
            }

            MonoBehaviour monoCom = com as MonoBehaviour;
            if (monoCom == null)
                continue;
            Type monoType = monoCom.GetType();
            if (!monoType.Assembly.FullName.Contains("UnityEngine.UI"))
                continue;
            // ��ȡMonoScript��Դ
            MonoScript monoScript = MonoScript.FromMonoBehaviour(monoCom);
            if (monoScript != null)
            {
                // ��ȡMonoScript��Դ��GUID
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(monoScript));
                Debug.Log($"{nodename}	|	<color=#FFF333>[{monoType.Name}]</color> <color=#FF0000>{guid}</color><color=#00FF00>({monoType.FullName})</color>");
                ComType2GUID[monoType.FullName] =
                    new AxiPrefabCache_Com2GUID()
                    {
                        SrcFullName = monoType.FullName,
                        SrcName = monoType.Name,
                        GUID = guid,
                    };
            }
            else
            {
                Debug.LogError("!!!! û��");
            }
        }

        //����
        foreach (Transform child in trans.transform)
            LoopPrefabNode(nodename, child.gameObject, depth + 1);
#else
		Debug.Log("�Ͱ汾��Ҫִ�б�����");
#endif
	}

	[MenuItem("Axibug��ֲ����/[2]�����м�ű�����")]
	public static void Part2()
	{
		if (UnityEngine.Windows.Directory.Exists(outCsDir))
			UnityEngine.Windows.Directory.Delete(outCsDir);
		Directory.CreateDirectory(outCsDir);
		AxiPrefabCache cache = AssetDatabase.LoadAssetAtPath<AxiPrefabCache>(cachecfgPath);
		foreach (var data in cache.caches)
		{
			string toName = "Axi" + data.SrcName;
			string toPath = outCsDir + toName + ".cs";
			string codeStr = "namespace AxibugCom { public class " + toName + " : " + data.SrcFullName + " {} }";
			try
			{
				System.IO.File.WriteAllText(toPath, codeStr);
				data.ToName = toName;
				data.ToPATH = toPath;
			}
			catch (Exception ex)
			{
				Debug.LogError("д��ʧ��" + ex.ToString());
			}
		}
		Debug.Log("д�����");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("<Color=#FFF333>�������  [2]�����м�ű�����</color>");
	}

	[MenuItem("Axibug��ֲ����/[3]�ռ����ɵĽű�")]
	public static void Part3()
	{
		AxiPrefabCache cache = AssetDatabase.LoadAssetAtPath<AxiPrefabCache>(cachecfgPath);
		List<MonoScript> allMonoScripts = FindAllAssetsOfType<MonoScript>();
		foreach (var data in cache.caches)
		{
			MonoScript monoScript = allMonoScripts.FirstOrDefault(w => w.name == data.ToName);
			if (monoScript == null)
			{
				Debug.LogError("û�ҵ�" + data.ToName);
				continue;
			}
			string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(monoScript));
			data.ToGUID = guid;
			data.monoScript = monoScript;
		}
		Debug.Log("д�����");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("<Color=#FFF333>�������  [3]�ռ����ɵĽű�</color>");
	}

	static List<T> FindAllAssetsOfType<T>() where T : UnityEngine.Object
	{
		List<T> assets = new List<T>();

		string[] allGuids = AssetDatabase.FindAssets("");
		foreach (string guid in allGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (path.EndsWith(".cs") || path.EndsWith(".js") || path.EndsWith(".boo")) // Unity֧�ֶ��ֽű����ԣ����ִ�Unity��Ҫʹ��C#
			{
				T asset = AssetDatabase.LoadAssetAtPath<T>(path);
				if (asset != null)
				{
					assets.Add(asset);
				}
			}
		}
		return assets;
	}


	[MenuItem("Axibug��ֲ����/[4]�滻����Ԥ����ͳ����е����")]
	public static void Part4()
	{
		AxiPrefabCache cache = AssetDatabase.LoadAssetAtPath<AxiPrefabCache>(cachecfgPath);
		Dictionary<string, string> tempReplaceDict = new Dictionary<string, string>();
		foreach (var data in cache.caches)
		{
			tempReplaceDict[data.GUID] = data.ToGUID;
		}
		ProcessAllPrefabs("*.prefab", tempReplaceDict);
		ProcessAllPrefabs("*.unity", tempReplaceDict);
		ProcessAllPrefabs("*.anim", tempReplaceDict);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("<Color=#FFF333>�������  [4]�滻����Ԥ����ͳ����е����</color>");
	}

	static void ProcessAllPrefabs(string form, Dictionary<string, string> tempReplaceDict, bool reverse = false)
	{
		List<GameObject> prefabs = new List<GameObject>();
		var resourcesPath = Application.dataPath;
		var absolutePaths = Directory.GetFiles(resourcesPath, form, SearchOption.AllDirectories);
		for (int i = 0; i < absolutePaths.Length; i++)
		{
			Debug.Log("prefab name: " + absolutePaths[i]);
			foreach (var VARIABLE in tempReplaceDict)
			{
				string oldValue = reverse ? VARIABLE.Value : VARIABLE.Key;
				string newValue = reverse ? VARIABLE.Key : VARIABLE.Value;
				ReplaceValue(absolutePaths[i], oldValue, newValue);
			}
			EditorUtility.DisplayProgressBar("����Ԥ���塭��", "����Ԥ�����С���", (float)i / absolutePaths.Length);
		}
		EditorUtility.ClearProgressBar();
	}

	/// <summary>
	/// �滻ֵ
	/// </summary>
	/// <param name="strFilePath">�ļ�·��</param>
	static void ReplaceValue(string strFilePath, string oldLine, string newLine)
	{
		if (File.Exists(strFilePath))
		{
			string[] lines = File.ReadAllLines(strFilePath);
			for (int i = 0; i < lines.Length; i++)
			{
				lines[i] = lines[i].Replace(oldLine, newLine);
			}
			File.WriteAllLines(strFilePath, lines);
		}
	}


	[MenuItem("Axibug��ֲ����/[5]UnPack����Ԥ����")]
	public static void UnpackPrefabs()
	{

		string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
		int prefabCount = 0;

		foreach (string path in allAssetPaths)
		{
			if (Path.GetExtension(path).Equals(".prefab"))
			{
				Debug.Log($"Unpacking {path}");
				UnpackPrefab(path);
				prefabCount++;
			}
		}
		Debug.Log($"Unpacked {prefabCount} prefabs.");

		Debug.Log("<Color=#FFF333>�������  [5]UnPack����Ԥ����</color>");
	}

	static void UnpackPrefab(string prefabPath)
	{
		GameObject prefabInstance = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		if (prefabInstance == null)
		{
			Debug.LogError($"Failed to load prefab at path: {prefabPath}");
			return;
		}

		var obj = GameObject.Instantiate(prefabInstance, null);
		TraverseHierarchy(obj);
		PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
		GameObject.DestroyImmediate(obj);
	}

	static void TraverseHierarchy(GameObject obj)
	{
		// ���ö����Ƿ���Ԥ�����ʵ��
		if (PrefabUtility.IsPartOfPrefabInstance(obj))
		{
			// ��Ԥ����ʵ��ת��Ϊ��ͨ��Ϸ����
			PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			Debug.Log("Prefab instance converted to game object: " + obj.name);
		}

		// �ݹ�����Ӷ���
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			TraverseHierarchy(obj.transform.GetChild(i).gameObject);
		}
	}


	[MenuItem("Axibug��ֲ����/[6]�޸�Sprite")]
	public static void FixMultipleMaterialSprites()
	{
		string[] guids = AssetDatabase.FindAssets("t:sprite");
		List<Sprite> spritesToFix = new List<Sprite>();

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

			// ����Ƿ��ж������
			if (IsUsingMultipleMaterials(sprite))
			{
				spritesToFix.Add(sprite);
				Debug.Log("Found sprite with multiple materials: " + path);
			}
		}

		// �޸�ÿ���ҵ���Sprite
		foreach (var sprite in spritesToFix)
		{
			FixSprite(sprite);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("<Color=#FFF333>�������  [6]�޸�Sprite</color>");
	}

	private static bool IsUsingMultipleMaterials(Sprite sprite)
	{
		if (sprite == null) return false;

		// ��ȡ����Ĳ���
		var textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;

		return textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple;
	}

	private static void FixSprite(Sprite sprite)
	{
		// ��ȡSprite��·��
		string path = AssetDatabase.GetAssetPath(sprite);
		var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

		if (textureImporter != null)
		{
			// ���浱ǰ�и���Ϣ
			SpriteMetaData[] originalMetaData = textureImporter.spritesheet;

			// ��ʱ����Sprite����
			textureImporter.spriteImportMode = SpriteImportMode.None;
			textureImporter.SaveAndReimport();

			// ��������Sprite���벢����ԭ���и����
			textureImporter.spriteImportMode = SpriteImportMode.Multiple;
			textureImporter.spritesheet = originalMetaData; // �ָ�ԭ�����и���Ϣ

			// ���µ�����Ӧ�ø���
			textureImporter.SaveAndReimport();
		}
	}
}