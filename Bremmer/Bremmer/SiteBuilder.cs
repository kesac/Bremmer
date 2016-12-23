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
    public class SiteBuilder
    {
        private readonly string TemplatesDirectory = "templates";
        private readonly string ViewsDirectory = "views";
        private readonly string ResourcesDirectory = "resources";
        private readonly string RazorExtension = ".cshtml";

        public DirectoryInfo Source { get; set; }
        public DirectoryInfo Target { get; set; }

        public SiteBuilder(string source)
        {
            this.Source = new DirectoryInfo(source);
            this.Target = new DirectoryInfo(Environment.CurrentDirectory + "/" + "output");
        }


        public void Build()
        {
            string outputFolder = this.Target.FullName;
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            DirectoryInfo[] subdirectories = this.Source.GetDirectories();

            this.DefineTemplates(subdirectories);
            this.ProcessViews(subdirectories);
            this.CopyResources(subdirectories);
        }


        private void DefineTemplates(DirectoryInfo[] directories)
        {
            // Find templates
            var templates = directories.FirstOrDefault(x => x.Name.ToLower().Equals(TemplatesDirectory));
            if (templates != null)
            {
                foreach (FileInfo template in templates.GetFiles())
                {
                    if (template.Name.ToLower().EndsWith(RazorExtension)) // Ignore non-razor files
                    {
                        using (StreamReader reader = new StreamReader(template.FullName))
                        {
                            string name = template.Name.RemoveExtension(RazorExtension);
                            string data = reader.ReadToEnd();
                            Engine.Razor.AddTemplate(name, data);
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
                foreach (FileInfo view in views.GetFiles())
                {
                    if (view.Name.ToLower().EndsWith(RazorExtension)) // Ignore non-razor files
                    {
                        using (StreamReader reader = new StreamReader(view.FullName))
                        {
                            string name = view.Name.RemoveExtension(RazorExtension);
                            string data = reader.ReadToEnd();

                            var result = Engine.Razor.RunCompile(data, name, null, new { });

                            using (StreamWriter writer = new StreamWriter(this.Target.FullName + "/" + name + ".html"))
                            {
                                writer.WriteLine(result);
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
                    string newPath = path.Replace(this.Source.FullName, this.Target.FullName);
                    Directory.CreateDirectory(newPath);
                }

                foreach (string path in Directory.GetFiles(resources.FullName, "*", SearchOption.AllDirectories))
                {
                    string newPath = path.Replace(this.Source.FullName, this.Target.FullName);
                    File.Copy(path, newPath, true);
                }
            }
        }

    }
}