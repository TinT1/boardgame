using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameGui))]
public class GameGuiEditor : Editor {

    public override void OnInspectorGUI()
    {
        GameGui gameGui = target as GameGui;
        if(gameGui.game!=null)GameEditor.OnInspectorGUI(gameGui.game);
    }
}
