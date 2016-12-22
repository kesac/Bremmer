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
        private readonly string RazorExtension = ".cshtml";

        public DirectoryInfo Source { get; set; }
        public DirectoryInfo Target { get; set; }

        public SiteBuilder(string source)
        {
            this.Source = new DirectoryInfo(source);
            this.Target = new DirectoryInfo(Environment.CurrentDirectory);
        }

        public void Build()
        {

            string outputFolder = this.Target.FullName + "/" + "output";
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            DirectoryInfo[] subdirectories = this.Source.GetDirectories();

            // Find templates
            var templates = subdirectories.FirstOrDefault(x => x.Name.ToLower().Equals(TemplatesDirectory));
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

            // Process pages
            var views = subdirectories.FirstOrDefault(x => x.Name.ToLower().Equals(ViewsDirectory));
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

                            try
                            {
                                var result = Engine.Razor.RunCompile(data, name, null, new { });
                                using (StreamWriter writer = new StreamWriter(outputFolder + "/" + name + ".html"))
                                {
                                    writer.WriteLine(result);
                                }
                            }
                            catch(Exception e)
                            {
                                System.Console.WriteLine("Could not generate from " + view.FullName);
                                System.Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }

            // Copy resources

        }
    }
}