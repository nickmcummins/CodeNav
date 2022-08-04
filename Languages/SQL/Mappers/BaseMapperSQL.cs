using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CodeNav.Languages.SQL.Mappers
{
    public class BaseMapperSQL
    {
        public static T MapBase<T>(SqlCodeObject member, ICodeViewUserControl control)
        {
            var element = Activator.CreateInstance<T>();

            return element;
        }
    }
}
