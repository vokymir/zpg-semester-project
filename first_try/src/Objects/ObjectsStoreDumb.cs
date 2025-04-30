namespace zpg
{
    class ObjectsStoreDumb : IObjectsStore
    {
        private List<RenderObject> _objects;

        public ObjectsStoreDumb()
        {
            _objects = new();
        }

        public void Add(RenderObject obj)
        {
            _objects.Add(obj);
        }

        public List<RenderObject> GetAll()
        {
            return _objects;
        }

        public List<RenderObject> GetOnlyRelevant()
        {
            return _objects;
        }
    }
}
