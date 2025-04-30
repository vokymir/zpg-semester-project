using OpenTK.Mathematics;

namespace zpg
{
    public class CollisionCube : CollisionBody
    {
        public Vector3 Center { get; set; }
        // all are over 2 because you calculate from center
        public float Xover2 { get; set; }
        public float Yover2 { get; set; }
        public float Zover2 { get; set; }
        // this is just because of WhiteScreen, which is not active
        public bool IsActive { get; set; } = true;

        public bool DoesCollide(CollisionCube other)
        {
            if (!IsActive) return false;

            bool xOverlap = Math.Abs(Center.X - other.Center.X) < (Xover2 + other.Xover2);
            bool yOverlap = Math.Abs(Center.Y - other.Center.Y) < (Yover2 + other.Yover2);
            bool zOverlap = Math.Abs(Center.Z - other.Center.Z) < (Zover2 + other.Zover2);

            return (xOverlap && yOverlap && zOverlap);
        }

        /// <summary>
        /// Is this at higher place than other object, and also on the same tile X-Z wise?
        /// </summary>
        public bool IsAbove(CollisionCube other)
        {
            float epsilon = 0.02f;
            bool xOverlap = Math.Abs(Center.X - other.Center.X) < (Xover2 + other.Xover2);
            bool zOverlap = Math.Abs(Center.Z - other.Center.Z) < (Zover2 + other.Zover2);
            bool isHigher = Center.Y - Yover2 + epsilon >= other.Center.Y + other.Yover2;

            return (xOverlap && zOverlap && isHigher);
        }

        /// <summary>
        /// Is directly on top of other object?
        /// Meaning, is this standing on other?
        /// </summary>
        public bool IsOnTop(CollisionCube other)
        {
            float epsilon = 0.1f;
            return IsAbove(other) && Distance(other) < Yover2 * 2 + other.Yover2 + epsilon;
        }

        /// <summary>
        /// Eucleidian distance from nearest points on both CollisionCubes.
        /// </summary>
        public float Distance(CollisionCube other)
        {
            // direction from this to other
            Vector3 dir = other.Center - Center;
            // if X is positive, it means the other object is more on right
            // so we should use this objects rightmost edge and leftmost others
            // the same for YZ
            float distanceX = Center.X + (dir.X >= 0 ? 1 : -1) * Xover2 - (other.Center.X + (dir.X >= 0 ? -1 : 1) * other.Xover2);
            float distanceY = Center.Y + (dir.Y >= 0 ? 1 : -1) * Yover2 - (other.Center.Y + (dir.Y >= 0 ? -1 : 1) * other.Yover2);
            float distanceZ = Center.Z + (dir.Z >= 0 ? 1 : -1) * Zover2 - (other.Center.Z + (dir.Z >= 0 ? -1 : 1) * other.Zover2);

            return (float)Math.Sqrt(
                    distanceX * distanceX +
                    distanceY * distanceY +
                    distanceZ * distanceZ
                    );
        }
    }
}
