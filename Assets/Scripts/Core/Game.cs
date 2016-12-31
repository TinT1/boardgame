using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.ComponentModel;

using CO = CoreObject;
using Coor = Field.Coordinates;

using EvTrig = COEventTrigger ;

// dodati buildera, 
public class Game
{
    public List<CO>     characters      = new List<CO>();
    List<CO>            deadCharacters  = new List<CO>();

    public CO currCh;
    public int turn;
    public int round;
    public Board board =new Board();


    public List<CO> gameObjects  = new List<CO>();

    public int crystalsOnBoard = 0;
    public int step;
    public int maxStep; 
    public int dice;


	public Game ()
    {
        GameInitialization.Initialize(this);
                
        StartGame();
    }

    public void StartGame()
    {
        //init
        Board.Place(characters[0],board[0,0]);
        Board.Place(characters[1], board[Board.n-1, 0]);
        Board.Place(characters[3], board[0, Board.n - 1]);
        Board.Place(characters[2], board[Board.n - 1, Board.n - 1]);

        foreach (CO character in characters) character.baseField = character.currentField;
        
        StartMainLoop();
    }

    private void StartMainLoop()
    {
        turn = -1;
        NextPlayer();
    }

    private void NextPlayer()
    {
 
        ++turn;
        round = turn / characters.Count;

        currCh = characters[turn % characters.Count];
        if(deadCharacters.Contains(currCh)){ NextPlayer(); return;}

        step = 0;

        System.Random rand = new System.Random();
        dice = rand.Next(0, currCh.stepPattern.Length - 1);
        maxStep = currCh.stepPattern[dice];
        ExecEventAction();
    }

    #region actions
   
    
    private void ExecEventAction(EvTrig.Type type=EvTrig.Type.Default, CO coTriggerer=null)
    {
        if (type == EvTrig.Type.StepOnField)
        {
            //Debug.Log(currCh.currentField.Print());
            Field currentField = currCh.currentField;
            CO currentCharacter = currCh;
            for (int i = currCh.currentField.fieldObjects.Count - 1; i >= 0; --i)
            {
              //  Debug.Log(currentField.fieldObjects[i].name);
                if (currentCharacter != currCh) return;
                currentField.fieldObjects[i].ExecEventAction(EvTrig.Type.StepOnField, round, step, maxStep, currCh, coTriggerer, currCh);
              
            }
        }
        else
        {
            for (int i = gameObjects.Count - 1; i >= 0; --i)
                gameObjects[i].ExecEventAction(type, round, step, maxStep, currCh, coTriggerer);

            currCh.ExecEventAction(type, round, step, maxStep, currCh, coTriggerer);           
        }
            
        
    }

    #endregion

    #region Behaviour
    public void FinishTurn()                            {   ExecEventAction(EvTrig.Type.FinishTurn);
                                                            NextPlayer();  }


    public void Damage(CoreObject target)
    {
        --target.health;
        bool finishTurn = false;
        if (target == currCh) finishTurn = true;
        if (target.health == 0) DestroyCO(target);
        if (finishTurn) FinishTurn();
    }

    public void DamageAndReposionToBase(CoreObject target)
    {
        Board.Move(target, target.baseField);
        Damage(target);
    }

    public void DamageAndRepositionToField(CoreObject target, Field f)
    {
        foreach (CO CObject in f.fieldObjects)
        {
            if (CObject.name == "wall") f = target.currentField;
            if (CObject.type == CO.Type.Character) this.DamageAndReposionToBase(CObject);
            break;
        }
        Board.Move(target, f);
        
        Damage(target);
    }

    public void MoveCurrChDamageAndReposionToBase(CoreObject target)
    {
        Field f = target.currentField;
        Board.Move(target, target.baseField);
        Damage(target);
        MoveCurrentCharacter(f);
    }

    public void DestroyCO (CoreObject co)
    {
        if (co.environmentOwner != null)
        {
            co.environmentOwner.environment.Remove(co);
        }
        if(Board.Contains(co)) Board.Remove(co);

        currCh.items.Remove(co);
        if(currCh.equipedItem!=null && currCh.equipedItem.Equals(co)) currCh.UnEquip();

        if (characters.Contains(co)) deadCharacters.Add(co);
    }

    public bool CanMove(CO character, Field field)
    {
        if (character.range.positions.TrueForAll(relativeCoos => Coor.IsEqual(character.currentField.coordinates + relativeCoos,field.coordinates)== false )) return false;

        foreach (CO target in field.fieldObjects)
            if (target.type == CoreObject.Type.Character && character.blockFree.Contains(CoreObject.Type.Character) == false) return false;
            else if (target.type == CoreObject.Type.Environment && character.blockFree.Contains(CoreObject.Type.Environment) == false) return false;

        if (character.previousField == field && !(step == 0)) return false;
        return true;
    }


    public bool CanCurrentCharacterMove(Field field)    { return CanMove(currCh, field) && HasFreeSteps();      }
    private bool HasFreeSteps()                         { return step < maxStep;                                }
    public void MoveCurrentCharacter(Field field)       { ++step; Board.Move(currCh, field);
                                                            ExecEventAction();
                                                            ExecEventAction(EvTrig.Type.StepOnField);           }


    
    public void UseItem(CO character,Field field)
    {
        character.equipedItem.guiEvent.eventAction(character, character.equipedItem, field.fieldObjects.Find(x => x.type == CO.Type.Character));
        ExecEventAction(EvTrig.Type.UseItem, character.equipedItem);
        character.UnEquip(); 
    }
    public void CurrentCharacterUseItem(Field field)     { UseItem(currCh, field); }
  
    public bool CanUseItem(CO attacker, Field field)
    {
        if (attacker.equipedItem == null) return false;
        foreach (Field.Coordinates c in attacker.equipedItem.range.positions)
            if (Field.Coordinates.IsEqual(field.coordinates-attacker.currentField.coordinates, c)) return true;
        return false;
    }
    public bool CanCurrentCharacterUseItem(Field field)  { return CanUseItem(currCh, field); }

    #endregion



    public void PickUp(CO item) 
    {
        currCh.items.Add(item);
        Board.Remove(item);
        ExecEventAction(EvTrig.Type.Pickup, item);
    }
    public void CollectCrystal(CO item)
    {
        crystalsOnBoard--;
        currCh.crystals++;
        currCh.lastCrystalTurn = turn;
        Board.Remove(item);

        if (currCh.crystalUltaTreshold <= currCh.crystals)
        {
            currCh.crystals = 0;
            currCh.coEvents.Add(currCh.ulta);
        }
    }



    public bool CanEquip(CO item) { return item.usable &&
                                           item.guiEvent.eventTrigger.IsTriggered(EvTrig.Type.Default,round,step,maxStep,0);     }

    public bool CanPickUp(CO item) {
        if (item.pickable && (!currCh.pickUpOnMaxStep || step == maxStep)) return item.pickable; else return false;
    }
    public bool CanFinishTurn() { return false; }

    public bool CanCollectCrystal(CO item)
    {
        return turn != currCh.lastCrystalTurn && item.type == CoreObject.Type.Crystal;
    }

    public void StealItem(CO to,CO from)
    {
       CO item = RemoveItemAndAddReturnItemEvent(from,to);
       if (item != null) AddItem(item,to);
    }

    public CO RemoveItemAndAddReturnItemEvent(CO from,CO tempOwner)
    {
       if(from.items.Count==0) return null ;
       CO item = from.items[from.items.Count-1];
       from.items.RemoveAt(from.items.Count-1);
       from.coEvents.Add(new COEvent(name: "ReturnItem", 
          eventTrigger: new EvTrig( stepDes: new EvTrig.Description(type: EvTrig.Description.Type.Start)), 
          eventAction: delegate (CO character2, CO caller2, CO target2) { 
              if(tempOwner!=null)tempOwner.items.Remove(item); 
              character2.items.Add(item);
            }));
       return item;
    }
   // jos trebamo dodati event crnome da izgubi item tj to treba biti u ovom returnu 
    public void AddItem(CO item,CO to)
    {
//      CO itemCopy = new CO(item);
      CO itemCopy = item;
 
      itemCopy.usable=true;
      to.items.Add(itemCopy);

      
    }


}
