namespace FrontToBack.Services.Email
{
    public interface ICreateEmailFile
    {
        string CreateFile(string FilePath, string ToText);
    }
}
