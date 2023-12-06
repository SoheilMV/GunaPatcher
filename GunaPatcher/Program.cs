using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpCodes = dnlib.DotNet.Emit.OpCodes;

namespace GunaPatcher
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "GunaPatcher";
			try
			{
                ModuleDefMD module = null;
                if (args.Length > 0)
				{
					module = ModuleDefMD.Load(args[0], ModuleDefMD.CreateModuleContext());
				}
				else
				{
					Console.Write("Path: ");
					string path = Console.ReadLine();
					Console.WriteLine();
					if (File.Exists(path))
					{
						module = ModuleDefMD.Load(path, ModuleDefMD.CreateModuleContext());
					}
				}

				var newLocation = module.Location.Replace(module.Name, "Cracked");

				if (module.Name != "Guna.UI2" && module.Assembly.Name != "Guna.UI2" && module.Name != "Guna.Charts.WinForms.dll" && module.Assembly.Name != "Guna.Charts.WinForms")
					Console.WriteLine("Are you doing this on purpose ?");
				else
				{
					GunaStatus status = GunaStatus.None;
					Console.WriteLine($"[Loaded] {module.Assembly.Name} | {module.Assembly.Version}");
					foreach (TypeDef type in module.Types.Where((TypeDef t) => t.HasNestedTypes))
					{
						foreach (TypeDef nestedType in type.NestedTypes.Where((TypeDef nt) => nt.HasMethods && nt.HasFields))
						{
							if ((CheckMethodsTypeSigCount(nestedType.Methods, module.CorLibTypes.Int32) == 1 &&
								CheckMethodsTypeSigCount(nestedType.Methods, module.CorLibTypes.Boolean) == 1 &&
								CheckMethodsTypeSigCount(nestedType.Methods, module.ImportAsTypeSig(typeof(Dictionary<string, string>))) == 1 &&
								CheckMethodsTypeSigCount(nestedType.Methods, module.CorLibTypes.Void) == 2) &&
								(CheckFieldsTypeSigCount(nestedType.Fields, module.CorLibTypes.Boolean) == 1 &&
								CheckFieldsTypeSigCount(nestedType.Fields, type) == 1 &&
								CheckFieldsTypeSigCount(nestedType.Fields, module.ImportAsTypeSig(typeof(Func<Dictionary<string, string>>))) == 1 &&
								 CheckFieldsTypeSigCount(nestedType.Fields, module.ImportAsTypeSig(typeof(Func<bool>))) == 1 &&
								 CheckFieldsTypeSigCount(nestedType.Fields, module.ImportAsTypeSig(typeof(Func<int>))) == 1))
							{
								foreach (MethodDef method in nestedType.Methods.Where((MethodDef m) => m.HasBody && m.Body.HasVariables && !m.HasReturnType))
								{
									method.Body.Variables.Clear();
									method.Body.Instructions.Clear();
									method.Body.ExceptionHandlers.Clear();
									method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "Cracked by TheHellTower | Soheil MV"));
									method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "Telegram : @TheHellTower | @MVSoft_ir"));
									method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, module.Import(typeof(MessageBox).GetMethod("Show", new Type[] { typeof(string), typeof(string) }))));
									method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

									status = GunaStatus.Cracked;

									Console.WriteLine($"{method.Name} Moded!");
								}
							}
						}
					}

					if (status == GunaStatus.Cracked)
					{
						if (!Directory.Exists(newLocation))
							Directory.CreateDirectory(newLocation);

						module.Write($"{newLocation}\\{module.Name}");
					}
					else
					{
						Console.WriteLine("The guna is not valid!");
					}
				}
			}
			catch
			{
				Console.WriteLine("The file is not valid!");
			}

			Console.ReadKey();
		}


		static int CheckMethodsTypeSigCount(IList<MethodDef> methods, TypeSig typeSig)
        {
			int count = 0;
			foreach (var method in methods)
			{
				if (method.ReturnType.FullName == typeSig.FullName)
					count++;
			}
			return count;
        }

		static int CheckFieldsTypeSigCount(IList<FieldDef> fields, TypeSig typeSig)
		{
			int count = 0;
			foreach (var field in fields)
			{
				if (field.FieldType.FullName == typeSig.FullName)
					count++;
			}
			return count;
		}

		static int CheckFieldsTypeSigCount(IList<FieldDef> fields, TypeDef typeDef)
		{
			int count = 0;
			foreach (var field in fields)
			{
				if (field.FieldType.FullName == typeDef.FullName)
					count++;
			}
			return count;
		}
	}
}