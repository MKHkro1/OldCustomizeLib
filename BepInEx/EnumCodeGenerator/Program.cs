using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EnumCodeGenerator
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("请输入dll路径：");
            string dllPath = Console.ReadLine();

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"错误: 找不到文件 {dllPath}");
                return;
            }
            Console.WriteLine("请输入要读取多少个Enum：");
            int count = int.Parse(Console.ReadLine());
            List<string> enumNames = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                var enumName = Console.ReadLine();
                enumNames.Add(enumName);
            }

            try
            {
                // 加载DLL
                Assembly assembly = Assembly.LoadFrom(dllPath);
                var output = new StringBuilder();

                foreach (string enumName in enumNames)
                {
                    // 查找枚举类型
                    Type enumType = FindEnumType(assembly, enumName);
                    if (enumType == null)
                    {
                        Console.WriteLine($"警告: 未找到枚举类型 {enumName}");
                        continue;
                    }

                    // 生成代码
                    string code = GenerateEnumCode(enumType);
                    output.AppendLine(code);
                    output.AppendLine(); // 添加空行分隔

                    Console.WriteLine($"已处理枚举: {enumName}");
                }

                // 写入输出文件
                string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");
                if (!File.Exists(outputPath))
                    File.Create(outputPath).Dispose();
                File.WriteAllText(outputPath, output.ToString(), Encoding.UTF8);

                Console.WriteLine($"\n代码已生成到: {outputPath}");
                Console.WriteLine($"处理了 {enumNames.Count} 个枚举类型");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部错误: {ex.InnerException.Message}");
                }
            }
        }

        static Type FindEnumType(Assembly assembly, string enumName)
        {
            // 尝试精确匹配
            Type type = assembly.GetType(enumName, false);
            if (type != null && type.IsEnum)
                return type;

            // 如果没有命名空间，尝试搜索所有类型
            foreach (Type t in assembly.GetTypes())
            {
                if (t.Name == enumName && t.IsEnum)
                    return t;
            }

            // 尝试带有命名空间的搜索
            foreach (Type t in assembly.GetTypes())
            {
                if (t.FullName == enumName && t.IsEnum)
                    return t;
            }

            return null;
        }

        static string GenerateEnumCode(Type enumType)
        {
            StringBuilder code = new StringBuilder();

            // 获取枚举的所有值和名称
            Array values = Enum.GetValues(enumType);
            string[] names = Enum.GetNames(enumType);

            // 生成方法头
            code.AppendLine($"public static {enumType.Name} Get{enumType.Name}ByString(string name)");
            code.AppendLine("{");
            code.AppendLine("    var id = -1;");
            code.AppendLine("    #region 映射");
            code.AppendLine("    switch (name)");
            code.AppendLine("    {");

            // 生成switch case
            for (int i = 0; i < values.Length; i++)
            {
                string name = names[i];
                object value = values.GetValue(i);
                int intValue = Convert.ToInt32(value);

                code.AppendLine($"        case \"{name}\":");
                code.AppendLine($"            id = {intValue};");
                code.AppendLine("            break;");
            }

            // 关闭switch和方法
            code.AppendLine("    }");
            code.AppendLine("    #endregion");
            code.AppendLine($"    return ({enumType.Name})id;");
            code.AppendLine("}");

            return code.ToString();
        }
    }
}