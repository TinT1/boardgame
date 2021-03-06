﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using CO = CoreObject;

public delegate void COAction(CoreObject character = null, CoreObject caller = null, CoreObject target = null);

public partial class CoreObject
{
    public static readonly List<CoreObject> created = new List<CoreObject>();
    public Field currentField;
    public Field previousField;
    public Field baseField;

    public Type type;
    public string name;
    public int[] stepPattern;
    public bool pickUpOnMaxStep;
    public int health;
    public int crystals;
    public int lastCrystalTurn;
    public int crystalUltaTreshold;
    public COEvent ulta;

    public COEvent guiEvent;

    public List<COEvent> coEvents;

    public bool block;
    public List<Type> blockFree;

    public bool usable;
    public bool pickable;
    public bool publicVisibility;
    public bool canStepOnPrevious;

    public CORange range;

    public List<CO> items = new List<CO>();
    public CO equipedItem;

    public List<CO> environment = new List<CO>();
    public bool gratisStep = false;
    public List<Field.Coordinates> stepHistory = new List<Field.Coordinates>();
    public void ClearStepHistory() { stepHistory.Clear(); gratisStep = true; }
    public bool GratisStep() {
        int l = stepHistory.Count;
        bool check  = 2 < stepHistory.Count && Field.Coordinates.IsEqual(stepHistory[l - 1],stepHistory[l - 2]) && Field.Coordinates.IsEqual(stepHistory[l - 1], stepHistory[l - 3]);
        if (check && gratisStep)
        {
            gratisStep = false;
            return true;
        }
        return false;
    }
    public string PrintStepHistory(){
      string res="";
      stepHistory.ForEach(x => res +=  x.Print());
      return res;
    }


    public CO environmentOwner;

    public bool BelongsTo(CO target)
    {
        return (    environmentOwner!=null && (target == environmentOwner || environmentOwner.BelongsTo(target))
                ||  target.items.Contains(this));
    }

    public void AddToEnvironment(CO co, Field field)
    {
        Board.Place(co, field);
        Own(co);
    }

    public void Own(CO co)
    {
        co.environmentOwner = this;
        environment.Add(co);
    }

    public bool IsVisibleTo(CO co)
    {
    return this.publicVisibility || this == co || this.BelongsTo(co);
    }

    private int[] defaultStepPattern = new int[] { 1, 2, 3, 4, 5, 6 };

    Dictionary<String, COData> additionalVars = new Dictionary<string, COData>();

    public int  GetInt(String key)          { return additionalVars[key]._int;  }
    public void SetInt(string key,int val)  { if (!additionalVars.ContainsKey(key)) additionalVars[key] = new COData();
                                              additionalVars[key].SetInt(val); }

    #region constructors

    public CoreObject(string name,
                        int[] stepPattern = null,
                        bool pickUpOnMaxStep = true,
                        int health = 4,
                        int crystals = 0,
                        int lastCrystalTurn = -1,
                        int crystalUltaTreshold = 2,
                        COEvent ulta = null,
                        COEvent guiEvent =null,
                        List<COEvent> eventAction = null,
                        bool block = false,
                        List<Type> blockFree = null,
                        bool usable = false,
                        bool publicVisibility = true,
                        bool pickable = false,
                        bool canStepOnPrevious = false,
                        CORange range = null,
                        Type type = Type.Character)
    {
        this.name = name;
        if (stepPattern == null) this.stepPattern = defaultStepPattern; else this.stepPattern = stepPattern;
        this.pickUpOnMaxStep = pickUpOnMaxStep;
        this.health = health;
        this.crystals = crystals;
        this.crystalUltaTreshold = crystalUltaTreshold;
        this.lastCrystalTurn = lastCrystalTurn;
        if (ulta!= null) this.ulta= new COEvent(ulta);
        if (guiEvent!=null)this.guiEvent = new COEvent(guiEvent);

        if (coEvents == null) this.coEvents = new List<COEvent>();
        else { this.coEvents = new List<COEvent>(); coEvents.ForEach(coEvent => this.coEvents.Add(new COEvent(coEvent))); }

        this.block = block;
        if (blockFree == null) this.blockFree = new List<Type>(); else this.blockFree = blockFree;

        this.usable = usable;
        this.pickable = pickable;
        this.publicVisibility = publicVisibility;

        this.canStepOnPrevious = canStepOnPrevious;
        if (range == null) this.range = new CORange(); else this.range = range;
        this.type = type;

        CoreObject.created.Add(this);

    }

    public CoreObject(CO co)
    {
        this.name = co.name;
        if (stepPattern == null) this.stepPattern = defaultStepPattern; else this.stepPattern = co.stepPattern;
        this.health = co.health;
        this.crystals = co.crystals;
        this.crystalUltaTreshold = co.crystalUltaTreshold;
        this.lastCrystalTurn = co.lastCrystalTurn;
        if (co.ulta!= null) this.ulta= new COEvent(co.ulta);
        this.pickUpOnMaxStep = co.pickUpOnMaxStep;
        if (co.guiEvent != null) this.guiEvent = new COEvent(co.guiEvent);

        if (co.coEvents == null) this.coEvents = new List<COEvent>();
        else { this.coEvents = new List<COEvent>(); co.coEvents.ForEach(coEvent => this.coEvents.Add(new COEvent(coEvent))); }

        this.block = co.block;
        if (blockFree == null) this.blockFree = new List<Type>(); else this.blockFree = new List<Type> (co.blockFree);

        this.usable = co.usable;
        this.pickable = co.pickable;
        this.publicVisibility = co.publicVisibility;
        this.canStepOnPrevious = co.canStepOnPrevious;

        if (co.range == null) this.range = new CORange();
        else this.range = new CORange(co.range);

        this.type = co.type;
    }

    #endregion

    public void Equip(CO item)
    { equipedItem = item; }

    public void UnEquip()
    { equipedItem = null; }

    public enum State { Move, UseItem };
    public State GetState { get { return equipedItem == null ? State.Move : State.UseItem; } }

    public void ExecEventAction(COEventTrigger.Type eventType,int round, int step, int maxStep,CO character, CO coTriggerer ,CO target=null,int depth = 0)
    {
        for (int i = this.environment.Count - 1; i >= 0; --i)
        {
           // Debug.Log("ExecEventAction(" + eventType+" this:"+this.name);
            this.environment[i].ExecEventAction(eventType, round, step, maxStep, character, coTriggerer, target,depth+1);
        }

        if(coTriggerer==null || coTriggerer == this)
        for(int evIndex = this.coEvents.Count-1;evIndex>=0;--evIndex)
            {
                COEvent coEvent = this.coEvents[evIndex];
                if (coEvent.eventTrigger.IsTriggered(eventType, round, step, maxStep,depth)) coEvent.eventAction(character, this, target);
            }

        for (int i = this.items.Count - 1; i >= 0; --i)
            this.items[i].ExecEventAction(eventType, round, step, maxStep, character, coTriggerer,target,depth+1);
    }
}

public partial class CoreObject
{
    public enum Type { Null,Character, Weapon, Utility, Environment, Crystal };
}

public partial class CoreObject
{
    public class CORange
    {
        public enum Mode { Relative,Absolute};

        public List<Field.Coordinates> positions;
        readonly Mode mode;

        public CORange(List<Field.Coordinates> positions=null,Mode mode=Mode.Relative)
        {
            if (positions == null) this.positions = new List<Field.Coordinates>(); else  this.positions= positions;
            this.mode = mode;
        }
        public CORange(CORange coRange)
        {
            this.mode = coRange.mode;
            this.positions = new List<Field.Coordinates>();
            if (coRange.positions != null) coRange.positions.ForEach(coor => this.positions.Add(new Field.Coordinates(coor)));
        }
    }


}
