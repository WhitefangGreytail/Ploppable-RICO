using ICities;
using UnityEngine;


namespace PloppableRICO
{
#if DEBUG
    //[ProfilerAspect()]
#endif
    public class PloppableRICOMod : IUserMod
	{
		public string Name
		{
			get
			{
                return "Ploppable RICO";
			}
		}
		public string Description
		{
			get
			{
                return "Allows Plopping of RICO Buildings";
			}
		}
	}
}
