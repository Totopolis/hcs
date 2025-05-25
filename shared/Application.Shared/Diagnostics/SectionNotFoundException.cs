namespace Application.Shared.Diagnostics;

[Serializable]
public class SectionNotFoundException : Exception
{
    public SectionNotFoundException()
    {
    }

    public SectionNotFoundException(string message) : base(message)
    {
    }
}
