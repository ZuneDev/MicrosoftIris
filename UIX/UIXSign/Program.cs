using dnlib.DotNet;
using dnlib.DotNet.Pdb;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;
using System;
using System.IO;

namespace UIXSign
{
    class Program
    {
        static void Main(string[] args)
        {
            string dll = args[0];
            string outputName = args.Length >= 2 ? args[1] : "UIX_signed.dll";
            string outputDir = Path.GetDirectoryName(dll);
            string dllCopy = Path.Combine(outputDir, outputName);
            File.Copy(dll, dllCopy, true);
            Console.WriteLine("Created working copy");

            // Load DLL
            using var module = ModuleDefMD.Load(dllCopy);
            var asm = module.Assembly;
            var writeOptions = new ModuleWriterOptions(module);
            Console.WriteLine("Loaded copy");

            if (module.Metadata.PEImage.ImageDebugDirectories.Count > 0)
                Console.WriteLine(module.Metadata.PEImage.ImageDebugDirectories[0]);
            Console.WriteLine("Original public key: " + asm.PublicKey.ToString());

            // 'Sign' the library so it can be loaded by unmodified Zune executables
            asm.PublicKey = new PublicKey("00240000048000009400000006020000002400005253413100040000010001001dc70401884cdfad2010ce192e1f08a30fb034cf504759943eec3359d4ed09af3ce1616dbb124e479617ec73e4162903766e7a5e7bf1984bb318040118fe0f69dfb8b6e5c7c47a0e1bc9a8984b22f7221cc235986c09c74cab38ea3562c18adb8e3a95b73faf1ed71d7c309058b86d951af2165eb215b47de335e360a6a25da7");
            module.IsStrongNameSigned = true;
            Console.WriteLine("Signed assembly");

            if (module.Metadata.PEImage.ImageDebugDirectories.Count > 0)
                Console.WriteLine(module.Metadata.PEImage.ImageDebugDirectories[0]);

            // Set up PDB to enable debugging
            if (module.Metadata.PEImage.ImageDebugDirectories.Count <= 0)
                module.CreatePdbState(PdbFileKind.WindowsPDB);
            using FileStream pdbFile = File.OpenRead(Path.ChangeExtension(dll, "pdb"));
            writeOptions.WritePdb = true;
            writeOptions.PdbOptions = PdbWriterOptions.PdbChecksum;
            writeOptions.PdbFileName = Path.GetFileName(pdbFile.Name);
            writeOptions.PdbFileNameInDebugDirectory = Path.GetFileName(pdbFile.Name);
            //writeOptions.PdbFileNameInDebugDirectory = Path.ChangeExtension(Path.GetFileName(dll), ".dn2.pdb");
            //module.CreatePdbState(PdbFileKind.EmbeddedPortablePDB);
            //var pdbBytes = File.ReadAllBytes(Path.ChangeExtension(dll, "pdb"));
            //var pdbStream = DataStreamFactory.Create(pdbBytes);
            //var pdbReader = ByteArrayDataReaderFactory.CreateReader(pdbBytes);
            //module.Metadata.PEImage.ImageDebugDirectories.Add(new ImageDebugDirectory(ref pdbReader, true));
            Console.WriteLine("Loaded PDB");

            if (module.Metadata.PEImage.ImageDebugDirectories.Count > 0)
                Console.WriteLine(module.Metadata.PEImage.ImageDebugDirectories[0]);

            // Replace the raw build with the modded DLL
            module.Write(dll, writeOptions);
            File.Delete(dllCopy);
            Console.WriteLine("DLL modded:");
            Console.WriteLine(dll);
        }
    }
}
