using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PrefabComponentLister : EditorWindow
{
	static string cachecfgPath = "Assets/AxiComToolCache.asset";
	static string outCsDir = Application.dataPath + "/AxiCom/";
	static Dictionary<string, AxiPrefabCache_Com2GUID> ComType2GUID = new Dictionary<string, AxiPrefabCache_Com2GUID>();

	[MenuItem("��ֲ����/[1]�ɼ�����Ԥ�����µ�UGUI���")]
	public static void Part1()
	{
		ComType2GUID.Clear();
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
	}
	static void GetPrefab(string path)
	{
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
		LoopPrefabNode(path, prefab.gameObject, 0);
	}
	static void LoopPrefabNode(string rootPath, GameObject trans, int depth)
	{
		string nodename = $"{rootPath}>{trans.name}";

		GameObject prefabRoot = trans.gameObject;
		int comCount = prefabRoot.GetComponentCount();
		for (int i = 0; i < comCount; i++)
		{
			var com = prefabRoot.GetComponentAtIndex(i);
			MonoBehaviour monoCom = com as MonoBehaviour;
			if (monoCom == null)
				continue;
			Type monoType = monoCom.GetType();
			if (!monoType.FullName.Contains("UnityEngine.UI"))
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
	}

	[MenuItem("��ֲ����/[2]�����м�ű�����")]
	public static void Part2()
	{
		if (Directory.Exists(outCsDir))
			Directory.Delete(outCsDir);
		Directory.CreateDirectory(outCsDir);
		AxiPrefabCache cache = AssetDatabase.LoadAssetAtPath<AxiPrefabCache>(cachecfgPath);
		foreach (var data in cache.caches)
		{
			string toName = "Axi" + data.SrcName;
			string toPath = outCsDir + toName + ".cs";
			string codeStr = "using UnityEngine.UI; public class " + toName + " : " + data.SrcName + " {}";
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
	}

	[MenuItem("��ֲ����/[3]�ռ����ɵĽű�")]
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


	[MenuItem("��ֲ����/[4]�滻����Ԥ����")]
	public static void Part4()
	{
		AxiPrefabCache cache = AssetDatabase.LoadAssetAtPath<AxiPrefabCache>(cachecfgPath);
		Dictionary<string, string> tempReplaceDict = new Dictionary<string, string>();
		foreach(var data in cache.caches)
		{
			tempReplaceDict[data.GUID] = data.ToGUID;
		}
		ProcessAllPrefabs("*.prefab", tempReplaceDict);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
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

}