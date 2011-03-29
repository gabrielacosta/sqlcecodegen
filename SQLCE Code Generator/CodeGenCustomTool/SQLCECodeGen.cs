﻿using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ChristianHelle.DatabaseTools.SqlCe.CodeGenCore;
using Microsoft.CustomTool;

namespace ChristianHelle.DatabaseTools.SqlCe.CodeGenCustomTool
{
    [Guid("64264FF6-2DD0-489a-A8C2-8FD7855FE3BF")]
    [ComVisible(true)]
    public class SQLCECodeGenerator : BaseCodeGeneratorWithSite
    {
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            return GenerateCode(FileNameSpace, inputFileName, CodeProvider.FileExtension);
        }

        public static byte[] GenerateCode(string FileNameSpace, string inputFileName, string fileExtension)
        {
            var fi = new FileInfo(inputFileName);
            var generatedNamespace = FileNameSpace + "." + fi.Name.Replace(fi.Extension, string.Empty);
            var connectionString = "Data Source=" + inputFileName;
            var database = new SqlCeDatabase(generatedNamespace, connectionString);
            var factory = new CodeGeneratorFactory(database);
            var codeGenerator = factory.Create(fileExtension);

            codeGenerator.WriteHeaderInformation();
            codeGenerator.GenerateEntities();
            codeGenerator.GenerateDataAccessLayer();

            var generatedCode = codeGenerator.GetCode();
            return Encoding.Unicode.GetBytes(generatedCode);
        }
    }
}