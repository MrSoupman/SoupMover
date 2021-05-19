using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SoupMover.FTM;
namespace SoupMover.Database
{
    public class Data
    {
        /// <summary>
        /// Holds a list of the files to be queued to another directory
        /// </summary>
        private List<string> SourceFiles;
        
        /// <summary>
        /// A list of directories and files to be moved to it
        /// </summary>
        private List<FilesToMove> Directories;


        /// <summary>
        /// The total amount of files that have been queued to be moved
        /// </summary>
        public uint IntTotalFiles { get; set; }

        public Data()
        {
            SourceFiles = new List<string>();
            Directories = new List<FilesToMove>();
        }

        //This section contains methods to manipulate the db

        /// <summary>
        /// Adds a file to the db's source files
        /// </summary>
        /// <param name="file"></param>
        /// <returns>returns true if the file doesn't already exist in the db and exists on disk; false otherwise.</returns>
        public bool AddFileToSource(string file)
        {
            if (File.Exists(file) && !SourceFiles.Contains(file))
            { 
                SourceFiles.Add(file);
                return true;            
            }
            return false;
        }

        /// <summary>
        /// Removes a file, if it exists, from the source list
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if the file was removed, false otherwise</returns>
        public bool RemoveFileFromSource(string file)
        {
            if (SourceFiles.Contains(file))
            {
                SourceFiles.Remove(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a file to a directory
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool AddFileToDestination(string file, string directory)
        {
            int index = GetIndexOfDirectory(directory);
            if (index > Directories.Count || index < 0)
                return false;
            if (!Directories[index].Contains(file))
            {
                if (Directories[index].Add(file))
                {
                    IntTotalFiles++;
                    RemoveFileFromSource(file);
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Removes a file from its destination, if both exist.
        /// </summary>
        /// <param name="file">The file to be moved</param>
        /// <param name="directory">The directory the file should be moved to</param>
        /// <returns></returns>
        public bool RemoveFileFromDestination(string file, string directory)
        {
            int index = GetIndexOfDirectory(directory);
            return RemoveFileFromDestination(file, index);
        }

        public bool RemoveFileFromDestination(string file, int index)
        {
            if (index > Directories.Count || index < 0)
                return false;
            if (Directories[index].Contains(file))
            {
                if (Directories[index].Remove(file))
                {
                    IntTotalFiles--;
                    return true;
                }
            }
            return false;
        }
                
        /// <summary>
		/// Adds a directory to db, if it doesn't exist
		/// </summary>
		/// <param name="dir"></param>
		public bool AddDirectory(string directory)
        {
            FilesToMove dir = new FilesToMove(directory);
            //If we get a valid directory to drop things in, we need to create a new directory, 
            //as well as a list to hold what files go to that directory
            if (!Directories.Contains(dir) && Directory.Exists(directory))
            {
                Directories.Add(dir);
                Directories.Sort();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a specified directory from the database
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>true for successful removal, false otherwise</returns>
        public bool RemoveDirectory(string directory)
        {
            int index = Directories.IndexOf(new FilesToMove(directory));
            return RemoveDirectory(index);
        }
        /// <summary>
        /// Removes a directory at the specified index from the database
        /// </summary>
        /// <param name="index"></param>
        /// <returns>true for successful removal, false otherwise</returns>
        public bool RemoveDirectory(int index)
        {
            if (index > Directories.Count || index < 0)
                return false;
            if (Directories[index].Count() > 0) //directory isn't empty
            {
                foreach (string file in Directories[index].GetFiles())
                { 
                    AddFileToSource(file);
                    RemoveFileFromDestination(file, index);
                }
            }
            Directories.RemoveAt(index);
            return true;
        }

        /// <summary>
		/// Private helper method to recursively add directories
		/// </summary>
		/// <param name="directory"></param>
		public void RecursiveDir(string directory)
        {
            try
            {
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(directory));
                if (dirs.Count == 0)
                    return;
                foreach (var dir in dirs)
                {
                    AddDirectory(dir.ToString());
                    RecursiveDir(dir.ToString());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
            catch (PathTooLongException e)
            {
                throw e;
            }
        }

        /// <summary>
		/// Private helper method to recursively add files
		/// </summary>
		/// <param name="directory"></param>
		public void RecursiveFiles(string directory)
        {
            try
            {
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(directory));
                List<string> files = new List<string>(Directory.EnumerateFiles(directory));
                if (dirs.Count == 0 && files.Count == 0)
                    return;
                foreach (string file in files)
                    AddFileToSource(file);
                foreach (var dir in dirs)
                    RecursiveFiles(dir.ToString());
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
            catch (PathTooLongException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Resets the database
        /// </summary>
        public void ResetDB()
        {
            SourceFiles.Clear();
            Directories.Clear();
            IntTotalFiles = 0;
        }

        /// <summary>
        /// Loads a json file into the database, if it is valid.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True on a successful load, false otherwise</returns>
        public bool LoadJSon(string file)
        {
            if (File.Exists(file))
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    JObject jFile = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    if (jFile.ContainsKey("SourceFiles") && jFile.ContainsKey("Directories"))
                    {
                        JArray sourceArray = JArray.Parse(jFile["SourceFiles"].ToString());
                        foreach (JToken token in sourceArray)
                            SourceFiles.Add(token.ToString());
                        JObject dirObj = JObject.Parse(jFile["Directories"].ToString());
                        Console.WriteLine(dirObj.Count);
                        foreach (JContainer obj in dirObj.Children())
                        {
                            JProperty property = obj.ToObject<JProperty>();
                            FilesToMove dir = new FilesToMove(property.Name);
                            JArray filesArray = JArray.Parse(property.Value.ToString());
                            foreach (JToken token in filesArray)
                            {
                                dir.Add(token.ToString());
                                IntTotalFiles++;
                            }
                            Directories.Add(dir);
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes the database out to a json file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true for a successful save, false otherwise</returns>
        public bool SaveJSon(string file)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.WriteStartObject();
                    writer.WritePropertyName("SourceFiles");
                    writer.WriteStartArray();
                    foreach (string sfile in SourceFiles)
                        writer.WriteValue(sfile);
                    writer.WriteEndArray();

                    writer.WritePropertyName("Directories");
                    writer.WriteStartObject();
                    foreach (FilesToMove dirs in Directories)
                    {
                        writer.WritePropertyName(dirs.GetDirectory());
                        writer.WriteStartArray();
                        foreach (string dfile in dirs.GetFiles())
                            writer.WriteValue(file);
                        writer.WriteEndArray();
                    }
                    writer.WriteEnd();
                    writer.WriteEnd();
                }
                try
                {
                    File.WriteAllText(file, sw.ToString());
                }
                catch(Exception e)
                {
                    string logName = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("MM-dd-yy-hhmmss") + ".txt";
                    if (!File.Exists(logName))
                    {
                        File.Create(logName).Dispose();
                    }
                    using (StreamWriter writer = File.AppendText(logName))
                    {
                        writer.WriteLine("Exception Dump on " + DateTime.Now.ToString("MM-dd-yy-hhmmss"));
                        writer.WriteLine("----------------------------------------\n");
                        writer.WriteLine(e.Message);
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("End of dump.");
                    }
                    return false;
                }

                return true;
            }
        }

        public bool MoveFile(string file, string directory)
        {
            int index = Directories.IndexOf(new FilesToMove(directory));
            return MoveFile(file, index);
        }

        /// <summary>
        /// Given a specific file, moves it
        /// </summary>
        /// <param name="file"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool MoveFile(string file, int index)
        {
            if (index < Count())
            {
                if (Directories[index].Contains(file))
                {
                    if (File.Exists(file) && !File.Exists(Directories[index].GetDirectory()))
                    {
                        int FileIndex = Directories[index].IndexOf(file);
                        try
                        {
                            File.Move(Directories[index].GetFile(FileIndex), Directories[index].GetDirectory() + "\\" + Path.GetFileName(Directories[index].GetFile(FileIndex)));
                            if (Directories[index].RemoveAt(FileIndex))
                                return true;
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                        return true;
                    }
                }
                else
                    return false;
            }
            return false;
        }

        //This group for methods that return information about the current database
        
        /// <summary>
        /// Returns the index of some directory, if it exists, in the database.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>Index of directory, or -1 if not found.</returns>
        public int GetIndexOfDirectory(string dir)
        {
            return Directories.IndexOf(new FilesToMove(dir));
        }

        /// <summary>
        /// Returns a list of files from a specific directory in the db
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public List<string> GetFilesFromDirectory(string dir)
        {
            int index = GetIndexOfDirectory(dir);
            if (index > Directories.Count || index < 0)
                return new List<string>(); //we couldn't find anything, just return an empty list for now
            return Directories[index].GetFiles();

        }

        /// <summary>
        /// Returns information about a specific directory
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A FilesToMove containing information about the directory and files to move to it</returns>
        public FilesToMove GetDirInfo(int index)
        {
            if (index < Directories.Count && index >= 0)
                return Directories[index];
            else
                return new FilesToMove();

        }

        /// <summary>
        /// Returns the size of Directories
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Directories.Count;
        }

        public List<FilesToMove> GetDirectories()
        {
            return Directories;
        }

        public List<string> GetSourceFiles()
        {
            return SourceFiles;
        }
    }
}
