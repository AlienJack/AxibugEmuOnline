using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SpriteProcessor : EditorWindow
{
    [MenuItem("Tools/�޸�������ʾ���")]
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
        EditorUtility.DisplayDialog("���", "���޸�������ʾ��顣", "OK");
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
