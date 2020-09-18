using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbench
{
    class Settings
    {
		private static Settings instance;

		public static Settings Instance
		{
			get 
			{
				if (instance == null)
					instance = new Settings();

				return instance; 
			}
			set { instance = value; }
		}

		public int AutosaveTime { get; set; } = 3000; 
    }
}
