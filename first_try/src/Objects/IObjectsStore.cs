namespace zpg
{
    interface IObjectsStore
    {
        public void Add(RenderObject obj);

        public List<RenderObject> GetAll();

        public List<RenderObject> GetOnlyRelevant();
    }
}
