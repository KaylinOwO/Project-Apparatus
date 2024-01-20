using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ProjectApparatus.Properties
{

	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		internal Resources()
		{
		}

		// (get) Token: 0x06000025 RID: 37 RVA: 0x00003254 File Offset: 0x00001454
		
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("ProjectApparatus.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// (get) Token: 0x06000026 RID: 38 RVA: 0x00003280 File Offset: 0x00001480
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00003287 File Offset: 0x00001487
		
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;
	}
}
