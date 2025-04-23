using OpenTK.Mathematics;

namespace zpg
{
    public class CollisionCube : CollisionBody
    {
        public Vector3 Center { get; set; }
        public float Xover2 { get; set; }
        public float Yover2 { get; set; }
        public float Zover2 { get; set; }
        public bool IsActive { get; set; } = true;

        public bool DoesCollide(CollisionCube other)
        {
            if (!IsActive) return false;

            bool xOverlap = Math.Abs(Center.X - other.Center.X) < (Xover2 + other.Xover2);
            bool yOverlap = Math.Abs(Center.Y - other.Center.Y) < (Yover2 + other.Yover2);
            bool zOverlap = Math.Abs(Center.Z - other.Center.Z) < (Zover2 + other.Zover2);

            return (xOverlap && yOverlap && zOverlap);
        }

        public bool IsAbove(CollisionCube other)
        {
            float epsilon = 0.02f;
            bool xOverlap = Math.Abs(Center.X - other.Center.X) < (Xover2 + other.Xover2);
            bool zOverlap = Math.Abs(Center.Z - other.Center.Z) < (Zover2 + other.Zover2);
            bool isHigher = Center.Y - Yover2 + epsilon >= other.Center.Y + other.Yover2;

            return (xOverlap && zOverlap && isHigher);
        }

        public float Distance(CollisionCube other)
        {
            float distanceX = Center.X + Xover2 - (other.Center.X + other.Xover2);
            float distanceY = Center.Y + Yover2 - (other.Center.Y + other.Yover2);
            float distanceZ = Center.Z + Zover2 - (other.Center.Z + other.Zover2);

            return (float)Math.Sqrt(
                    distanceX * distanceX +
                    distanceY * distanceY +
                    distanceZ * distanceZ
                    );
        }

        public override string ToString()
        {
            return $"center {Center} width {Xover2} depth {Zover2} height {Yover2}";
        }
    }
}
