using System.Linq;
using Zu.TypeScript.TsTypes;

namespace CodeNav.Shared.Languages.JavaScript.Mappers
{
    public static class IdMapperJS
    {
        public static string MapId(string name, NodeArray<ParameterDeclaration>? parameters)
        {
            if (parameters == null)
            {
                return name;
            }

            return name + string.Join(string.Empty, parameters.Select(p => p.IdentifierStr));
        }

    }
}
