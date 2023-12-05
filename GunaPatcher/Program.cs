using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpCodes = dnlib.DotNet.Emit.OpCodes;

namespace GunapPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
			var module = ModuleDefMD.Load(args[0]);
			var newLocation = module.Location.Replace(module.Name, "Cracked");

			if (module.Name != "Guna.UI2" && module.Assembly.Name != "Guna.UI2" && module.Name != "Guna.Charts.WinForms.dll" && module.Assembly.Name != "Guna.Charts.WinForms")
				Console.WriteLine("Are you doing this on purpose ?");
			else
			{
				bool cracked = false;
				Console.WriteLine($"[Loaded] {module.Assembly.Name} | {module.Assembly.Version}");
				foreach (TypeDef type in module.Types)
				{
					foreach (TypeDef nt2 in type.NestedTypes.Where((TypeDef nt) => nt.HasMethods && nt.Methods.Count() == 5 && nt.Fields.Count() == 5))
					{
						foreach (MethodDef method in nt2.Methods.Where((MethodDef m) => m.HasBody && m.Body.HasInstructions && m.Body.Instructions.Count() > 300))
						{
							method.Body.Variables.Clear();
							method.Body.Instructions.Clear();
							method.Body.ExceptionHandlers.Clear();
							method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "Cracked by Soheil MV"));
							method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "https://t.me/MVSoft_ir"));
							method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, module.Import(typeof(MessageBox).GetMethod("Show", new Type[] { typeof(string), typeof(string) }))));
							method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

							cracked = true;

							Console.WriteLine($"{method.Name} Cracked!");
						}
					}
				}

				if (cracked)
				{
					if (!Directory.Exists(newLocation))
						Directory.CreateDirectory(newLocation);

					module.Write($"{newLocation}\\{module.Name}");

					Console.ReadKey();
				}
			}
		}
    }
}