namespace WorkPump.Common.WorkItems
{
    public interface IWorkItemDefinitionRepository
    {
        bool Insert(WorkItemDefinitionBase workItemDefinition);

        WorkItemDefinitionBase Get(ulong id);

        T Find<T>(ulong id)
            where T : WorkItemDefinitionBase;

        bool Remove(ulong id);
    }

    public class WorkItemDefinitionRepository
    {
    }
}
