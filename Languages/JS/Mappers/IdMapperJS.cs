using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.TypeScript.TsTypes;

namespace CodeNav.Languages.JS.Mappers
{
    public class IdMapperJS
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
