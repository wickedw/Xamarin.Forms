using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil.Cil;

using static Mono.Cecil.Cil.Instruction;
using static Mono.Cecil.Cil.OpCodes;

using Xamarin.Forms.Build.Tasks;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Core.XamlC
{
	class StyleSheetConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ILContext context, BaseNode node)
		{
			var module = context.Body.Method.Module;
			var body = context.Body;

			INode rootNode = node;
			while (!(rootNode is ILRootNode))
				rootNode = rootNode.Parent;

			var rootTargetPath = RDSourceTypeConverter.GetPathForType(module, ((ILRootNode)rootNode).TypeReference);
			var uri = new Uri(value, UriKind.Relative);

			var resourceId = ResourceDictionary.RDSourceTypeConverter.GetResourceId(uri, rootTargetPath, s => RDSourceTypeConverter.GetResourceIdForPath(module, s));

			//return StyleSheet.Parse(rootObjectType.GetTypeInfo().Assembly, resourceId, lineInfo);

			var getTypeFromHandle = module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));
			var getAssembly = module.ImportReference(typeof(Type).GetProperty("Assembly").GetGetMethod());
			yield return Create(Ldtoken, module.ImportReference(((ILRootNode)rootNode).TypeReference));
			yield return Create(Call, module.ImportReference(getTypeFromHandle));
			yield return Create(Callvirt, module.ImportReference(getAssembly)); //assembly

			yield return Create(Ldstr, resourceId); //resourceId

			foreach (var instruction in node.PushXmlLineInfo(context))
				yield return instruction; //lineinfo

			var styleSheetParse = module.ImportReference(typeof(StyleSheets.StyleSheet).GetMethods().FirstOrDefault(mi => mi.Name == "Parse" && mi.GetParameters().Length == 3));
			yield return Create(Call, module.ImportReference(styleSheetParse));
		}
	}
}