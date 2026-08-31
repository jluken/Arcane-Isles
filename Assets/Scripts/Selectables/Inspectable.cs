using System.Collections.Generic;
using UnityEngine;

public class Inspectable : Selectable
{
    public SelectionData deepInspect;

    public string inspectConv;

    public override void Awake()
    {
        deepInspect = new SelectionData(this)
        {
            actionName = "Examine",
            setSelect = true,
            interaction = new DeepInspect(inspectConv)
        };
        base.Awake();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>();
        acts.Add(deepInspect);
        acts.Add(inspectSelection);
        acts.Add(goHere);
        return acts;
    }

    public class DeepInspect : Interaction
    {
        private string convName;
        public DeepInspect(string convName)
        {
            this.convName = convName;
        }

        public override void Interact(Character npc, Selectable interactable)
        {
            DialogueInterface.Instance.StartPlayerObjConversation(convName, npc, interactable);
        }
    }
}
