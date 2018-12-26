namespace WorkPump.Common.Jobs
{
    public abstract class JobDefinitionBase
    {
        public JobDefinitionBase(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }

        public abstract string Name { get; }
    }
}
