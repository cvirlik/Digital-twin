using System;
using System.Collections.Generic;

namespace Digital_twin.Dataset.Support.Actions
{
    public class ActionList
    {
        private LinkedList<Action> actions;
        private int maxSize;

        public ActionList(int size)
        {
            actions = new LinkedList<Action>();
            maxSize = size;
        }

        public int CurrentLenght()
        {
            return actions.Count;
        }

        public void AddAction(Action action)
        {
            if (actions.Count == maxSize)
            {
                actions.RemoveFirst();
            }
            actions.AddLast(action);
        }

        public void DeleteAction()
        {
            if (actions.Count > 0)
            {
                Action action = actions.Last.Value;
                action.Undo();
                actions.RemoveLast();
            }
        }
    }
}

