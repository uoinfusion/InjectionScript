using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InjectionScript.Runtime.State
{
    public class EquipSet
    {
        private List<Equip> equips;

        public IEnumerable<Equip> Equips
        {
            get => equips;
            set => equips = new List<Equip>(value ?? Enumerable.Empty<Equip>());
        }

        private EquipSet()
        {
        }

        public EquipSet(params Equip[] equips)
        {
            this.equips = new List<Equip>(equips?.Where(x => x != null) ?? Array.Empty<Equip>());
        }

        public EquipSet(IEnumerable<Equip> equips)
        {
            this.equips = new List<Equip>(equips);
        }

        public int GetAtLayer(int layer) => equips.FirstOrDefault(x => x.Layer == layer)?.Id ?? 0;
    }
}
