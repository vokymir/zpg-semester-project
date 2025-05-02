namespace zpg
{
    /// <summary>
    /// Any entity has to have it's transformation matrix.
    /// </summary>
    class Entity
    {
        public Transform Transform { get; } = new Transform();
    }
}
