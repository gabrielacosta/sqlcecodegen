﻿using CodeGenGUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace ChristianHelle.DatabaseTools.SqlCe.CodeGenCore.UnitTest
{
    [TestClass]
    public class CodeGenFileSerializerTest
    {
        [TestMethod]
        public void SaveFileTest()
        {
            var actual = new CodeGenFile
            {
                DataSource = Guid.NewGuid().ToString(),
                GeneratedCode = Guid.NewGuid().ToString()
            };
            var target = new CodeGenFileSerializer();
            target.SaveFile(actual, "test.xml");
        }

        [TestMethod]
        public void LoadFileTest()
        {
            var target = new CodeGenFileSerializer();
            target.LoadFile("test.xml");
        }
    }
}