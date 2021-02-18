namespace ClientGUI.ItemTemplates
{
    public class ListResult
    {
        public string ImageSource { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; private set; }

        public ListResult(bool isDirectory, string name)
        {
            ImageSource = isDirectory ? "Content/folderIcon.png" : "Content/fileIcon.png";
            Name = name;
            IsDirectory = isDirectory;
        }
    }
}
