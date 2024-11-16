public class FileNameData
{
    public string Name;
    public string FullName;
    public string NoExtendPath;
    public string ClassName;
    public FileNameData(string name, string fullName,string path)
    {
        Name = name;
        FullName = fullName;
        NoExtendPath = path;    
        ClassName = name.Replace("q_", "");
        ClassName = ClassName.Replace(".xlsx","");
    }
}
