using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AuraPresetsEnum
{
    flu,
    sprainedLeg,
}

public static class AuraPresets
{
    public static Aura Flu()
    {
        Aura a = new Aura(Debufftypes.Disease, Statuses.ill);
        a.SetSpreadable(0.01f, false);
        a.SetTimed(30);
        a.AddValueChange(VariableModifiers.Health, -3);
        a.AddValueChange(VariableModifiers.Workspeed, -.2f);
        a.AddValueChange(VariableModifiers.Walkspeed, -.2f);
        return a;
    }

    public static Aura SprainedLeg()
    {
        Aura a = new Aura(Debufftypes.Injury, Statuses.injured);
        a.SetBehaveAsTreatedWound(120);
        a.AddValueChange(VariableModifiers.Workspeed, -.2f);
        a.AddValueChange(VariableModifiers.Walkspeed, -.6f);
        a.AddValueChange(VariableModifiers.Health, -30);

        return a;
    }
    public static Aura Sad()
    {
        Aura a = new Aura(Debufftypes.Mood, Statuses.sad);
        a.SetTimed(120);
        a.AddValueChange(VariableModifiers.Workspeed, -.2f);
        a.AddValueChange(VariableModifiers.Walkspeed, -.1f);
        return a;
    }
    public static Aura Happy()
    {
        Aura a = new Aura(Debufftypes.Mood, Statuses.happy);
        a.SetTimed(120);
        a.AddValueChange(VariableModifiers.Workspeed, .2f);
        a.AddValueChange(VariableModifiers.Walkspeed, .1f);
        return a;
    }
}
