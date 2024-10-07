﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace BulkSecurity
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Bulk Security"),
        ExportMetadata("Description", "An XRM ToolBox Tool"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "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"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class MyPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new MyPluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public MyPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}