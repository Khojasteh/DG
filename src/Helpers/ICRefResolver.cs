namespace Document.Generator.Helpers
{
    public interface ICRefResolver
    {
        string NameOf(string cref);
        string UrlOf(string cref);
    }
}
