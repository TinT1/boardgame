﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


using CO = CoreObject;
public class GameGui : MonoBehaviour
{
    public Game game;
    public Board board;

    public static readonly bool inputFinished;

    public static Dictionary<string,GUIStyle> customStyles;

    public float fieldW = 70f, fieldH = 50f;
    const float ratio = 0.8f;

    void Awake()
    {
        Field.SetSkin(); SetSkins();
        game = new Game();
    }

    public static void SetSkins()
    {
        skins["Background"] = Resources.Load("Skins/Background") as GUISkin;
        skins["CO"] = Resources.Load("Skins/CO") as GUISkin;
        skins["Default"] = Resources.Load("Skins/DefaultGuiSkin") as GUISkin;
        skins["FinishTurnPassive"] = Resources.Load("Skins/FinishTurnPassive") as GUISkin;
    }

    public static Dictionary<string, GUISkin> skins = new Dictionary<string, GUISkin>();



    void OnGUI()
   {
        GUISkin defaultSkin = skins["Default"];
        GUI.skin = skins["Background"];
        GUI.Box(new Rect(0f,0f,Screen.width,Screen.height),"");
        GUI.skin = defaultSkin;
        if (board == null) board = game.board;

        float contentX = 0;
        GUILayout.BeginArea(new Rect(contentX,0f,8f*Screen.width,Screen.height));

        GUI.skin = skins["CO"];

        #region fields
        for (int i = 0; i < Board.n; ++i)
            for (int j = 0; j < Board.n; ++j)
            {

                Field boardField = board[i, j];

                if (!boardField.Show()) continue;

                //GUI.skin = game.Skin(boardFieldra);
                StringBuilder buttonLabel = new StringBuilder();

//                GUI.Box(new Rect(i * fieldW, j * fieldH, fieldW, fieldH), "",game.Skin(boardField).box);
                Rect nameRect = new Rect(i * fieldW, j * fieldH, ratio * fieldW, fieldH);
                List<CO> visibleFieldObjects = new List<CO>();
                boardField.fieldObjects.ForEach(x =>  { if(x.IsVisibleTo(game.currCh)) visibleFieldObjects.Add(x);} );

                for (int fieldObjectIndex = 0; visibleFieldObjects.Count == 0 || fieldObjectIndex < visibleFieldObjects.Count; ++fieldObjectIndex)
                {

                    float div = (visibleFieldObjects.Count == 0) ? 1f : (1f / (float)visibleFieldObjects.Count);
                    float postotak = fieldObjectIndex * div;

                    nameRect = new Rect(i * fieldW, (j + postotak) * fieldH, ratio * fieldW, fieldH * div);



                    GUIStyle buttonStyle = (visibleFieldObjects.Count == 0) ? defaultSkin.button : customStyles[visibleFieldObjects[fieldObjectIndex].name];

                    if (game.currCh.GetState == CO.State.Move) buttonLabel.Append(game.CanCurrentCharacterMove(board[i, j]) ? "*" : "");
                    if (game.currCh.GetState == CO.State.UseItem) buttonLabel.Append(game.CanCurrentCharacterUseItem(boardField) ? "XY" : "");

                    if (GUI.Button(nameRect, buttonLabel.ToString(),buttonStyle))
                    {
                        if (game.currCh.GetState == CO.State.Move && game.CanCurrentCharacterMove(boardField))
                            game.MoveCurrentCharacter(boardField);

                        if (game.currCh.GetState == CO.State.UseItem && game.CanCurrentCharacterUseItem(board[i, j])
                            && visibleFieldObjects.Exists(co => co.type == CoreObject.Type.Character))
                            game.CurrentCharacterUseItem(boardField);
                    }
                    if (visibleFieldObjects.Count == 0) break;
                }
            }
        #endregion

        GUI.skin = defaultSkin;
        int row = 0;

        #region inventory

        if (game.currCh.GetState == CO.State.UseItem)
        {
            if (GUI.Button(new Rect((Board.n) * fieldW, fieldH * row, fieldW, fieldH), "Move")) game.currCh.UnEquip();
            ++row;
        }
        game.currCh.items.ForEach(item => {
            if (GUI.Button(new Rect((Board.n) * fieldW, fieldH * row, fieldW, fieldH), item.name) && game.CanEquip(item))
            {
                game.currCh.Equip(item);
            }
            ++row;
        });

        row = 0;
        game.currCh.currentField.fieldObjects.ForEach(fObj => {
            if (game.CanPickUp(fObj) && GUI.Button(new Rect((Board.n + 1) * fieldW, fieldH * row, fieldW, fieldH), "pickUp:" + fObj.name))
            {
                game.PickUp(fObj);
            }
            if (game.CanCollectCrystal(fObj) && GUI.Button(new Rect((Board.n + 2) * fieldW, fieldH * row, fieldW, fieldH), "collectCrystal:" + fObj.name))
            {
                game.CollectCrystal(fObj);
            }
            ++row;
        });

        #endregion

        if (!game.CanFinishTurn()) GUI.skin = skins["FinishTurnPassive"];

        if (GUI.Button(new Rect((Board.n - 1 + 1) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "Skip4Turns"))
        { game.FinishTurn(); game.FinishTurn(); game.FinishTurn(); game.FinishTurn(); }
        if (GUI.Button(new Rect((Board.n - 1) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "FinishTurn")) game.GuiFinishTurn();
        GUI.Box(new Rect((Board.n - 2) * fieldW, (Board.n) * fieldH, fieldW, fieldH), (game.dice + 1) + ":" + game.maxStep + ":" + game.step);
        GUI.Box(new Rect((Board.n - 3) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "round:" + game.round);
        GUI.Box(new Rect((Board.n - 4) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "turn:" + game.turn);


        GUILayout.EndArea();
    }
}
