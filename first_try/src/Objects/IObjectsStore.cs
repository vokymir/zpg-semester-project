namespace zpg
{
    /// <summary>
    /// Stores all RenderObjects in the scene. Doesn't make sense to make it parametric, in the moment.
    /// Provides all useful methods for objects manipulation and retrieval.
    /// </summary>
    interface IObjectsStore
    {
        public void Add(RenderObject obj);

        public List<RenderObject> GetAll();

        public List<RenderObject> GetOnlyRelevant();
    }
}
