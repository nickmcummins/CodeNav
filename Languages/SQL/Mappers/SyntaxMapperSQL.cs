using CodeNav.Helpers;
using CodeNav.Models;
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
                    Members = Enumerable.Empty<CodeItem>().ToList()
                }
            };

        }
    }
}
