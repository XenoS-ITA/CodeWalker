using CodeWalker.GameFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

class Program
{
    private static void ConvertXml(string xmlPath, string pathOut = null)
    {
        string fileName = Path.GetFileName(xmlPath);
        string fileNameWithoutXML = Path.GetFileNameWithoutExtension(xmlPath);

        if (pathOut == null)
        {
            pathOut = xmlPath.Replace(".xml", "");
        }
        else
        {
            if (!Directory.Exists(pathOut))
            {
                throw new Exception($"The output path '{pathOut}' dont exists!");
            }

            pathOut = Path.Combine(pathOut, fileNameWithoutXML);
        }

        // Ignore no xml files
        if (!fileName.EndsWith(".xml"))
        {
            throw new Exception(fileName + " is not an XML file!");
        }

        // Get file path without extension
        string dir = Path.GetDirectoryName(xmlPath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutXML);

        string pathWithoutExtension = Path.Combine(dir, fileNameWithoutExtension);

        // Load XML
        var doc = new XmlDocument();
        string text = File.ReadAllText(xmlPath);
        if (!string.IsNullOrEmpty(text))
        {
            doc.LoadXml(text);
        }

        var mformat = XmlMeta.GetXMLFormat(fileName, out _);
        byte[] data = XmlMeta.GetData(doc, mformat, pathWithoutExtension);

        if (data == null)
        {
            throw new Exception(fileName + ": Format not supported. Cannot import " + XmlMeta.GetXMLFormatName(mformat));
        }

        // Create if not already existent
        if (!File.Exists(pathOut))
        {
            try
            {
                var file = File.Create(pathOut);
                file.Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        File.WriteAllBytes(pathOut, data);
    }

    static void Main(string[] args)
    {
        List<string> inputFiles = new List<string>();
        string outputDir = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-o")
            {
                i++;
                outputDir = args[i];
            } else
            {
                inputFiles.Add(args[i]);
            }
        }

        if (inputFiles.Count == 0)
        {
            throw new Exception("No input files!");
        }

        foreach (string inputFile in inputFiles)
        {
            ConvertXml(inputFile, outputDir);
        }
    }
}