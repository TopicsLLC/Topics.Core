#region License

/*
 * Copyright 2012-2018 Topics, LLC.
 *
 * Licensed under strict accordance with the Topics, LLC. License Agreement
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Topics.Core.Configuration
{
    public static class ExpressionEvaluator 
    {

        public static string Eval(string sCSCode, dynamic Solution, dynamic Environment, dynamic Server)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.core.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");
            cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            cp.ReferencedAssemblies.Add(path);

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.IO;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using Topics.Core.Models;\n");
            sb.Append("using System.Dynamic;\n");


            sb.Append("namespace CSCodeEvaler{ \n");

            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("private string _underscore_ = \"_\";");
            sb.Append("public object EvalCode(dynamic Solution, dynamic Environment, dynamic Server){\n");
            sb.Append("try { \n");
            sb.Append("return " + sCSCode + "; \n");
            sb.Append("} catch(Exception ex) { return ex.Message; } \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                return "ERROR: " + cr.Errors[0].ErrorText;
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, new object[] { Solution, Environment, Server });
            return s.ToString();
        }

        public static string Eval(string sCSCode, dynamic Variables, dynamic Solution, dynamic SolutionProject, dynamic Environment)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.core.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");
            cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

            string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            cp.ReferencedAssemblies.Add(path);

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.IO;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using Topics.Core.Models;\n");
            sb.Append("using System.Dynamic;\n");


            sb.Append("namespace CSCodeEvaler{ \n");

            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("private string _underscore_ = \"_\";");
            sb.Append("public object EvalCode(dynamic Variables, dynamic Solution, dynamic SolutionProject, dynamic Environment){\n");
            sb.Append("try { \n");
            sb.Append("return " + sCSCode + "; \n");
            sb.Append("} catch(Exception ex) { return ex.Message; } \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                return "ERROR: " + cr.Errors[0].ErrorText;
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, new object[] { Variables, Solution, SolutionProject, Environment });
            return s.ToString();
        }

        public static string Eval(string sCSCode, dynamic Variables, dynamic Solution, dynamic SolutionProject, dynamic Environment, dynamic Server)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.core.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");
            cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            cp.ReferencedAssemblies.Add(path);

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.IO;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using Topics.Core.Models;\n");
            sb.Append("using System.Dynamic;\n");


            sb.Append("namespace CSCodeEvaler{ \n");

            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("private string _underscore_ = \"_\";");
            sb.Append("public object EvalCode(dynamic Variables, dynamic Solution, dynamic SolutionProject, dynamic Environment, dynamic Server){\n");
            sb.Append("try { \n");
            sb.Append("return " + sCSCode + "; \n");
            sb.Append("} catch(Exception ex) { return ex.Message; } \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                return "ERROR: " + cr.Errors[0].ErrorText;
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, new object[] { Variables, Solution, SolutionProject, Environment, Server });
            return s.ToString();
        }
    }
}
