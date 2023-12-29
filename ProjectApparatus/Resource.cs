using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ProjectApparatus
{

	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resource
	{
		internal Resource()
		{
		}

		// (get) Token: 0x0600001D RID: 29 RVA: 0x000030A4 File Offset: 0x000012A4

		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resource.resourceMan == null)
				{
					Resource.resourceMan = new ResourceManager("ProjectApparatus.Resource", typeof(Resource).Assembly);
				}
				return Resource.resourceMan;
			}
		}

		// (get) Token: 0x0600001E RID: 30 RVA: 0x000030D0 File Offset: 0x000012D0
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000030D7 File Offset: 0x000012D7

		internal static CultureInfo Culture
		{
			get
			{
				return Resource.resourceCulture;
			}
			set
			{
				Resource.resourceCulture = value;
			}
		}

		// (get) Token: 0x06000020 RID: 32 RVA: 0x000030DF File Offset: 0x000012DF
		internal static byte[] _0Harmony
		{
			get
			{
				return (byte[])Resource.ResourceManager.GetObject("_0Harmony", Resource.resourceCulture);
			}
		}

		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;
	}
}
