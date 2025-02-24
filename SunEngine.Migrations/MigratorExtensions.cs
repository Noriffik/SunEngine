using System;
using FluentMigrator.Builders.Create.Table;

namespace SunEngine.Migrations.Migrations
{
    internal static class MigratorExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax AsMaxString(
            this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(Int32.MaxValue);
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsMyDateTime(
            this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            if (!DbProvider.IsPostgre)
                return createTableColumnAsTypeSyntax.AsDateTime();

            return createTableColumnAsTypeSyntax.AsCustom("TimestampTz");
        }
    }
}