using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using System;

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
