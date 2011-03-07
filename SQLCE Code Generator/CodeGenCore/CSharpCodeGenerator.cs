using System.Text;

namespace ChristianHelle.DatabaseTools.SqlCe.CodeGenCore
{
    public class CSharpCodeGenerator : CodeGenerator
    {
        public CSharpCodeGenerator(SqlCeDatabase tableDetails)
            : base(tableDetails)
        {
        }

        public override void GenerateEntities()
        {
            GenerateEntities(new EntityGeneratorOptions());
        }

        public override void GenerateEntities(EntityGeneratorOptions options)
        {
            code.AppendLine("\nnamespace " + Database.Namespace);
            code.AppendLine("{");

            GenerateEntityBase();

            foreach (var table in Database.Tables)
                GenerateEntity(table);

            code.AppendLine("}");
        }

        public override void GenerateDataAccessLayer()
        {
            GenerateDataAccessLayer(new DataAccessLayerGeneratorOptions());
        }

        public override void GenerateDataAccessLayer(DataAccessLayerGeneratorOptions options)
        {
            code.AppendLine("\nnamespace " + Database.Namespace);
            code.AppendLine("{");

            foreach (var table in Database.Tables)
            {
                code.AppendLine("\t#region " + table.TableName);

                code.AppendLine("\tpublic partial class " + table.TableName);
                code.AppendLine("\t{");

                var generator = new CSharpDataAccessLayerGenerator(code, table);
                if (options.GenerateSelectAll)
                    generator.GenerateSelectAll();
                if (options.GenerateSelectAllWithTop)
                    generator.GenerateSelectWithTop();
                if (options.GenerateSelectBy)
                    generator.GenerateSelectBy();
                if (options.GenerateSelectByWithTop)
                    generator.GenerateSelectByWithTop();
                if (options.GenerateCreateIgnoringPrimaryKey)
                    generator.GenerateCreateIgnoringPrimaryKey();
                if (options.GenerateCreateUsingAllColumns)
                    generator.GenerateCreateUsingAllColumns();
                if (options.GenerateDelete)
                    generator.GenerateDelete();
                if (options.GenerateDeleteBy)
                    generator.GenerateDeleteBy();
                if (options.GenerateDeleteAll)
                    generator.GenerateDeleteAll();
                if (options.GenerateSaveChanges)
                    generator.GenerateSaveChanges();

                code.AppendLine("\t}");
                code.AppendLine("\t#endregion");
                code.AppendLine();
            }

            code.AppendLine("}");
        }

        #region Generate Entities
        private void GenerateEntity(Table table)
        {
            code.AppendLine("\t#region " + table.TableName);

            code.AppendLine("\tpublic partial class " + table.TableName);
            code.AppendLine("\t{");

            foreach (var column in table.Columns)
            {
                if (column.Value.ManagedType.IsValueType)
                {
                    code.AppendLine("\t\tprivate " + column.Value.ManagedType + "? _" + column.Value.Name + ";");
                    code.AppendLine("\t\tpublic " + column.Value.ManagedType + "? " + column.Value.Name);
                }
                else
                {
                    code.AppendLine("\t\tprivate " + column.Value.ManagedType + " _" + column.Value.Name + ";");
                    code.AppendLine("\t\tpublic " + column.Value.ManagedType + " " + column.Value.Name);
                }

                code.AppendLine("\t\t{");
                code.AppendLine("\t\t\t get { return _" + column.Value.Name + "; }");
                code.AppendLine("\t\t\t set");
                code.AppendLine("\t\t\t{");
                code.AppendLine("\t\t\t\t_" + column.Value.Name + " = value;");

                if (column.Value.MaxLength > 0)
                {
                    if (column.Value.ManagedType.Equals(typeof(string)))
                    {
                        code.AppendLine("\t\t\t\tif (_" + column.Value.Name + ".Length > " + column.Value.MaxLength + ")");
                        code.AppendLine("\t\t\t\t\tthrow new System.ArgumentException(\"Max length for " + column.Value.Name + " is " + column.Value.MaxLength + "\");");
                    }
                }

                code.AppendLine("\t\t\t}");
                code.AppendLine("\t\t}");
            }
            code.AppendLine("\t}");

            code.AppendLine("\t#endregion");
            code.AppendLine();
        }

        private void GenerateEntityBase()
        {
            code.AppendLine("\t#region EntityBase");
            code.AppendLine("\tpublic static class EntityBase");
            code.AppendLine("\t{");
            code.AppendLine("\t\tpublic static System.String ConnectionString { get; set; }");
            code.AppendLine();
            code.AppendLine("\t\tprivate static System.Data.SqlServerCe.SqlCeConnection connectionInstance = null;");
            code.AppendLine("\t\tpublic static System.Data.SqlServerCe.SqlCeConnection Connection");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\tget");
            code.AppendLine("\t\t\t{");
            code.AppendLine("\t\t\t\tif (connectionInstance == null)");
            code.AppendLine("\t\t\t\t\tconnectionInstance = new System.Data.SqlServerCe.SqlCeConnection(ConnectionString);");
            code.AppendLine("\t\t\t\tif (connectionInstance.State != System.Data.ConnectionState.Open)");
            code.AppendLine("\t\t\t\t\tconnectionInstance.Open();");
            code.AppendLine("\t\t\t\treturn connectionInstance;");
            code.AppendLine("\t\t\t}");
            code.AppendLine("\t\t}");
            code.AppendLine();
            code.AppendLine("\t\tpublic static System.Data.SqlServerCe.SqlCeCommand CreateCommand()");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\treturn Connection.CreateCommand();");
            code.AppendLine("\t\t}");
            code.AppendLine("\t}");
            code.AppendLine("\t#endregion");
            code.AppendLine();
        }
        #endregion
    }
}