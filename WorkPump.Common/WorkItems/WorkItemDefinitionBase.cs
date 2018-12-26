namespace WorkPump.Common.WorkItems
{
    public abstract class WorkItemDefinitionBase
    {
        public WorkItemDefinitionBase(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }

        public abstract string Name { get; }
    }
}
