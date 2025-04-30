namespace zpg
{
    class ObjectsStoreGrid : IObjectsStore
    {
        private Dictionary<(int, int, int), List<RenderObject>> _objects;
        public (int X, int Y, int Z) Grid { get; set; } = (3, 3, 3);
        private Camera _camera;

        public ObjectsStoreGrid(Camera camera)
        {
            _camera = camera;
            _objects = new();
        }

        public void Add(RenderObject obj)
        {
            int x = (int)obj.Transform.Position.X;
            int y = (int)obj.Transform.Position.Y;
            int z = (int)obj.Transform.Position.Z;

            // normalize to grid coordinates
            var key = (x / Grid.X, y / Grid.Y, z / Grid.Z);

            if (_objects.ContainsKey(key))
                _objects[key].Add(obj);
            else
                _objects[key] = new List<RenderObject>(Grid.X * Grid.Y * Grid.Z) { obj };
        }

        public List<RenderObject> GetAll()
        {
            List<RenderObject> all = new List<RenderObject>(_objects.Count * Grid.X * Grid.Y * Grid.Z);

            foreach (var (k, v) in _objects)
            {
                all.AddRange(v);
            }

            return all;
        }

        public List<RenderObject> GetOnlyRelevant()
        {
            (int, int, int) playerPosition = ((int)_camera.Transform.Position.X / Grid.X, (int)_camera.Transform.Position.Y / Grid.Y, (int)_camera.Transform.Position.Z / Grid.Z);

            List<RenderObject> relevant = new(Grid.X * Grid.Y * Grid.Z);

            if (_objects.ContainsKey(playerPosition))
                relevant.AddRange(_objects[playerPosition]);

            return relevant;
        }
    }
}
