namespace zpg
{
    /// <summary>
    /// Have a private dictionary of objects, indexed by their grid position.
    /// Therefore can get only relevant object, even though in small levels, it's the whole maze probably.
    /// </summary>
    class ObjectsStoreGrid : IObjectsStore
    {
        private Dictionary<(int, int, int), List<RenderObject>> _objects;
        // grid dimensions, don't have to be a cube
        public (int X, int Y, int Z) Grid { get; init; } = (3, 3, 3);
        // needed for knowing which objects are relevant
        private Camera _camera;
        // generated initially, because I was too lazy to write it down myself...
        private List<(int X, int Y, int Z)> _relativeRelevantGrid = new();

        public ObjectsStoreGrid(Camera camera)
        {
            _camera = camera;
            _objects = new();

            GenerateRelativeRelevantGrid();
        }

        private void GenerateRelativeRelevantGrid()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        _relativeRelevantGrid.Add((x, y, z));
                    }
                }
            }

        }

        public void Add(RenderObject obj)
        {
            int x = (int)(obj.Transform.Position.X);
            int y = (int)(obj.Transform.Position.Y);
            int z = (int)(obj.Transform.Position.Z);

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
            (int X, int Y, int Z) playerPosition = ((int)_camera.Transform.Position.X / Grid.X, (int)_camera.Transform.Position.Y / Grid.Y, (int)_camera.Transform.Position.Z / Grid.Z);
            List<RenderObject> relevant = new(Grid.X * Grid.Y * Grid.Z);

            foreach (var shift in _relativeRelevantGrid)
            {
                var position = (playerPosition.X + shift.X, playerPosition.Y + shift.Y, playerPosition.Z + shift.Z);
                if (_objects.ContainsKey(position))
                    relevant.AddRange(_objects[position]);
            }

            return relevant;
        }
    }
}
