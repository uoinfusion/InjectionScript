namespace InjectionScript.Runtime.State
{
    public class Equip
    {
        public int Layer { get; }
        public int Id { get; }

        public Equip(int layer, int id)
        {
            Layer = layer;
            Id = id;
        }
    }
}
