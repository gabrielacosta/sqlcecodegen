using System;

namespace ChristianHelle.DatabaseTools.SqlCe.CodeGenCore
{
    public class VisualBasicCodeGenerator : CodeGenerator
    {
        private const string NOT_SUPPORTED = "Only C# code generation is currently supported";

        public VisualBasicCodeGenerator(SqlCeDatabase database)
            : base(database)
        {
        }

        public override void GenerateEntities()
        {
            throw new NotSupportedException(NOT_SUPPORTED);
        }

        public override void GenerateDataAccessLayer()
        {
            throw new NotSupportedException(NOT_SUPPORTED);
        }

        public override void GenerateEntities(EntityGeneratorOptions options)
        {
            throw new NotSupportedException(NOT_SUPPORTED);
        }

        public override void GenerateDataAccessLayer(DataAccessLayerGeneratorOptions options)
        {
            throw new NotSupportedException(NOT_SUPPORTED);
        }
    }
}