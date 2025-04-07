
namespace zpg
{
    // propably is redundant?
    // I am not sure, wanted to be more flexible, but probably won't be of any use.
    // It won't hurt to inherit from it right now.
    public class CollisionBody
    {
        public virtual bool DoesCollide(CollisionBody other)
        {
            return true;
        }
    }
}
