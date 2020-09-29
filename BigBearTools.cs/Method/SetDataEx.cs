using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateAreaBoundary
{
    public static class SetDataEx
    {
        public static string AllGuid = "28E00BD9-4258-4642-8ECC-4C07779844B7";

        /// <summary>
        /// 扩展存储，记录string信息到Element里面（Transaction在主程序里设定）
        /// </summary>
        /// <param name="element"></param>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetData(this Element element, string name, string data, string guid)
        {
            try
            {
                //Guid 为全局统一标识
                //SchemaBuilder这个类用来存储外部扩展
                SchemaBuilder schemaBuilder = new SchemaBuilder(new Guid(guid));
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetSchemaName("uBIM");
                //在这里添加多个
                schemaBuilder.AddSimpleField(name, typeof(string));
                Schema schema = schemaBuilder.Finish();
                Entity entity = new Entity(schema);
                Field field = schema.GetField(name);
                entity.Set(field, data);
                element.SetEntity(entity);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
                return false;
            }
        }

    }
}
