using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Runtime.State
{
    public sealed class InjectionRuntimeState
    {
        public ArmSets ArmSets { get; } = new ArmSets();
        public DressSets DressSets { get; } = new DressSets();
        public Objects Objects { get; } = new Objects();
        public Globals Globals { get; } = new Globals();

        public int DressSpeed { get; set; }
        public int UseEquipForDress { get; set; }
    }
}
