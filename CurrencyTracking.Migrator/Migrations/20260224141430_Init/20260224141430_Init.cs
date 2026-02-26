using FluentMigrator;

namespace CurrencyTracking.UserApi.Migrations;

[Migration(20260224141430)]
public class Init : Migration
{
	public override void Up()
	{
		Execute.EmbeddedScript("20260224141430_Init_Up.sql");
	}

	public override void Down()
	{
		Execute.EmbeddedScript("20260224141430_Init_Down.sql");
	}
}
