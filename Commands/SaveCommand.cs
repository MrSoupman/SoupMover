
using SoupMover.Models;
using SoupMover.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SoupMover.Commands
{
    public class SaveCommand : BaseCommand
    {
        private ObservableCollection<string> SourceFiles;
        private ObservableCollection<DestinationPathViewModel> Directories;
        public override void Execute(object parameter)
        {
            SaveFileDialog save = new SaveFileDialog()
            {
                Filter = "JSON files(*.json) | *.json",
                RestoreDirectory = true,
                OverwritePrompt = true
            };
            if (save.ShowDialog() == DialogResult.OK)
            {
                using (var stream = File.Create(save.FileName))
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
        }

        public SaveCommand(ObservableCollection<string> SourceFiles, ObservableCollection<DestinationPathViewModel> Directories)
        {
            this.SourceFiles = SourceFiles;
            this.Directories = Directories;
        }
    }
}
