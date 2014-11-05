using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

[CustomEditor(typeof(PuzzleData)), CanEditMultipleObjects]
public class KaraEditor : Editor
{
  public override void OnInspectorGUI()
  {
    if (Event.current.type == EventType.Layout)
    {
      return;
    }
    
    Rect position = new Rect(0, 
                             50, // Horrid Fucking Hack
                             Screen.width, 
                             Screen.height - 32);
    
    foreach (var item in targets)
    {
      PuzzleData pw = item as PuzzleData;
      Rect usedRect = InspectKara(position, pw);    
      position.y += usedRect.height;
    }        
  }

  [MenuItem ("Kara/Create Puzzle")]
  static void CreatePuzzle()
  {
    string path = EditorUtility.SaveFilePanel("Create Puzzle", "Assets/Resources/", "puzzle.asset", "asset");
    path = FileUtil.GetProjectRelativePath(path);

    PuzzleData pd = CreateInstance<PuzzleData>();
    AssetDatabase.CreateAsset(pd, path);
    AssetDatabase.SaveAssets();
  }
    
  public static Rect InspectKara(Rect position, PuzzleData tarPuz)
  {
    Rect saveOrig = position;


    tarPuz.m_width = EditorGUI.IntField(new Rect(position.x, 
                                                   position.y, 
                                                   position.width * 0.5f, 
                                                   EditorGUIUtility.singleLineHeight), 
                                          "Width", 
                                        tarPuz.m_width);
      
    tarPuz.m_height = EditorGUI.IntField(new Rect(position.x + position.width * 0.5f, 
                                                    position.y, 
                                                    position.width * 0.5f, 
                                                    EditorGUIUtility.singleLineHeight), 
                                           "Height", 
                                         tarPuz.m_height);
      
    position.y += EditorGUIUtility.singleLineHeight;
    position.height -= EditorGUIUtility.singleLineHeight;
      
    EditorGUI.BeginChangeCheck();
    float xWidth = Mathf.Min(position.width / Mathf.Max(1, tarPuz.m_width),
                             position.height / Mathf.Max(1, tarPuz.m_height));
      
    if (EditorGUI.EndChangeCheck() || 
      (tarPuz.m_data == null) || 
      (tarPuz.m_data.Length != (tarPuz.m_width * tarPuz.m_height)))
    {
      tarPuz.m_data = new int[tarPuz.m_width * tarPuz.m_height];
    }
      

    GUIStyle kaFontStyle = new GUIStyle(EditorStyles.textField);
    kaFontStyle.fontSize = Mathf.FloorToInt(xWidth * 0.7f);
    for (int x = 0; x < tarPuz.m_width; x++)
    {
      for (int y = 0; y < tarPuz.m_height; y++)
      {
        tarPuz.m_data [x + y * tarPuz.m_width] = 
            EditorGUI.IntField(new Rect(position.x + xWidth * x,
                                        position.y + xWidth * y,
                                        xWidth,
                                        xWidth), 
                             tarPuz.m_data [x + y * tarPuz.m_width], kaFontStyle);
          
      }
    }

    return new Rect(saveOrig.x, saveOrig.y, saveOrig.width, EditorGUIUtility.singleLineHeight * (tarPuz.m_height + 1));
  }
}