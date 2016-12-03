using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameGui))]
public class GameGuiEditor : Editor {

    public override void OnInspectorGUI()
    {
        GameGui gameGui = target as GameGui;

        GameEditor.OnInspectorGUI(gameGui.game);
         
        
        //base.OnInspectorGUI();
    }
}
