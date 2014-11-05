using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(BuildTile))]
public class TilePrefabDrawer : PropertyDrawer
{
	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		return 40f;
	}
	
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		EditorGUI.LabelField(new Rect(position.xMin,
		                              position.yMin,
		                              position.width * 0.3f, 
		                              32f * 0.45f), label);

		SerializedProperty tt = property.FindPropertyRelative("m_type");
		TileTypes newVal = (TileTypes)EditorGUI.EnumPopup(new Rect(position.xMin,
		                                                           position.yMin + 32f * 0.5f,
		                                                           position.width * 0.3f, 
		                                                           32f * 0.45f), 
		                                                  (TileTypes)tt.enumValueIndex);
		tt.enumValueIndex = (int)newVal;

		SerializedProperty tile = property.FindPropertyRelative("m_tile");
		tile.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.xMin + position.width * 0.33f,
		                                                           position.yMin,
		                                                           position.width * 0.66f,
		                                                           32f * 0.45f), 
		                                                  tile.objectReferenceValue, 
		                                                  typeof(GameObject), 
		                                                  false);

		SerializedProperty blown = property.FindPropertyRelative("m_blownTile");
		blown.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.xMin + position.width * 0.33f,
		                                                            position.yMin + 32f * 0.5f, 
		                                                          position.width * 0.66f, 
		                                                            32f * 0.45f), 
		                                                   blown.objectReferenceValue, 
		                                                   typeof(GameObject), 
		                                                   false);

		GUIStyle newStyle = new GUIStyle(EditorStyles.miniLabel);
		newStyle.alignment = TextAnchor.MiddleRight;
		newStyle.fontSize = 10;
		newStyle.padding = new RectOffset(0,0,0,0);
		newStyle.margin =  new RectOffset(0,0,0,0);

		SerializedProperty isBlown = property.FindPropertyRelative("m_isBlown");
		EditorGUI.LabelField(new Rect(position.xMin + position.width * 0.33f, 
		                              position.yMin + 28f,
		                              position.width * 0.3f - 15f, 
		                              14f), "Blown", newStyle);
		isBlown.boolValue = EditorGUI.Toggle(new Rect(position.xMin + position.width * 0.33f + position.width * 0.3f - 30f, 
		                                              position.yMin + 28f,
		                                              14f, 
		                                              14f), isBlown.boolValue);


		SerializedProperty isBurn = property.FindPropertyRelative("m_isBurning"); 
		EditorGUI.LabelField(new Rect(position.xMin + position.width * 0.66f, 
		                              position.yMin + 28f,
		                              position.width * 0.3f - 15f, 
		                              14f), "Burning", newStyle);
		isBurn.boolValue = EditorGUI.Toggle(new Rect(position.xMin + position.width * 0.66f + position.width * 0.3f - 30f, 
		                                              position.yMin + 28f,
		                                              14f, 
		                                             14f), isBurn.boolValue);


		EditorGUI.DrawRect(new Rect(position.xMin,
		                            38f,
		                            position.width,
		                            2f), Color.grey);
	}


}
