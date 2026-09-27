using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BepInPriority : Attribute
{
    public const int Low = 0;
    public const int Default = 1;
    public const int High = 2;

    internal readonly int priority = Default;

    public BepInPriority() { }
    public BepInPriority(int priority) => this.priority = priority;
}
