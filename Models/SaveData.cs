using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Models
{
    /// <summary>
    /// Kind of a dumb way I thought of for serializing json data.
    /// I wanted to store both sourcefiles and destination path data in one json file
    /// ...But I don't think C#'s json can do that.
    /// So I decided to do this jank class file so I can (de)serialize easily
    /// </summary>
    public class SaveData
    {
        public ObservableCollection<string> SourceFiles { get; set; }
        public ObservableCollection<DestinationPathViewModel> Directories { get; set; }
        public SaveData(ObservableCollection<string> SourceFiles, ObservableCollection<DestinationPathViewModel> Directories)
        {
            this.SourceFiles = SourceFiles;
            this.Directories = Directories;
        }
    }
}
