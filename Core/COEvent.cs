using UnityEngine;
using System.Collections;

using CO = CoreObject;

public class COEvent
{ 
    public COAction         eventAction;
    public COEventTrigger   eventTrigger;

    public COEvent(COAction eventAction, COEventTrigger eventTrigger)
    {
        this.eventAction = eventAction;
        this.eventTrigger = new COEventTrigger(eventTrigger);
    }

    public COEvent(COEvent coEvent)
    {
        this.eventAction = coEvent.eventAction;
        this.eventTrigger = new COEventTrigger(coEvent.eventTrigger);
    }
}
