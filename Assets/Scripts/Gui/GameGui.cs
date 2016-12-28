﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using CO = CoreObject;
public class GameGui : MonoBehaviour
{
    public Game game;
    public Board board;

    public static bool inputFinished;

    public float fieldW = 70f, fieldH = 50f;
    float ratio = 0.9f;

    void Awake()
    {
        Field.SetSkin(); SetSkins();
        game = new Game();
    }

    public static void SetSkins()
    {
        skins["FinishTurnPassive"] = Resources.Load("Skins/FinishTurnPassive") as GUISkin;
    }

    public static Dictionary<string, GUISkin> skins = new Dictionary<string, GUISkin>();



    void OnGUI()
    {
        GUISkin defaultSkin = GUI.skin;

        if (board == null) board = game.board;

        #region fields
        for (int i = 0; i < Board.n; ++i)
            for (int j = 0; j < Board.n; ++j)
            {

                Field boardField = board[i, j];

                if (boardField.Show() == false) continue;
                GUI.skin = boardField.Skin();


                string buttonLabel = "";

                GUI.Box(new Rect(i * fieldW, j * fieldH, fieldW, fieldH), "");
                Rect nameRect = new Rect(i * fieldW, j * fieldH, ratio * fieldW, fieldH);
                Rect attackRect = new Rect(i * fieldW + nameRect.width, j * fieldH, (1f - ratio) * fieldW, fieldH);


                int visibleObjsCount = 0;
                boardField.fieldObjects.ForEach(x => visibleObjsCount += x.IsVisibleTo(game.currCh) ? 1 : 0);


                for (int fieldObjectIndex = 0; boardField.fieldObjects.Count == 0 || fieldObjectIndex < boardField.fieldObjects.Count; ++fieldObjectIndex)
                {

                    float div = (visibleObjsCount == 0) ? 1f : (1f / (float)visibleObjsCount);
                    float postotak = fieldObjectIndex * div;

                    nameRect = new Rect(i * fieldW, (j + postotak) * fieldH, ratio * fieldW, fieldH * div);


                    buttonLabel = (visibleObjsCount == 0) ? "" : boardField.fieldObjects[fieldObjectIndex].name;

                    if (game.currCh.GetState == CO.State.Move) buttonLabel += game.CanCurrentCharacterMove(board[i, j]) ? "*" : "";
                    if (game.currCh.GetState == CO.State.UseItem) buttonLabel += game.CanCurrentCharacterUseItem(boardField) ? "XY" : "";

                    if (GUI.Button(nameRect, buttonLabel))
                    {
                        if (game.currCh.GetState == CO.State.Move && game.CanCurrentCharacterMove(boardField) == true)
                            game.MoveCurrentCharacter(boardField);

                        if (game.currCh.GetState == CO.State.UseItem && game.CanCurrentCharacterUseItem(board[i, j]) == true)
                            if (boardField.fieldObjects.Exists(co => co.type == CoreObject.Type.Character))
                            {
                                game.CurrentCharacterUseItem(boardField);
                            }
                    }
                    if (boardField.fieldObjects.Count == 0) break;
                }



                // if (GUI.Button(atacckRect, "")) ;

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

        if (game.CanFinishTurn() == false) GUI.skin = skins["FinishTurnPassive"];

        if (GUI.Button(new Rect((Board.n - 1 + 1) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "Skip4Turns"))
        { game.FinishTurn(); game.FinishTurn(); game.FinishTurn(); game.FinishTurn(); }
        if (GUI.Button(new Rect((Board.n - 1) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "FinishTurn")) game.FinishTurn();
        GUI.Box(new Rect((Board.n - 2) * fieldW, (Board.n) * fieldH, fieldW, fieldH), (game.dice + 1) + ":" + game.maxStep + ":" + game.step);
        GUI.Box(new Rect((Board.n - 3) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "round:" + game.round);
        GUI.Box(new Rect((Board.n - 4) * fieldW, (Board.n) * fieldH, fieldW, fieldH), "turn:" + game.turn);

    }
}