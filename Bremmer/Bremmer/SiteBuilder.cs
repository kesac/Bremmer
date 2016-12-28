using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace Bremmer
{
    public delegate void ProcessEventHandler(object sender, EventArgs e);

    public class ProcessEventArgs : EventArgs
    {
        public string Description { get; set; }

        public ProcessEventArgs(string description)
        {
            this.Description = description;
        }
    }

    public class SiteBuilder
    {
        private readonly string TemplatesDirectory = "templates";
        private readonly string ViewsDirectory = "views";
        private readonly string ResourcesDirectory = "resources";
        private readonly string RazorExtension = ".cshtml";

        public DirectoryInfo Source { get; set; }
        public DirectoryInfo Destination { get; set; }

        public event ProcessEventHandler Processed;

        public SiteBuilder(string source)
        {
            this.Source = new DirectoryInfo(source);
            this.Destination = new DirectoryInfo(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "output");
        }

        private void Log(string description)
        {
            if(this.Processed != null)
            {
                this.Processed.Invoke(this, new ProcessEventArgs(description));
            }
        }

        public void Build()
        {
            string outputFolder = this.Destination.FullName;
            if (!Directory.Exists(outputFolder))
            {
                this.Log("Created " + outputFolder);
                Directory.CreateDirectory(outputFolder);
            }

            DirectoryInfo[] subdirectories = this.Source.GetDirectories();

            this.ClearTarget();
            this.DefineTemplates(subdirectories);
            this.ProcessViews(subdirectories);
            this.CopyResources(subdirectories);

            this.Log("Site build complete. See " + this.Destination.FullName);
        }

        private void ClearTarget()
        {
            this.Log("Deleting output folder and all contents...");
            Directory.Delete(this.Destination.FullName, true);
        }

        private void DefineTemplates(DirectoryInfo[] directories)
        {
            // Find templates
            var templates = directories.FirstOrDefault(x => x.Name.ToLower().Equals(TemplatesDirectory));
            if (templates != null)
            {
                foreach (FileInfo template in templates.GetFiles())
                {
                    this.Log("Processing template " + template.FullName);
                    if (template.Name.ToLower().EndsWith(RazorExtension)) // Ignore non-razor files
                    {
                        using (StreamReader reader = new StreamReader(template.FullName))
                        {
                            string name = template.Name.RemoveExtension(RazorExtension);
                            string data = reader.ReadToEnd();
                            Engine.Razor.AddTemplate(name, data);
                            this.Log("Added template " + name);
                        }
                    }
                }
            }
        }

        private void ProcessViews(DirectoryInfo[] directories)
        {
            var views = directories.FirstOrDefault(x => x.Name.ToLower().Equals(ViewsDirectory));
            if (views != null)
            {
                foreach (FileInfo view in views.GetFiles("*", SearchOption.AllDirectories)) // Recurse
                {
                    if (view.Name.ToLower().EndsWith(RazorExtension)) // Ignore non-razor files
                    {
                        this.Log("Processing view " + view.FullName);
                        using (StreamReader reader = new StreamReader(view.FullName))
                        {
                            string name = view.FullName.RemoveSubstring(views.FullName).RemoveExtension(RazorExtension);
                            string data = reader.ReadToEnd();
                            string destination = this.Destination.FullName + Path.DirectorySeparatorChar + name;

                            if (name.EndsWith("index"))
                            {
                                string destinationFolder = this.Destination.FullName + Path.DirectorySeparatorChar + name.RemoveExtension("index");
                                if (!Directory.Exists(destinationFolder))
                                {
                                    Directory.CreateDirectory(destinationFolder);
                                }

                                destination += ".html";
                            }
                            else
                            {
                                Directory.CreateDirectory(destination); // order matters here
                                destination += Path.DirectorySeparatorChar + "index.html";
                            }

                            var result = Engine.Razor.RunCompile(data, name, null, new { });
                            using (StreamWriter writer = new StreamWriter(destination))
                            {
                                writer.WriteLine(result);
                                this.Log("Compiled view " + view.FullName);
                            }

                        }
                    }
                }
            }
        }

        private void CopyResources(DirectoryInfo[] directories)
        {
            // Copy resources
            var resources = directories.FirstOrDefault(x => x.Name.ToLower().Equals(ResourcesDirectory));
            if (resources != null)
            {
                foreach (string path in Directory.GetDirectories(resources.FullName, "*", SearchOption.AllDirectories))
                {
                    string newPath = path.Replace(this.Source.FullName, this.Destination.FullName);
                    Directory.CreateDirectory(newPath);
                    this.Log("Created new folder " + newPath);
                }

                foreach (string path in Directory.GetFiles(resources.FullName, "*", SearchOption.AllDirectories))
                {
                    string newPath = path.Replace(this.Source.FullName, this.Destination.FullName);
                    File.Copy(path, newPath, true);
                    this.Log("Copied file to" + newPath);
                }
            }
        }

    }
}