using UnityEngine;
using System.Collections;

using CO = CoreObject;

public class COEvent
{
    public COAction         eventAction;
    public COEventTrigger   eventTrigger;
    public string name;

    public COEvent(COAction eventAction, COEventTrigger eventTrigger,string name="defEvName")
    {
        this.eventAction = eventAction;
        this.eventTrigger = new COEventTrigger(eventTrigger);
        this.name = name;
    }

    public COEvent(COEvent coEvent)
    {
        this.eventAction = coEvent.eventAction;
        this.eventTrigger = new COEventTrigger(coEvent.eventTrigger);
        this.name = coEvent.name;
    }

    public string Print() { return name; }
}
