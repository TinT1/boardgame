using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public static class GameEditor
{
    static bool showCharacters = true;

    static bool showBoardObjects = false;

    public static void OnInspectorGUI(Game game)
    {
        EditorGUILayout.LabelField("round", game.round.ToString());

        showCharacters = EditorGUILayout.Foldout(showCharacters, "characters");

        if (showCharacters)game.characters.ForEach(character => CoreObjectEditor.OnInspectorGUI(character));

        showBoardObjects = EditorGUILayout.Foldout(showBoardObjects, "boardItems");

        if (showBoardObjects) game.gameObjects.ForEach(co => CoreObjectEditor.OnInspectorGUI(co));

    }
}

