using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using CO = CoreObject;

public delegate void COAction(CoreObject character = null, CoreObject caller = null, CoreObject target = null);

//*

public partial class CoreObject
{
    public Field currentField;
    public Field previousField;
    public Field baseField;

    public Type type;
    public string name;
    public int[] stepPattern;

    public int health;



    public COEvent guiEvent;

    public List<COEvent> coEvents;

    public bool block;
    public List<Type> blockFree;

    public bool usable;
    public bool pickable;

    public bool canStepOnPrevious;

    public CORange range;


    //---------------

    public List<CO> items = new List<CO>();
    public CO equipedItem;


    public List<CO> environment = new List<CO>();

    public CO environmentOwner;

    public bool BelongsTo(CO target)
    {
        return (    environmentOwner!=null && (target == environmentOwner || environmentOwner.BelongsTo(target))
                ||  target.items.Contains(this));
    }

    public void AddToEnvironment(CO co, Field field)
    {
        //        Debug.Log(co.name + field.coordinates.i + field.coordinates.j);
        Board.Place(co, field);

        co.environmentOwner = this;
        environment.Add(co);
    }

    private int[] defaultStepPattern = new int[] { 1, 2, 3, 4, 5, 6 };

    Dictionary<String, COData> additionalVars = new Dictionary<string, COData>();

    public int  GetInt(String key)          { return additionalVars[key]._int;  }
    public void SetInt(string key,int val)  { if (additionalVars.ContainsKey(key) == false) additionalVars[key] = new COData();
                                              additionalVars[key].SetInt(val); }

    #region constructors

    public CoreObject(string name,
                        int[] stepPattern = null,
                        int health = 2,
                        COEvent guiEvent=null,
                        List<COEvent> eventAction = null,
                        bool block = false,
                        List<Type> blockFree = null,
                        bool usable = false,
                        bool pickable = false,
                        bool canStepOnPrevious = false,
                        CORange range = null,
                        Type type = Type.Character)
    {
        this.name = name;
        if (stepPattern == null) this.stepPattern = defaultStepPattern; else this.stepPattern = stepPattern;
        this.health = health;

        if(guiEvent!=null)this.guiEvent = new COEvent(guiEvent);

        if (coEvents == null) this.coEvents = new List<COEvent>();
        else { this.coEvents = new List<COEvent>(); coEvents.ForEach(coEvent => this.coEvents.Add(new COEvent(coEvent))); }

        this.block = block;
        if (blockFree == null) this.blockFree = new List<Type>(); else this.blockFree = blockFree;

        this.usable = usable;
        this.pickable = pickable;

        this.canStepOnPrevious = canStepOnPrevious;
        if (range == null) this.range = new CORange(); else this.range = range;
        this.type = type;

    }

    public CoreObject(CO co)
    {
       
        this.name = co.name;
        if (stepPattern == null) this.stepPattern = defaultStepPattern; else this.stepPattern = co.stepPattern;
        this.health = co.health;

        if (co.guiEvent != null) this.guiEvent = new COEvent(co.guiEvent);

        if (co.coEvents == null) this.coEvents = new List<COEvent>();
        else { this.coEvents = new List<COEvent>(); co.coEvents.ForEach(coEvent => this.coEvents.Add(new COEvent(coEvent))); }

        this.block = co.block;
        if (blockFree == null) this.blockFree = new List<Type>(); else this.blockFree = new List<Type> (co.blockFree);

        this.usable = co.usable;
        this.pickable = co.pickable;

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
        
       // Debug.Log("CO:" + this.name + " triggerer:" + (coTriggerer == null ? "e" : coTriggerer.name)+ (coTriggerer == null || coTriggerer == this));
       
        for (int i = this.environment.Count - 1; i >= 0; --i)
        {
           // Debug.Log("ExecEventAction(" + eventType+" this:"+this.name);
            this.environment[i].ExecEventAction(eventType, round, step, maxStep, character, coTriggerer, target,depth+1);
        }


        if(coTriggerer==null || coTriggerer == this)
        this.coEvents.ForEach(coEvent => 
            {
                if (coEvent.eventTrigger.IsTriggered(eventType, round, step, maxStep,depth)) coEvent.eventAction(character, this, target);
            });
        


        for (int i = this.items.Count - 1; i >= 0; --i)
            this.items[i].ExecEventAction(eventType, round, step, maxStep, character, coTriggerer,target,depth+1);

    }

}












public partial class CoreObject
{
    public enum Type { Null,Character, Weapon, Utility, Environment };
}

public partial class CoreObject
{
    public class CORange
    {
        public enum Mode { Relative,Absolute};

        public List<Field.Coordinates> positions;
        Mode mode;

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
