using SoupMover.Models;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoupMover.Commands
{
    public class LoadCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "JSON files(*.json) | *.json",
                RestoreDirectory = true
            };
            if (open.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(open.FileName);
                SaveData save = JsonSerializer.Deserialize<SaveData>(json);
                foreach (string source in save.SourceFiles)
                    HVM.AddToSourceFiles(source);
                foreach(DestinationPathViewModel dir in save.Directories)
                    HVM.AddToDirectories(dir);
            }
        }

        public LoadCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
