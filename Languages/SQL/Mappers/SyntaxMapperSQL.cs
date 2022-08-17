using CodeNav.Helpers;
using CodeNav.Languages.SQL.Models;
using CodeNav.Models;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Microsoft.VisualStudio.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using SqlParser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser;

namespace CodeNav.Languages.SQL.Mappers
{
    public class SyntaxMapperSQL
    {
        private static ICodeViewUserControl? _control;
        public static List<CodeItem?> Map(string filePath, ICodeViewUserControl control, string? sqlString = null)
        {
            _control = control;
            sqlString ??= File.ReadAllText(filePath);

            var sqlScript = SqlParser.Parse(sqlString).Script;
            return new List<CodeItem?>()
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    BorderColor = Colors.DarkGray,
                    Moniker = KnownMonikers.JSONScript,
                    ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1,
                    Members = sqlScript.Children.SelectMany(child => MapMember(child)).ToList()
                }
            };

        }

        private static List<CodeItem> MapMember(SqlCodeObject member)
        {
            switch (member.GetType().Name)
            {
                case "SqlBatch":
                    return member.Children.SelectMany(child => MapMember(child)).ToList();
                case "SqlVariableDeclareStatement":
                    return MapVariableDeclareStatement(member as SqlVariableDeclareStatement);
            }

            return CodeItem.EmptyList;
        }

        private static List<CodeItem> MapVariableDeclareStatement(SqlVariableDeclareStatement sqlVariableDeclareStatement)
        {
            var declareVariableBlock = BaseMapperSQL.MapBase<SqlBlockItem>(sqlVariableDeclareStatement, _control);
            declareVariableBlock.Name = sqlVariableDeclareStatement.Sql.Length > 50 ? sqlVariableDeclareStatement.Sql.Substring(0, 50) : sqlVariableDeclareStatement.Sql;

            return null;
        }
    }
}
