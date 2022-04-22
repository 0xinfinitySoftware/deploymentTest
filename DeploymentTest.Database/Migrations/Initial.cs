using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace DeploymentTest.Database.Migrations
{
    [Migration(1)]
    [ExcludeFromCodeCoverage]
    // ReSharper disable once InconsistentNaming
    public class InitialMigration : Migration
    {
        private const string SchemaName = "dbo";
        private const string TableName = "test";


        public override void Up()
        {
            Create.Schema(SchemaName);

            Create.Table(TableName).InSchema(SchemaName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ObjectName").AsString(50).NotNullable()
                .WithColumn("ObjectId").AsString(50).NotNullable()
                .WithColumn("Hash").AsInt32().NotNullable();
         
        }

        public override void Down()
        {
            Delete.Table(TableName).InSchema(SchemaName);
            Delete.Schema(SchemaName);
        }
    }
}
