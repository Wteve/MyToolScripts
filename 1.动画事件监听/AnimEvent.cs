using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour {

    public Action<string> stringAction;
    public Action<int> intAction;

    public void AddMsg(Action<string> _stringAction) {
        stringAction += _stringAction;
    }

    public void ClearMsg(Action<string> _stringAction) {
        stringAction -= _stringAction;
    }

    public void StringEvent(string eventString) {
        stringAction?.Invoke(eventString);
    }

    public void IntEvent(int eventValue) {
        intAction?.Invoke(eventValue);
    }
}
