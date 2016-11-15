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
        COEvent pickUpEvent = new COEvent(
            eventTrigger: new EvTrig(EvTrig.Type.Pickup),
            eventAction: delegate (CO character, CO caller, CO target) {

                caller.coEvents.Add(new COEvent(
                    eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start),
                                                roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { caller2.usable = true; }
                    ));

                caller.coEvents.Add(new COEvent(
                    eventTrigger: new EvTrig(type: EvTrig.Type.FinishTurn, roundDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: game.round + 1)),
                    eventAction: delegate (CO character2, CO caller2, CO target2) { game.DestroyCO(caller2); }
                    ));
            });
        #endregion


        #region wallAndwallPlacer
        CO wall = new CO("wall", block: true, type: CO.Type.Environment);
        wall.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target) { game.DestroyCO(caller); },
                                        eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start), depth: 2)));

        CO wallPlacer = new CO("wallPlacer");
        wallPlacer.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target) { if (character.previousField != null) caller.AddToEnvironment(new CO(wall), character.previousField); else Debug.Log("nowall"); },
                                                eventTrigger: new EvTrig(stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Range, start: 1))));


        #endregion

        #region minePlacer

        CO mine = new CO("mine", type: CO.Type.Weapon);
        mine.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target)
        {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target)==false)
            {
                game.DamageAndReposionToBase(target);
                caller.environmentOwner.SetInt("nbrOfMines", caller.environmentOwner.GetInt("nbrOfMines") + 1);
                game.DestroyCO(caller);
                   
            }

        

        }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));



        CO bigMine = new CO("bigMine", type: CO.Type.Weapon);
        bigMine.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target)
        {// Debug.Log(character.currentField.Print()+" "+ caller.currentField.Print()+" "+target.currentField.Print()); 
            if (caller.BelongsTo(target) == false)
            {
                game.DamageAndReposionToBase(target);
                game.DestroyCO(caller);

            }



        }, eventTrigger: new EvTrig(EvTrig.Type.StepOnField)));

        CO minePlacer = new CO("smallMinePlacer", usable: true,
                                 guiEvent: new COEvent(eventAction: delegate (CO character, CO caller, CO target) 
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
                                guiEvent: new COEvent(eventAction: delegate (CO character, CO caller, CO target)
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
                             guiEvent:new COEvent(eventAction: delegate (CO character, CO caller, CO target) { game.DamageAndReposionToBase(target); },
                                                  eventTrigger: new EvTrig(stepDes:new EvTrig.Description(EvTrig.Description.Type.Penultimate))),
                            range: new CO.CORange(new List<Coor>() { new Coor(2, 2), new Coor(-2, 2), new Coor(2, 0), new Coor(0, 2), new Coor(-2, 0), new Coor(0, -2), new Coor(2, -2), new Coor(-2, -2) }),
                            type: CO.Type.Weapon);

        catapult.coEvents.Add(new COEvent(pickUpEvent));

        catapult.coEvents.Add(new COEvent(eventTrigger: new EvTrig(EvTrig.Type.UseItem),
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

        #region itemPlacer
        CO itemPlacer = new CO("ItemPlacer");
        itemPlacer.coEvents.Add(new COEvent(eventTrigger: new EvTrig(stepDes: new EvTrig.Description(EvTrig.Description.Type.Start)),
                                                eventAction: delegate (CO character, CO caller, CO target) {
                                                    System.Random rnd = new System.Random();
                                                    if(rnd.NextDouble()>0.9f)game.board.TryRandomPlace(new CO(catapult));
                                                }));

        game.gameObjects.Add(itemPlacer);

        #endregion
        #endregion


        #region Characters
        CO zuti = new CO("Y");
        zuti.items.Add(wallPlacer);

        CO plavi = new CO("B", new int[] { 1, 1, 2, 3, 4, 6 }, blockFree: new List<CO.Type>() { CO.Type.Environment });


        CO crveni = new CO("R");
        crveni.coEvents.Add(new COEvent(eventAction: delegate (CO character, CO caller, CO target) { caller.items.Add(devour); },
                                            eventTrigger: new EvTrig(roundDes: new EvTrig.Description(period: 3, pShift: 2),
                                                                        stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Exact, exact: 0))));


        CO crni = new CO("Bl", canStepOnPrevious: true);

        CO zeleni = new CO("G");
        zeleni.items.Add(minePlacer);
        zeleni.items.Add(bigMinePlacer);



        #endregion

        game.characters.Add(zeleni);
        game.characters.Add(crveni);
        game.characters.Add(plavi);
        game.characters.Add(zuti);
        Board.Place(new CO(catapult), game.board[5, 5]);
    }
}
