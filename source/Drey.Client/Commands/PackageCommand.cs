using Mono.Options;
using System;
using System.IO;

namespace Drey.Client.Commands
{
    public class PackageCommand : BaseCommand
    {
        string _inputFolder;
        string _outputFile;

        public PackageCommand()
        {
            Command = "package";
            Description = "Packages an applet into an archive file.";
            Parser = new OptionSet
            {
                { "i|inputFolder=", "The input folder", v => _inputFolder = v },
                { "o|outputFile=", "The name and path of the output folder.", v => _outputFile = v }
            };
        }
        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(_inputFolder) || string.IsNullOrEmpty(_outputFile));
        }
        public override int Execute()
        {
            var fInfo = new FileInfo(_outputFile);
            var directoryPath = fInfo.Directory.FullName;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (fInfo.Exists)
            {
                fInfo.Delete();
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(_inputFolder, _outputFile, System.IO.Compression.CompressionLevel.Optimal, false);
            return 0;
        }
    }
}