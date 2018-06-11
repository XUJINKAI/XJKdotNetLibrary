using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Reflection;

namespace XJK
{
    public static class ObjectDumper
    {
        public static string Dump(this object obj, int maxDepth = 4)
        {
            ObjectDumperWriter dumper = new ObjectDumperWriter(4);
            string name = $"<{obj?.GetType().FullName ?? null}>";
            DumpInner(dumper, name, obj, 0, maxDepth);
            return dumper.ToString();
        }

        private static void DumpInner(ObjectDumperWriter dw, string name, object obj, int depth, int maxDepth)
        {
            if (depth > maxDepth) { dw.Add(1, "......"); return; }
            dw.Add(0, name);
            if (obj == null || obj is ValueType || obj is string)
            {
                dw.Add(1, $" = {obj ?? "null"}");
                dw.Add(3, (obj?.GetType().Name ?? "null"));
            }
            else
            {
                if (obj is IEnumerable list)
                {
                    int count = 0;
                    foreach (var item in list)
                    {
                        count++;
                    }
                    if (obj.GetType().IsArray)
                    {
                        dw.Add(1, $"   {obj.GetType().GetElementType().FullName}[{count}]");
                    }
                    else if (obj.GetType().IsGenericType)
                    {
                        dw.Add(1, $"   {obj.GetType().GenericTypeArguments.Join(o => o.FullName)}[{count}]");
                    }
                    else
                    {
                        dw.Add(1, "   " + obj.GetType().FullName + "{" + count.ToString() + "}");
                    }
                    if (depth + 1 <= maxDepth)
                    {
                        dw.Indent++;
                        foreach (var item in list)
                        {
                            if(item is DictionaryEntry entry)
                            {
                                DumpInner(dw, "- " + entry.Key.ToString(), entry.Value, depth + 1, maxDepth);
                            }
                            else
                            {
                                DumpInner(dw, "-", item, depth + 1, maxDepth);
                            }
                        }
                        dw.Indent--;
                    }
                }
                else
                {
                    dw.Add(3, $"{obj.GetType().FullName}");
                    if (depth + 1 <= maxDepth)
                    {
                        Type type = obj as Type;
                        if (type != null) dw.Add(1, type.FullName);
                        dw.Indent++;
                        Dictionary<string, object> ObjectCache = new Dictionary<string, object>();
                        var members = (type == null) ? obj.GetType().GetMembers() : type.GetMembers();
                        foreach (var mem in members)
                        {
                            if (mem is PropertyInfo || mem is FieldInfo)
                            {
                                PropertyInfo prop = mem as PropertyInfo;
                                FieldInfo field = mem as FieldInfo;
                                if (prop != null)
                                {
                                    if (prop.GetMethod.IsStatic ^ (obj is Type))
                                    {
                                        continue;
                                    }
                                }
                                object memberValue = null;
                                if (field != null) memberValue = field.GetValue(obj);
                                try
                                {
                                    if (prop != null) memberValue = prop.GetValue(obj);
                                }
                                catch (Exception ex)
                                {
                                    dw.Add(0, mem.Name);
                                    dw.Add(1, ex.Message);
                                    dw.Add(3, (prop.GetMethod.ReturnType.FullName));
                                    continue;
                                }
                                if (memberValue == null || memberValue is ValueType || memberValue is string)
                                {
                                    DumpInner(dw, mem.Name, memberValue, depth + 1, maxDepth);
                                }
                                else
                                {
                                    ObjectCache.Add(mem.Name, memberValue);
                                }
                            }
                        }
                        foreach (var kv in ObjectCache)
                        {
                            DumpInner(dw, kv.Key, kv.Value, depth + 1, maxDepth);
                        }
                        dw.Indent--;
                    }
                    else
                    {
                        dw.Add(1, "   " + obj.ToString());
                    }
                }
            }
        }
    }
}
