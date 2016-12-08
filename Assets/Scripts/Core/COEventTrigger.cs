using UnityEngine;
using System.Collections;
//novo je StepRange

// proširiti timeinfo na eventinfo

using CO = CoreObject;

public class COEventTrigger
{

    public class Description
    {
        public enum Type { Exact, Penultimate, Periodic, Range,Start,Finish }

        public Type type;
        public int period;
        public int pShift;
        public int exact;
        public int start;
        public int last;

        public Description(Type type=Type.Periodic, int period=1,int pShift = 0, int exact=-1, int start=0, int last = -1)
        {
            this.type = type;
            this.period = period;
            this.pShift = pShift;
            this.exact = exact;
            this.start = start;
            this.last = last;
        }

        public Description(Description des)
        {
            this.type = des.type;
            this.period = des.period;
            this.pShift = des.pShift;
            this.exact = des.exact;
            this.start = des.start;
            this.last = des.last;
        }

        public bool IsTriggered(int i, int max)
        {
            return
                        type == Type.Periodic       && i % period  == pShift   
                    ||  type == Type.Start          && i==0                
                    ||  type == Type.Finish         && i == max                 
                    ||  type == Type.Penultimate    && i == max                
                    ||  type == Type.Exact          && (i == exact || (exact == -1 && i == max)) 
                    ||  type == Type.Range          && (i >= start && (i <= last || last == -1));

        }
    }

    public enum Type { Default, FinishTurn , UseItem , Pickup, StepOnField}
    
    public Type type;
    public Description stepDes;
    public Description roundDes;
    int depth;
    
    public COEventTrigger(Type type=Type.Default,Description roundDes=null,Description stepDes=null, int depth = -1)
    {
        
        this.type = type;
        if (roundDes != null) this.roundDes = new Description(roundDes); else this.roundDes = new Description();
        if (stepDes != null) this.stepDes = new Description(stepDes); else this.stepDes = new Description();
        if (depth == -1)
        {
           if(type==Type.StepOnField) this.depth = 0;
           else this.depth = 1;
        }
        else this.depth = depth;
    }

    public COEventTrigger(COEventTrigger coEventTrigger)
    {
        this.type = coEventTrigger.type;
        this.roundDes = new Description(coEventTrigger.roundDes);
        this.stepDes = new Description(coEventTrigger.stepDes);
        this.depth = coEventTrigger.depth;
    }

    public bool IsTriggered(Type type, int round, int step, int maxStep,int depth)
    {
        //Debug.Log("IsTriggered(" + type.ToString());
        return      this.type==type 
                &&  depth<=this.depth
                &&  roundDes.IsTriggered(round, -1) 
                &&  stepDes.IsTriggered(step, maxStep);
    }

    public string Print() { return "TriggerPrint(replace)"; }
}

