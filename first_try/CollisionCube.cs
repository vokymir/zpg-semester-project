using OpenTK.Mathematics;

namespace zpg
{
    public class CollisionCube : CollisionBody
    {
        public Vector3 Center { get; set; }
        public float Xover2 { get; set; }
        public float Yover2 { get; set; }
        public float Zover2 { get; set; }

        public bool DoesCollide(CollisionCube other)
        {
            bool xOverlap = Math.Abs(Center.X - other.Center.X) < (Xover2 + other.Xover2);
            bool yOverlap = Math.Abs(Center.Y - other.Center.Y) < (Yover2 + other.Yover2);
            bool zOverlap = Math.Abs(Center.Z - other.Center.Z) < (Zover2 + other.Zover2);

            if (xOverlap && yOverlap && zOverlap)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"center {Center} width {Xover2} depth {Zover2} height {Yover2}";
        }
    }
}
