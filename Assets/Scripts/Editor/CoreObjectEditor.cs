using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

using CO = CoreObject;
public static class CoreObjectEditor
{

    static bool showCharacter = true;

    public static void OnInspectorGUI(CoreObject co)
    {
        showCharacter= EditorGUILayout.Foldout(showCharacter, co.name);

        if (showCharacter)
        {
            EditorGUILayout.LabelField("health", co.health.ToString());
            EditorGUILayout.LabelField("currentField", co.currentField!=null?co.currentField.Print():"");
            EditorGUILayout.LabelField("equipedItem", co.equipedItem != null ? co.equipedItem.name : "e");
            EditorGUILayout.LabelField("crystals",  co.crystals.ToString());

            EditorGUILayout.LabelField("range", co.range != null ? co.range.positions.Count.ToString() : "e");

            PrintCOLess(co,0);
        }


    }


    public static void PrintCOLess(CO co,int depth)
    {
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20 * depth);

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField(depth.ToString()+co.name);
        
      //  GUILayout.BeginHorizontal();
        
        if(co.items.Count!=0)EditorGUILayout.LabelField("Items:");
        foreach (CO item in co.items)
            PrintCOLess(item,depth+1);


        if (co.environment.Count!=0) EditorGUILayout.LabelField("Envs:");
        foreach (CO envObj in co.environment)
            PrintCOLess(envObj, depth + 1);

        if (co.coEvents.Count != 0) EditorGUILayout.LabelField("events:");
        foreach (COEvent ev in co.coEvents)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20 * (depth+1));
            EditorGUILayout.LabelField(ev.Print());
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
      //  GUILayout.EndHorizontal();
    }
}

