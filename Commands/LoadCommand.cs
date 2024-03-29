﻿using SoupMover.Models;
using SoupMover.ViewModels;
using System.IO;
using System.Text.Json;
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
                HVM.Reset();
                string json = File.ReadAllText(open.FileName);
                SaveData save = JsonSerializer.Deserialize<SaveData>(json);
                foreach (string source in save.SourceFiles)
                    HVM.AddToSourceFiles(source);
                foreach (DestinationPathViewModel dir in save.Directories)
                { 
                    HVM.AddToDirectories(dir);
                    HVM.TotalCount += dir.GetFiles().Count;
                }
            }
        }

        public LoadCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
