namespace zpg
{
    /// <summary>
    /// The easiest solution - has a private list of RenderObjects.
    /// All objects are relevant.
    /// </summary>
    class ObjectsStoreDumb : IObjectsStore
    {
        private List<RenderObject> _objects;

        public ObjectsStoreDumb() => _objects = new();

        public void Add(RenderObject obj) => _objects.Add(obj);

        public List<RenderObject> GetAll() => _objects;

        public List<RenderObject> GetOnlyRelevant() => _objects;
    }
}
