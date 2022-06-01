
using SoupMover.Models;
using SoupMover.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SoupMover.Commands
{
    public class SaveCommand : BaseCommand
    {
        private List<string> SourceFiles;
        private List<DestinationPathViewModel> Directories;
        private string SaveLocation { get; set; } = string.Empty;
        public override void Execute(object parameter)
        {
            if (parameter != null) //when we call this using the menu, this is null
            {
                SaveData(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "autosave.json");
                return;
            }
            SaveFileDialog save = new SaveFileDialog()
            {
                Filter = "JSON files(*.json) | *.json",
                RestoreDirectory = true,
                OverwritePrompt = true,
                InitialDirectory = SaveLocation ?? ""
            };
            if (save.ShowDialog() == DialogResult.OK)
            {
                SaveData(save.FileName);                
            }
        }

        private void SaveData(string path)
        {
            using (var stream = File.Create(path))
            {
                if (stream != null)
                {
                    var options = new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    };
                    var Save = new SaveData(SourceFiles, Directories);
                    string json = JsonSerializer.Serialize<SaveData>(Save, options);
                    stream.Write(Encoding.UTF8.GetBytes(json), 0, json.Length);
                    stream.Close();
                }
            }
        }

        public SaveCommand(List<string> SourceFiles, List<DestinationPathViewModel> Directories)
        {
            this.SourceFiles = SourceFiles;
            this.Directories = Directories;
        }
    }
}
