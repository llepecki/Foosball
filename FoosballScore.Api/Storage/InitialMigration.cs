using FluentMigrator;
using FoosballScore.Api.ReadModels;
using FoosballScore.Domain;

namespace FoosballScore.Api.Storage;

[Migration(0, "Initial migration")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(GameReadModel)).Exists())
        {
            Create
                .Table(nameof(GameReadModel))
                .WithColumn(nameof(GameReadModel.AggregateId)).AsString(64).NotNullable()
                .WithColumn(nameof(GameReadModel.Id)).AsGuid().NotNullable()
                .WithColumn(nameof(GameReadModel.Name)).AsAnsiString(Constraints.NameMaxLength).NotNullable()
                .WithColumn(nameof(GameReadModel.CurrentSet)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.FirstSetWinner)).AsAnsiString(Constraints.TeamMaxLength).Nullable()
                .WithColumn(nameof(GameReadModel.SecondSetWinner)).AsAnsiString(Constraints.TeamMaxLength).Nullable()
                .WithColumn(nameof(GameReadModel.ThirdSetWinner)).AsAnsiString(Constraints.TeamMaxLength).Nullable()
                .WithColumn(nameof(GameReadModel.FirstSetBlueGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.FirstSetRedGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.SecondSetBlueGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.SecondSetRedGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.ThirdSetBlueGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.ThirdSetRedGoals)).AsInt32().NotNullable()
                .WithColumn(nameof(GameReadModel.GameWinner)).AsAnsiString(Constraints.TeamMaxLength).Nullable()
                .WithColumn(nameof(GameReadModel.Created)).AsDateTimeOffset().NotNullable()
                .WithColumn(nameof(GameReadModel.Modified)).AsDateTimeOffset().NotNullable()
                .WithColumn(nameof(GameReadModel.Version)).AsInt32().NotNullable();

            Create
                .PrimaryKey($"PK_{nameof(GameReadModel)}_{nameof(GameReadModel.AggregateId)}")
                .OnTable(nameof(GameReadModel))
                .Column(nameof(GameReadModel.AggregateId));

            Create
                .Index($"IX_{nameof(GameReadModel)}_{nameof(GameReadModel.Id)}")
                .OnTable(nameof(GameReadModel))
                .OnColumn(nameof(GameReadModel.Id)).Ascending();

            Create
                .Index($"IX_{nameof(GameReadModel)}_{nameof(GameReadModel.Created)}")
                .OnTable(nameof(GameReadModel))
                .OnColumn(nameof(GameReadModel.Created)).Ascending();
        }
    }

    public override void Down()
    {
    }
}