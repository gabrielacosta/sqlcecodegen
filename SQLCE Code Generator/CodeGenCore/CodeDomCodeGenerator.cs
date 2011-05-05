﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Text;

namespace ChristianHelle.DatabaseTools.SqlCe.CodeGenCore
{
    public class CodeDomCodeGenerator : CodeGenerator, IDisposable
    {
        protected readonly CodeCompileUnit compileUnit;
        protected readonly CodeNamespace codeNamespace;
        protected readonly CodeDomProvider provider;

        public CodeDomCodeGenerator(SqlCeDatabase tableDetails, CodeDomProvider provider)
            : base(tableDetails)
        {
            this.provider = provider;

            compileUnit = new CodeCompileUnit();
            codeNamespace = new CodeNamespace(Database.Namespace);
            compileUnit.Namespaces.Add(codeNamespace);
        }

        public override string GetCode()
        {
            using (var writer = new StringWriter(code))
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
            return code.ToString();
        }

        public override void ClearCode()
        {
            codeNamespace.Comments.Clear();
            codeNamespace.Imports.Clear();
            codeNamespace.Types.Clear();
            base.ClearCode();
        }

        private static void GenerateXmlDoc(CodeCommentStatementCollection comments, string summary, params KeyValuePair<string, string>[] parameters)
        {
            comments.Add(new CodeCommentStatement("<summary>", true));
            comments.Add(new CodeCommentStatement(summary, true));
            comments.Add(new CodeCommentStatement("</summary>", true));

            foreach (var parameter in parameters)
                comments.Add(
                    new CodeCommentStatement(
                        string.Format("<param name=\"{0}\">{1}</param>", parameter.Key, parameter.Value), true));
        }

        public override void WriteHeaderInformation()
        {
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
            codeNamespace.Comments.Add(new CodeCommentStatement("--------------------------------------------------------------------------------------------------"));
            codeNamespace.Comments.Add(new CodeCommentStatement("<auto-generatedInfo>"));
            codeNamespace.Comments.Add(new CodeCommentStatement("\tThis code was generated by SQL CE Code Generator (http://sqlcecodegen.codeplex.com)"));
            codeNamespace.Comments.Add(new CodeCommentStatement("\tSQL CE Code Generator was written by Christian Resma Helle"));
            codeNamespace.Comments.Add(new CodeCommentStatement("\tand is under GNU General Public License version 2 (GPLv2)"));
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
            codeNamespace.Comments.Add(new CodeCommentStatement("\tThis code contains class representations of the objects defined in the SQL CE database schema"));
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
            codeNamespace.Comments.Add(new CodeCommentStatement("\tGenerated: " + DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            codeNamespace.Comments.Add(new CodeCommentStatement("</auto-generatedInfo>"));
            codeNamespace.Comments.Add(new CodeCommentStatement("--------------------------------------------------------------------------------------------------"));
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
        }

        #region Generate Entities

        public override void GenerateEntities()
        {
            GenerateEntities(new EntityGeneratorOptions());
        }

        public override void GenerateEntities(EntityGeneratorOptions options)
        {
            foreach (var table in Database.Tables)
            {
                var type = new CodeTypeDeclaration(table.Name);
                type.Attributes = MemberAttributes.Public;
                type.IsPartial = true;
                type.IsClass = true;
                type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, table.Name));
                type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, table.Name));
                GenerateXmlDoc(type.Comments, "Represents the " + table.Name + " table");

                foreach (var column in table.Columns)
                {
                    var field = new CodeMemberField(column.Value.ManagedType, "_" + column.Key);
                    field.Attributes = MemberAttributes.Private;
                    type.Members.Add(field);
                }

                foreach (var column in table.Columns)
                {
                    if (!column.Value.ManagedType.Equals(typeof(string)) || !column.Value.MaxLength.HasValue)
                        continue;

                    var maxLengthField = new CodeMemberField(typeof(int), column.Key + "_MAX_LENGTH");
                    maxLengthField.Attributes = MemberAttributes.Private | MemberAttributes.Const;
                    maxLengthField.InitExpression = new CodePrimitiveExpression(column.Value.MaxLength);

                    GenerateXmlDoc(maxLengthField.Comments, "The Maximum Length the " + column.Value.Name + " field allows");
                    type.Members.Add(maxLengthField);
                }

                foreach (var column in table.Columns)
                {
                    var property = new CodeMemberProperty();
                    property.Name = column.Key;
                    property.Type = new CodeTypeReference(column.Value.ManagedType);
                    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    GenerateXmlDoc(property.Comments, "Gets or sets the value of " + column.Value.Name);

                    property.GetStatements.Add(
                        new CodeMethodReturnStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), "_" + column.Key)));

                    if (column.Value.ManagedType.Equals(typeof(string)))
                        property.SetStatements.Add(
                            new CodeConditionStatement(
                                new CodeSnippetExpression("value.Length > " + column.Key + "_MAX_LENGTH"),
                                new CodeThrowExceptionStatement(
                                    new CodeObjectCreateExpression(
                                        new CodeTypeReference(typeof(ArgumentException)),
                                        new CodePrimitiveExpression("Max length for " + column.Key + " is " + column.Value.MaxLength)))));

                    property.SetStatements.Add(
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), "_" + column.Key),
                                new CodePropertySetValueReferenceExpression()));

                    type.Members.Add(property);
                }

                codeNamespace.Types.Add(type);
            }
        }

        #endregion

        public override void GenerateDataAccessLayer()
        {
            GenerateDataAccessLayer(new DataAccessLayerGeneratorOptions());
        }

        public override void GenerateDataAccessLayer(DataAccessLayerGeneratorOptions options)
        {
            GenerateEntityBase();
            GenerateCreateDatabase();
            GenerateIRepository();
            GenerateIDataRepository();

            foreach (var table in Database.Tables)
            {
                GenerateITableRepository(table);
                GenerateTableRepository(table);
            }
        }

        private void GenerateITableRepository(Table table)
        {
        }

        private void GenerateTableRepository(Table table)
        {
        }

        private void GenerateIDataRepository()
        {
        }

        private void GenerateIRepository()
        {
        }

        private void GenerateCreateDatabase()
        {
            // public static class DatabaseFile
            var type = new CodeTypeDeclaration("DatabaseFile");
            type.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            type.IsClass = true;
            type.IsPartial = true;
            type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "DatabaseFile"));
            type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            GenerateXmlDoc(type.Comments, "Helper class for generating the database file in runtime");

            // public static int CreateDatabase()
            var method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = "CreateDatabase";

            // int resultCount = 0
            var resultCount = new CodeVariableDeclarationStatement(typeof(int), "resultCount");
            resultCount.InitExpression = new CodePrimitiveExpression(0);

            // SqlCeEngine engine = null;
            var engine = new CodeVariableDeclarationStatement(typeof(SqlCeEngine), "engine");
            engine.InitExpression = new CodePrimitiveExpression(null);

            // try
            var engineTryFinally = new CodeTryCatchFinallyStatement();

            // engine = new SqlCeEngine(EntityBase.ConectionString);
            engineTryFinally.TryStatements.Add(
                new CodeAssignStatement(
                    new CodeVariableReferenceExpression("engine"),
                    new CodeObjectCreateExpression(typeof(SqlCeEngine),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(new CodeTypeReference("EntityBase")), "ConnectionString"))));

            // engine.CreateDatabase();
            engineTryFinally.TryStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression("engine"), "CreateDatabase")));

            // finally { engine.Dispose(); }
            engineTryFinally.FinallyStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression("engine"), "Dispose")));

            // SqlCeCommand command = null;
            var command = new CodeVariableDeclarationStatement(typeof(SqlCeCommand), "command");
            command.InitExpression = new CodePrimitiveExpression(null);

            // try
            var commandTryFinally = new CodeTryCatchFinallyStatement();

            // command = EntityBase.CreateCommand();
            commandTryFinally.TryStatements.Add(
                new CodeAssignStatement(
                    new CodeVariableReferenceExpression("command"),
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression("EntityBase"), "CreateCommand")));

            foreach (var table in Database.Tables)
            {
                var query = new StringBuilder();
                query.Append("CREATE TABLE ");
                query.Append(table.Name);
                query.Append("(");

                foreach (var column in table.Columns)
                {
                    query.AppendFormat("{0} {1}", column.Key, column.Value.DatabaseType.ToUpper());
                    if (string.Compare(column.Value.DatabaseType, "ntext", true) == 0 ||
                        string.Compare(column.Value.DatabaseType, "image", true) == 0)
                    {
                        query.Append(", ");
                        continue;
                    }
                    if (column.Value.ManagedType == typeof(string))
                        query.Append("(" + column.Value.MaxLength + ")");
                    if (column.Value.AutoIncrement.HasValue)
                        query.AppendFormat(" IDENTITY({0},{1})", column.Value.AutoIncrementSeed, column.Value.AutoIncrement);
                    if (column.Value.IsPrimaryKey)
                        query.Append(" PRIMARY KEY");
                    if (!column.Value.AllowsNull)
                        query.Append(" NOT NULL");
                    query.Append(", ");
                }
                query.Remove(query.Length - 2, 2);
                query.Append(")");

                // command.CommandText = "CREATE TABLE ....."
                commandTryFinally.TryStatements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("command"), "CommandText"),
                        new CodePrimitiveExpression(query.ToString())));

                // resultCount += command.ExecuteNonQuery()
                commandTryFinally.TryStatements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("command"), "ExecuteNonQuery")));
            }

            // finally { command.Dispose(); }
            commandTryFinally.FinallyStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression("command"), "Dispose")));

            method.Statements.Add(resultCount);
            method.Statements.Add(engine);
            method.Statements.Add(engineTryFinally);
            method.Statements.Add(command);
            method.Statements.Add(commandTryFinally);
            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("resultCount")));
            type.Members.Add(method);
            codeNamespace.Types.Add(type);
        }

        #region Generate EntityBase
        private void GenerateEntityBase()
        {
            // public static class EntityBase
            var type = new CodeTypeDeclaration("EntityBase");
            type.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            type.IsPartial = true;
            type.IsClass = true;
            type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "EntityBase"));
            type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            GenerateXmlDoc(type.Comments, "Base class for all data access repositories");

            // private static readonly object syncLock = new object();
            var lockField = new CodeMemberField(typeof(object), "syncLock");
            lockField.Attributes = MemberAttributes.Private | MemberAttributes.Static | MemberAttributes.Final;
            lockField.InitExpression = new CodeObjectCreateExpression(typeof(object));

            // private static SqlCeConnection connectionInstance = null;
            var connField = new CodeMemberField(typeof(SqlCeConnection), "connectionInstance");
            connField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
            connField.InitExpression = new CodePrimitiveExpression(null);

            // public static string ConnectionString = null;
            var connStrField = new CodeMemberField(typeof(string), "ConnectionString");
            connStrField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            connStrField.InitExpression = new CodePrimitiveExpression(null);

            // public static SqlCeConnection Connection
            var connectionProperty = new CodeMemberProperty();
            connectionProperty.Name = "Connection";
            connectionProperty.Type = new CodeTypeReference(typeof(SqlCeConnection));
            connectionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            GenerateXmlDoc(connectionProperty.Comments, "Gets or sets the value of Connection");

            // get { }

            // if (connectionInstance == null)
            //     connection.Open();
            connectionProperty.GetStatements.Add(
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("connectionInstance"),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(null)),
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(null, "connectionInstance"),
                        new CodeObjectCreateExpression(typeof(SqlCeConnection)))));

            // if (connectionInstance.State != System.Data.ConnectionState.Open)
            //     connection.Open();
            connectionProperty.GetStatements.Add(
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("connectionInstance.State"),
                        CodeBinaryOperatorType.IdentityInequality,
                        new CodeSnippetExpression("System.Data.ConnectionState.Open")),
                    new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(null, "connectionInstance"))));

            // connectionInstance.ConnectionString = ConnectionString;
            connectionProperty.GetStatements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("connectionInstance"), "ConnectionString"),
                    new CodeFieldReferenceExpression(null, "ConnectionString")));

            // connectionInstance.Open();
            connectionProperty.GetStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(null, "connectionInstance"),
                    "Open"));

            // return connectionInstance;
            connectionProperty.GetStatements.Add(
                 new CodeMethodReturnStatement(
                     new CodeFieldReferenceExpression(null, "connectionInstance")));

            // set { connectionInstance = value; }
            connectionProperty.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(null, "connectionInstance"),
                        new CodePropertySetValueReferenceExpression()));

            var createCommandMethod = new CodeMemberMethod();
            createCommandMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            createCommandMethod.ReturnType = new CodeTypeReference(typeof(SqlCeCommand));
            createCommandMethod.Name = "CreateCommand";
            GenerateXmlDoc(createCommandMethod.Comments, "Create a SqlCeCommand instance using the global SQL CE Conection instance");

            createCommandMethod.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(null, "Connection"),
                        "CreateCommand")));

            type.Members.Add(lockField);
            type.Members.Add(connStrField);
            type.Members.Add(connField);
            type.Members.Add(connectionProperty);
            type.Members.Add(createCommandMethod);
            codeNamespace.Types.Add(type);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            provider.Dispose();
        }

        #endregion
    }

    public class CSharpCodeDomCodeGenerator : CodeDomCodeGenerator
    {
        public CSharpCodeDomCodeGenerator(SqlCeDatabase database)
            : base(database, new CSharpCodeProvider())
        {
        }

        public override string GetCode()
        {
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            using (var writer = new StringWriter(code))
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            return code.ToString();
        }
    }

    public class VisualBasicCodeDomCodeGenerator : CodeDomCodeGenerator
    {
        public VisualBasicCodeDomCodeGenerator(SqlCeDatabase database)
            : base(database, new VBCodeProvider())
        {
        }
    }
}