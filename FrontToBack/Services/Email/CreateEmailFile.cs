namespace FrontToBack.Services.Email
{
    public class CreateEmailFile : ICreateEmailFile
    {
        public string CreateFile(string FilePath, string ToText)
        {
            using (StreamReader reader = new StreamReader(FilePath))    // out ile de yazmaq olar
            {
                ToText = reader.ReadToEnd();
            }
            return ToText;
        }
    }
}
