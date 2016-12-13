using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using CO = CoreObject;
using Coor = Field.Coordinates;
using EvTrig = COEventTrigger;

public static class GameInitialization
{
    public static void Initialize(Game game)
    {
        #region Objects
        //  CO wall = new CO();

        #region pickupEvent
        COEvent pickUpEvent = new COEvent(name:"pickup",
            eventTrigger: new EvTrig(EvTrig.Type.Pickup),
            eventAction: delegate (CO character, CO caller, CO target) {

                caller.coEvents.Add(new COEvent(name:"usableOn",
                    eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start),
                                                roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { caller2.usable = true;}
                    ));

                caller.coEvents.Add(new COEvent(name:"DestroyAfterPickup",
                    eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }
                    ));
            });
        #endregion

        #region wallAndwallPlacer
        CO wall = new CO("wall", block: true, type: CO.Type.Environment);
        wall.coEvents.Add(new COEvent(name:"WallSelfdestruct", eventAction: delegate (CO character, CO caller, CO target) { game.DestroyCO(caller); },
                                        eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start), depth: 2)));

        CO wallPlacer = new CO("wallPlacer");
        wallPlacer.coEvents.Add(new COEvent(name:"PlaceWall", eventAction: delegate (CO character, CO caller, CO target) { if (character.previousField != null) caller.AddToEnvironment(new CO(wall), character.previousField); else Debug.Log("nowall"); },
                                                eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Range, start: 1))));


        #endregion

        #region minePlacer

        CO mine = new CO("mine", type: CO.Type.Weapon);
        mine.coEvents.Add(new COEvent(name:"MineDmg", eventAction: delegate (CO character, CO caller, CO target)
        {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target)==false)
            {
                game.DamageAndReposionToBase(target);
                caller.environmentOwner.SetInt("nbrOfMines", caller.environmentOwner.GetInt("nbrOfMines") + 1);
                game.DestroyCO(caller);
                   
            }

        

        }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));



        CO bigMine = new CO("bigMine", type: CO.Type.Weapon);
        bigMine.coEvents.Add(new COEvent(name:"BigMineDmg", eventAction: delegate (CO character, CO caller, CO target)
        {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target) == false)
            {
                game.DamageAndReposionToBase(target);
                game.DestroyCO(caller);

            }

        }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));

        CO minePlacer = new CO("smallMinePlacer", usable: true,
                                 guiEvent: new COEvent(name:"PlaceMine", eventAction: delegate (CO character, CO caller, CO target) 
                                         {
                                             if (caller.GetInt("nbrOfMines") > 0)
                                             {
                                                 CO newMine = new CO(mine);
                                                 newMine.coEvents.Add(new COEvent(
                                                    eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 3),depth:2),
                                                    eventAction: delegate (CO character2, CO caller2, CO target2) 
                                                        {
                                                            caller2.environmentOwner.SetInt("nbrOfMines", caller2.environmentOwner.GetInt("nbrOfMines") + 1);
                                                            game.DestroyCO(caller2);
                                                        }
                                                    ));
                                                 caller.AddToEnvironment(newMine, character.currentField);
                                                 caller.SetInt("nbrOfMines", caller.GetInt("nbrOfMines") - 1);
                                        
                                             }
                                         },eventTrigger:new EvTrig()),
                                range: new CO.CORange(new List<Coor>() { new Coor(0, 0) }),
                                type: CO.Type.Weapon);

        minePlacer.SetInt("nbrOfMines", 3);

        CO bigMinePlacer = new CO("bigMinePlacer", usable: true,
                                guiEvent: new COEvent(name: "PlaceBigMine", eventAction: delegate (CO character, CO caller, CO target)
                                                        {
                                                            if (caller.GetInt("nbrOfBigMines") > 0)
                                                            {
                                                                CO newMine = new CO(bigMine);
                                                                caller.AddToEnvironment(newMine, character.currentField);
                                                                caller.SetInt("nbrOfBigMines", caller.GetInt("nbrOfBigMines") - 1);

                                                            }
                                                        },
                                                      eventTrigger:new EvTrig()),
                               range: new CO.CORange(new List<Coor>() { new Coor(0, 0) }),
                               type: CO.Type.Weapon);

        bigMinePlacer.SetInt("nbrOfBigMines", 1);

        #endregion

        #region devour
        CO devour = new CO("devour", usable: true,
                            guiEvent: new COEvent(  eventAction: delegate (CO character, CO caller, CO target) { game.MoveCurrChDamageAndReposionToBase(target); },
                                                    eventTrigger:new EvTrig()),
                            range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(-1, 1), new Coor(-1, -1), new Coor(0, -1), new Coor(1, -1) }),
                            type: CO.Type.Weapon);

        devour.coEvents.Add(new COEvent(eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        devour.coEvents.Add(new COEvent(eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
                                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        #endregion

        #region catapult
        CO catapult = new CO("catapult", pickable: true,
                             guiEvent:new COEvent(name:"CatapultDmg", eventAction: delegate (CO character, CO caller, CO target) { game.DamageAndReposionToBase(target); },
                                                  eventTrigger: new EvTrig(stepDes:new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                            range: new CO.CORange(new List<Coor>() { new Coor(2, 2), new Coor(-2, 2), new Coor(2, 0), new Coor(0, 2), new Coor(-2, 0), new Coor(0, -2), new Coor(2, -2), new Coor(-2, -2) }),
                            type: CO.Type.Weapon);

        catapult.coEvents.Add(new COEvent(pickUpEvent));

        catapult.coEvents.Add(new COEvent(name:"DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                            eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));

        #endregion
        #region shotgun
        CO shotgun = new CO("shotgun", pickable: true,
                            guiEvent: new COEvent(name: "ShotgunDmg", eventAction: delegate (CO character, CO caller, CO target) {
                                Coor newCoor = target.currentField.coordinates + (target.currentField.coordinates - character.currentField.coordinates);
                                game.DamageAndRepositionToField(target, game.board[newCoor.i,newCoor.j]);
                            },
                                                  eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                            range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(-1, 1), new Coor(1, 0), new Coor(0, 1), new Coor(-1, 0), new Coor(0, -1), new Coor(1, -1), new Coor(-1, -1) }),
                            type: CO.Type.Weapon);
        shotgun.coEvents.Add(new COEvent(pickUpEvent));
        shotgun.coEvents.Add(new COEvent(name: "DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
                                            eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
        #endregion
        #region bowAndArrow
        CO bowAndArrow = new CO("bowandarrow",
                            /* endAction: delegate (CO character, CO caller, CO target)
                                  {
                                      Coor f = target.currentField;
                                  },*/
                            range: new CO.CORange(new List<Coor>() { new Coor(-2, 2), new Coor(2, -2), new Coor(-2, -2), new Coor(2, 2) }),
                            type: CO.Type.Character);

        #endregion


        #region laser
      CO laser = new CO("laser", pickable: true,
			   guiEvent:new COEvent(name:"LaserDmg", eventAction: delegate (CO character, CO caller, CO target) { game.DamageAndReposionToBase(target); },
						eventTrigger: new EvTrig(stepDes:new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
			  range: new CO.CORange(new List<Coor>() { new Coor(1, 1), new Coor(-1, 1),  new Coor(1, -1), new Coor(-1, -1) }),
			  type: CO.Type.Weapon);

      laser.coEvents.Add(new COEvent(pickUpEvent));

      laser.coEvents.Add(new COEvent(name:"DestroyItemOnUse", eventTrigger: new EvTrig(EvTrig.Type.UseItem),
					  eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }));
     #endregion

        #region crystal 

       

        CO crystal = new CO("crystal", type: CO.Type.Crystal);

	#endregion

        #region itemPlacer
        CO itemPlacer = new CO("ItemPlacer");
        itemPlacer.coEvents.Add(new COEvent(eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Start)),
                                                eventAction: delegate (CO character, CO caller, CO target) {
                                                    System.Random rnd = new System.Random();
                                                    if(rnd.NextDouble()>0.9f)game.board.TryRandomPlace(new CO(catapult));
                                                    if(rnd.NextDouble()>0.9f)game.board.TryRandomPlace(new CO(laser));
                                                    if (game.crystalsOnBoard < 3) { game.crystalsOnBoard++; game.board.TryRandomPlace(new CO(crystal)); }
                                                }));


        game.gameObjects.Add(itemPlacer);

        #endregion
        #endregion


        #region Characters
        CO zuti = new CO("Y");
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


        CO plavi = new CO("B", new int[] { 1, 1, 2, 3, 4, 6 }, blockFree: new List<CO.Type>() { CO.Type.Environment });


        CO crveni = new CO("R");
        crveni.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target) { caller.items.Add(devour); },
                                            eventTrigger: new EvTrig(roundDes: new EvTrig.Description(period: 3, pShift: 2),
                                                                        stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: 0))));


        CO crni = new CO("Bl", canStepOnPrevious: true,pickUpOnMaxStep:false);

        CO zeleni = new CO("G");

        zeleni.items.Add(minePlacer);
        zeleni.items.Add(bigMinePlacer);

        COEvent zeleniUlta = new COEvent(name: "GreenUlt",
            eventTrigger: new EvTrig(EvTrig.Type.FinishTurn),
            eventAction: delegate (CO character, CO caller, CO target) {

                CO ultaMine = new CO(bigMine);

                ultaMine.coEvents.Add(new COEvent(
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

        game.characters.Add(zeleni);
        game.characters.Add(crveni);
        game.characters.Add(crni);
        game.characters.Add(zuti);
        Board.Place(new CO(shotgun), game.board[5, 5]);
    }

}
