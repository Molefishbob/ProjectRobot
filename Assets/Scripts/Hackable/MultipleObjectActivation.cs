﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleObjectActivation : MonoBehaviour, IButtonInteraction
{
    [SerializeField]
    private List<MonoBehaviour> _consoles;
    [SerializeField]
    private List<MonoBehaviour> _targets;

    private void Start()
    {
        foreach(MonoBehaviour console in _consoles)
        {
            try
            {
                GenericHackable temp = (GenericHackable)console;
            }
            catch
            {
                Debug.Log("Consoles have to inherit GenericHackable script!");
            }
        }
        foreach (MonoBehaviour target in _targets)
        {
            try
            {
                IButtonInteraction temp = (IButtonInteraction)target;
            }
            catch
            {
                Debug.Log("Targets have to implement IButtonInteraction script!");
            }
        }
    }

    /// <summary>
    /// Called when a connected console is hacked
    /// 
    /// Checks if every console in the list of consoles have been hacked.
    /// If they are hacked the method calls every single targets ButtonDown method.
    /// </summary>
    public void ButtonDown()
    {
        bool hacked = true;
        foreach (MonoBehaviour console in _consoles)
        {
            if (console.GetComponent<GenericHackable>()._currentStatus != GenericHackable.Status.Hacked)
            {
                hacked = false;
                break;
            }
        }
        if (hacked)
        {
            foreach(MonoBehaviour target in _targets)
            {
                target.GetComponent<IButtonInteraction>().ButtonDown();
            }
        }
    }

    public void ButtonUp()
    {
        throw new System.NotImplementedException();
    }

}