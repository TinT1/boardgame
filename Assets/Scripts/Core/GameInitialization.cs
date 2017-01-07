using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

using CO = CoreObject;
using Coor = Field.Coordinates;
using EvTrig = COEventTrigger;

public static class GameInitialization
{
    public static void Initialize(Game game)
    {
        #region Objects
        //  CO wall = new CO();

        CO.CORange defaultRange = new CO.CORange(positions: new List<Coor> {new Coor(-1,0), new Coor(1, 0), new Coor(0, -1), new Coor(0, 1),
                                                                             new Coor(-1,-1), new Coor(1, 1), new Coor(-1, 1), new Coor(1, -1) });

        CO.CORange jumpRange = new CO.CORange(positions: new List<Coor>());
        defaultRange.positions.ForEach(coor => jumpRange.positions.Add(2 * coor));

        #region pickupEvent
        COEvent pickUpEvent = new COEvent(name: "Pickup",
            eventTrigger: new EvTrig(EvTrig.Type.Pickup),
            eventAction: delegate (CO character, CO caller, CO target) {

                caller.coEvents.Add(new COEvent(name: "UsableOn",
                    eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start),
                                                roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { caller2.usable = true; }
                    ));

                caller.coEvents.Add(new COEvent(name: "DestroyAfterPickup",
                    eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }
                    ));
            });
        #endregion

        #region wallAndwallPlacer
        CO wall = new CO("Wall", block: true, type: CO.Type.Environment);
        wall.coEvents.Add(new COEvent(name: "WallSelfdestruct", eventAction: delegate (CO character, CO caller, CO target) { game.DestroyCO(caller); },
                                        eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start), depth: 2)));

        CO wallPlacer = new CO("WallPlacer");
        wallPlacer.coEvents.Add(new COEvent(name: "PlaceWall", eventAction: delegate (CO character, CO caller, CO target) { if (character.previousField != null) caller.AddToEnvironment(new CO(wall), character.previousField); else Debug.Log("nowall"); },
                                                eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Range, start: 1))));


        #endregion

        #region minePlacer
        
        
        CO smallMine = new CO("SmallMine", type: CO.Type.Weapon, publicVisibility: false);
        smallMine.coEvents.Add(new COEvent(name: "MineDmg", eventAction: delegate (CO character, CO caller, CO target)
         {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target) == false)
             {
                 game.DamageAndRepositionToBase(target);
                 caller.environmentOwner.SetInt("nbrOfMines", caller.environmentOwner.GetInt("nbrOfMines") + 1);
                 game.DestroyCO(caller);

             }


         }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));

        CO bigMine = new CO("BigMine", type: CO.Type.Weapon, publicVisibility: false);
        bigMine.coEvents.Add(new COEvent(name: "BigMineDmg", eventAction: delegate (CO character, CO caller, CO target)
         {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target) == false)
             {
                 game.DamageAndRepositionToBase(target);
                 game.DestroyCO(caller);

             }

         }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));

        CO minePlacer = new CO("SmallMinePlacer", usable: true,
                                 guiEvent: new COEvent(name: "PlaceMine", eventAction: delegate (CO character, CO caller, CO target)
                                          {
                                              if (caller.GetInt("nbrOfMines") > 0)
                                              {
                                                  CO newMine = new CO(smallMine);
                                                  newMine.coEvents.Add(new COEvent(
                                                     name:"RemoveMines",
                                                     eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 3), depth: 2),
                                                     eventAction: delegate (CO character2, CO caller2, CO target2)
                                                         {
                                                             caller2.environmentOwner.SetInt("nbrOfMines", caller2.environmentOwner.GetInt("nbrOfMines") + 1);
                                                             game.DestroyCO(caller2);
                                                         }
                                                     ));
                                                  caller.AddToEnvironment(newMine, character.currentField);
                                                  caller.SetInt("nbrOfMines", caller.GetInt("nbrOfMines") - 1);

                                              }
                                          }, eventTrigger: new EvTrig()),
                                range: new CO.CORange(new List<Coor>() { new Coor(0, 0) }),
                                type: CO.Type.Weapon);

        minePlacer.SetInt("nbrOfMines", 3);

        CO bigMinePlacer = new CO("BigMinePlacer", usable: true,
                                guiEvent: new COEvent(name: "PlaceBigMine", eventAction: delegate (CO character, CO caller, CO target)
                                                        {
                                                            if (caller.GetInt("nbrOfBigMines") > 0)
                                                            {
                                                                CO newMine = new CO(bigMine);
                                                                caller.AddToEnvironment(newMine, character.currentField);
                                                                caller.SetInt("nbrOfBigMines", caller.GetInt("nbrOfBigMines") - 1);

                                                            }
                                                        },
                                                      eventTrigger: new EvTrig()),
                               range: new CO.CORange(new List<Coor>() { new Coor(0, 0) }),
                               type: CO.Type.Weapon);

        bigMinePlacer.SetInt("nbrOfBigMines", 1);

        #endregion

        #region devour
        CO devour = new CO("Devour", usable: true,
                            guiEvent: new COEvent(  name:"Devour",
                                                    eventAction: delegate (CO character, CO caller, CO target) { game.MoveCurrChDamageAndRepositionToBase(target); },
                                                    eventTrigger: new EvTrig()),
                            range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(-1, 1), new Coor(-1, -1), new Coor(0, -1), new Coor(1, -1) }),
                            type: CO.Type.Weapon);

        devour.coEvents.Add(new COEvent(name: "DestroyDevourOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        devour.coEvents.Add(new COEvent(name: "DestroyDevourOnFinishTurn", eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
                                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        #endregion

        #region catapult
        CO catapult = new CO("Catapult", pickable: true,
                             guiEvent: new COEvent(name: "CatapultDmg", eventAction: delegate (CO character, CO caller, CO target) { game.DamageAndRepositionToBase(target); },
                                                  eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                            range: new CO.CORange(new List<Coor>() { new Coor(2, 2), new Coor(-2, 2), new Coor(2, 0), new Coor(0, 2), new Coor(-2, 0), new Coor(0, -2), new Coor(2, -2), new Coor(-2, -2) }),
                            type: CO.Type.Weapon);
        //ovak nesto?
        // bolje mina. svejedno
        catapult.coEvents.Add(new COEvent(pickUpEvent));

        catapult.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                            eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));

        #endregion
        #region shotgun
        CO shotgun = new CO("Shotgun", pickable: true,
                            guiEvent: new COEvent(name: "ShotgunDmg", eventAction: delegate (CO character, CO caller, CO target) {
                                Coor newCoor = target.currentField.coordinates + (target.currentField.coordinates - character.currentField.coordinates);
                                game.DamageAndRepositionToField(target, game.board[newCoor.i, newCoor.j]);
                            },
                                                  eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                            range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(-1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(0, -1), new Coor(1, -1), new Coor(-1, -1) }),
                            type: CO.Type.Weapon);
        shotgun.coEvents.Add(new COEvent(pickUpEvent));
        shotgun.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                            eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        #endregion
        #region bowAndArrow
        CO bowAndArrow = new CO("Bowandarrow",
                            /* endAction: delegate (CO character, CO caller, CO target)
                                  {
                                      Coor f = target.currentField;
                                  },*/
                            range: new CO.CORange(new List<Coor>() { new Coor(-2, 2), new Coor(2, -2), new Coor(-2, -2), new Coor(2, 2) }),
                            type: CO.Type.Character);

        #endregion


        #region laser
        CO laser = new CO("Laser", pickable: true,
                 guiEvent: new COEvent(name: "LaserDmg", eventAction: delegate (CO character, CO caller, CO target) { game.DamageAndRepositionToBase(target); },
                          eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(-1, 1), new Coor(1, -1), new Coor(-1, -1) }),
                type: CO.Type.Weapon);

        laser.coEvents.Add(new COEvent(pickUpEvent));

        laser.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                        eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        #endregion

        #region Baseball 
        CO baseball = new CO("Baseball", pickable: true,
                 guiEvent: new COEvent(name: "StunAction", 
                    eventAction: delegate (CO character, CO caller, CO target) { 
                      target.coEvents.Add(new COEvent(name: "NoMoveRound", 
                        eventAction: delegate (CO character2, CO caller2, CO target2) { game.step=game.maxStep; caller2.coEvents.RemoveAll(e => e.name== "NoMoveRound"); },
                        eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start), depth: 2)
                      ));
                    },
                    eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                 range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(-1, 1), new Coor(-1, -1), new Coor(0, -1), new Coor(1, -1) }),
                 type: CO.Type.Weapon);
        


        baseball.coEvents.Add(new COEvent(pickUpEvent));

        baseball.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                        eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        
        
        #endregion


        #region knife 
        CO knife = new CO("Knife", pickable: true,
                 guiEvent: new COEvent(name: "KnifeAction", 
                    eventAction: delegate (CO character, CO caller, CO target) { 
                       if(character.currentField== target.previousField) game.MoveCurrChDamageAndRepositionToBase(target);
                    },
                    eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                 range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(-1, 1), new Coor(-1, -1), new Coor(0, -1), new Coor(1, -1) }),
                 type: CO.Type.Weapon);
        


        knife.coEvents.Add(new COEvent(pickUpEvent));

        knife.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                        eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        
        
        #endregion



        #region crystal 



        CO crystal = new CO("Crystal", type: CO.Type.Crystal);

        #endregion

        #region itemPlacer
        CO itemPlacer = new CO("ItemPlacer");
        itemPlacer.coEvents.Add(new COEvent(    name:"ItemPlacerEv",
                                                eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Start)),
                                                eventAction: delegate (CO character, CO caller, CO target) {
                                                    System.Random rnd = new System.Random();
                                                    if (rnd.NextDouble() > 0.98f) game.board.TryRandomPlace(new CO(catapult));
                                                    if (rnd.NextDouble() > 0.98f) game.board.TryRandomPlace(new CO(laser));
                                                    if (rnd.NextDouble() > 0.9f)  game.board.TryRandomPlace(new CO(baseball));
                                                    if (rnd.NextDouble() > 0.9f)  game.board.TryRandomPlace(new CO(knife));
                                                    if (game.crystalsOnBoard < 5) { game.crystalsOnBoard++; game.board.TryRandomPlace(new CO(crystal)); }
                                                }));


        game.gameObjects.Add(itemPlacer);

        #endregion
        #endregion


        #region Characters
        CO zuti = new CO("Yellow");
        zuti.items.Add(wallPlacer);

        COEvent zutiUlta = new COEvent(name: "YellowUlt",
            eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
            eventAction: delegate (CO character, CO caller, CO target) {
                caller.AddToEnvironment(new CO(wall), game.board.fields[1, 1]);
                caller.AddToEnvironment(new CO(wall), game.board.fields[1, Board.n - 2]);
                caller.AddToEnvironment(new CO(wall), game.board.fields[Board.n - 2, 1]);
                caller.AddToEnvironment(new CO(wall), game.board.fields[Board.n - 2, Board.n - 2]);
                int m = Board.n / 2;
                List<int> ms = new List<int>() { m, m + 1, m - 1 };
                for (int t = 0; t < 3; ++t)
                {
                    m = ms[t];
                    caller.AddToEnvironment(new CO(wall), game.board.fields[1, m]);
                    caller.AddToEnvironment(new CO(wall), game.board.fields[m, 1]);
                    caller.AddToEnvironment(new CO(wall), game.board.fields[Board.n - 2, m]);
                    caller.AddToEnvironment(new CO(wall), game.board.fields[m, Board.n - 2]);
                }

                caller.coEvents.RemoveAll(x => x.name == caller.ulta.name);
            });

        zuti.ulta = zutiUlta;
        CO rozi = new CO("Pink");
        rozi.coEvents.Add(new COEvent(      name:"ClearStepHistory", 
                                            eventAction: delegate (CO character, CO caller, CO target) { caller.ClearStepHistory(); /*Debug.Log("clear step h");*/ },
                                            eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: 0))));

        CO jumper = new CO("Jumper", pickable: true, usable:true,
                             guiEvent: new COEvent(name: "Jumper", eventAction: delegate (CO character, CO caller, CO target) 
                             {
                                 //Debug.Log("jumper guievent");
                                 game.currCh.range = new CO.CORange(jumpRange);

                             },
                                                  eventTrigger: new EvTrig()),
                            range: new CO.CORange(new List<Coor>() { new Coor(0,0)}),
                            type: CO.Type.Weapon);
      
        jumper.coEvents.Add(new COEvent(name: "ReturnDefaultRangeAndDestroyJumperOnFinishTurn", eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
                                    eventAction: delegate (CO character2, CO caller2, CO target2) { 
                                      game.currCh.range = new CO.CORange(defaultRange);
                                      game.DestroyCO(caller2); }));
 

        rozi.coEvents.Add(new COEvent(name:"AddExtraStep",eventAction: delegate (CO character, CO caller, CO target){
            if(game.step!=0) 
              character.stepHistory.Add(character.previousField==null ? new Field.Coordinates(0,0) : 
                                                                      (character.currentField.coordinates - character.previousField.coordinates));
            if (caller.GratisStep())
                game.maxStep++;
           
            //Debug.Log("step rozi");
        },
        eventTrigger: new EvTrig()
        ));


        COEvent roziUlta = new COEvent(name: "roziUlt",
            eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Start),
                                    roundDes: new EvTrig.Description(exact: game.round + 1)),
            eventAction: delegate (CO character, CO caller, CO target) {
              rozi.items.Add(new CO(jumper));
              game.maxStep=2;
              caller.coEvents.RemoveAll(x => x.name == caller.ulta.name);
            });
        rozi.ulta = roziUlta;
        
        CO plavi = new CO("Blue", new int[] { 1, 1, 2, 3, 4, 6 }, blockFree: new List<CO.Type>() { CO.Type.Environment });

    
        CO crveni = new CO("Red");
        crveni.coEvents.Add(new COEvent(    name: "AddDevour",
                                            eventAction: delegate (CO character, CO caller, CO target) { caller.items.Add(devour); },
                                            eventTrigger: new EvTrig(roundDes: new EvTrig.Description(period: 3, pShift: 2),
                                                                        stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: 0))));


        CO crni = new CO("Black", canStepOnPrevious: true,pickUpOnMaxStep:false);

        CO stealer = new CO("stealer", usable: true,
                            guiEvent: new COEvent(name:"StealerGUIEv",eventAction: delegate (CO character, CO caller, CO target) 
                               { 
                                  game.StealItem(game.currCh,target);
                               },
                               eventTrigger: new EvTrig()),
                            range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(-1, 1), new Coor(-1, -1), new Coor(0, -1), new Coor(1, -1) }),
                            type: CO.Type.Weapon);

        stealer.coEvents.Add(new COEvent( name: "DestroyStealerOnUse", 
                                          eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                          eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        stealer.coEvents.Add(new COEvent( name: "DestroyStealerOnFinishTurn", 
                                          eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
                                          eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
 
        COEvent crniUlta = new COEvent(name: "crniUlt",
            eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Start),
                                    roundDes: new EvTrig.Description(exact: game.round + 1)),
            eventAction: delegate (CO character, CO caller, CO target) {
              crni.items.Add(new CO(stealer));
              caller.coEvents.RemoveAll(x => x.name == caller.ulta.name);
            });
        
        
        crni.ulta = crniUlta;



        CO zeleni = new CO("Green");

        zeleni.items.Add(minePlacer);
        zeleni.items.Add(bigMinePlacer);

        COEvent zeleniUlta = new COEvent(name: "GreenUlt",
            eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
            eventAction: delegate (CO character, CO caller, CO target) {

                CO ultaMine = new CO(bigMine);

                ultaMine.coEvents.Add(new COEvent(
                   name: "UltaMineEv",
                   eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1), depth: 2),
                   eventAction: delegate (CO character2, CO caller2, CO target2)
                        {
                            game.DestroyCO(caller2);
                        }
                        ));

                for (int m = 0; m < 6; ++m)
                {
                    CO newMine = new CO(ultaMine);
                    game.board.TryRandomPlace(newMine);
                    character.Own(newMine);
                }

                caller.coEvents.RemoveAll(x => x.name == caller.ulta.name);
            });

        zeleni.ulta = zeleniUlta;

        #endregion


        // znaci sad bi stavili event ultu, koji ubacuje "item" koji ima range, pogubljen sam dns
        // za pocetak mozemo staviti taj item da ima od pocetka da tesitramo kako funkcionira item jumper za dodavanje jumpa
        crveni.range=new CO.CORange(defaultRange);
        zeleni.range = new CO.CORange(defaultRange);
        zuti.range = new CO.CORange(defaultRange);
        rozi.range = new CO.CORange(defaultRange);
        crni.range = new CO.CORange(defaultRange);
        plavi.range = new CO.CORange(defaultRange);
        
        game.characters.Add(crni);
        game.characters.Add(rozi);

        game.characters.Add(zeleni);
        game.characters.Add(zuti);
        Board.Place(new CO(shotgun), game.board[5, 5]);

//        CoreObject.created.ForEach(c =>Debug.Log(c.name));
        var textures = Resources.LoadAll("Textures/CO",typeof(Texture));
        
        GameGui.customStyles = new Dictionary<String,GUIStyle>();
        for(int i=0; i < textures.Length; ++i) {
          GUIStyle guiStyle = new GUIStyle();
          guiStyle.name = textures[i].name;
          guiStyle.normal.background = textures[i] as Texture2D;
          guiStyle.stretchWidth=false;
          GameGui.customStyles.Add(guiStyle.name,guiStyle);

        }

        GUISkin savedCustomStylesSkin = Resources.Load("Skins/CO") as GUISkin;
        savedCustomStylesSkin.customStyles = new GUIStyle[textures.Length];
        int k=0; 
        foreach(GUIStyle style in GameGui.customStyles.Values) {
          savedCustomStylesSkin.customStyles[k] = style;
          ++k;
        }
        /* SerializedObject.ApplyModifiedProperties(coSkin); */
        /* SerializedProperty.Update(); */

    }

}

