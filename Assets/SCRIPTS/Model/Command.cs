using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Command
{
    private string commandName;
    private string description;
    private int numberOfArguments;
    private string usageHelp;
    private bool isActive;
    private string methodToCall;


    public Command(string commandName, string description, int numberOfArguments, string usageHelp, bool isActive, string methodToCall)
    {
        this.commandName = commandName;
        this.description = description;
        this.numberOfArguments = numberOfArguments;
        this.usageHelp = usageHelp;
        this.isActive = isActive;
        this.methodToCall = methodToCall;
    }


    public string CommandName
    {
        get
        {
            return this.commandName;
        }
        set
        {
            commandName = value;
        }
    }

    public string Description
    {
        get
        {
            return this.description;
        }
        set
        {
            description = value;
        }
    }

    public int NumberOfArguments
    {
        get
        {
            return this.numberOfArguments;
        }
        set
        {
            numberOfArguments = value;
        }
    }

    public string UsageHelp
    {
        get
        {
            return this.usageHelp;
        }
        set
        {
            usageHelp = value;
        }
    }

    public bool IsActive
    {
        get
        {
            return this.isActive;
        }
        set
        {
            isActive = value;
        }
    }


    public string MethodToCall
    {
        get
        {
            return this.methodToCall;
        }
        set
        {
            methodToCall = value;
        }
    }
}

