using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using ColossalFramework.Plugins;
using System;
using UnityEngine;
using ICities;

namespace PloppableRICO
{

	public class PloppableResidential2 : ResidentialBuildingAI
	{
		public int m_levelmin = 1;
		public int m_levelmax = 1;
		public int m_constructionCost = 1;
		public int m_households = 1;
		public string m_name = "Residential";
		BuildingData Bdata;
		int leveltimer = 20;
		public byte m_levelprog = 0;

		public override void GetWidthRange (out int minWidth, out int maxWidth)
		{
			base.GetWidthRange (out minWidth, out maxWidth);
			minWidth = 1;
			maxWidth = 32;
		}

		public override void GetLengthRange (out int minLength, out int maxLength)
		{
			base.GetLengthRange (out minLength, out maxLength);
			minLength = 1;
			maxLength = 16;
		}

		public override string GenerateName(ushort buildingID, InstanceID caller){ 
			return name;
		}
			
		public override bool ClearOccupiedZoning (){
			return true;
		}

		public override int GetConstructionCost()
		{
			int result = (m_constructionCost * 100);
			Singleton<EconomyManager>.instance.m_EconomyWrapper.OnGetConstructionCost(ref result, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
			return result;
		}

		public override int CalculateHomeCount (Randomizer r, int width, int length)
		{
			width = m_households;
			length = 1;

			int num = 100;

			return Mathf.Max(100, width * length * num + r.Int32(100u)) / 100;
		}

		public override void SimulationStep (ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
		{
			BuildingData[] dataArray = BuildingDataManager.buildingData;		
			Bdata = dataArray [(int)buildingID];

			if (Bdata == null) {
						
				Bdata = new BuildingData ();
				dataArray [(int)buildingID] = Bdata;
				Bdata.level = m_levelmin;
				Bdata.Name = buildingData.Info.name;
				Bdata.saveflag = false;
			}

			buildingData.m_flags &= ~Building.Flags.Demolishing;
			buildingData.m_flags &= ~Building.Flags.ZonesUpdated;

		
			if (Bdata.saveflag == false) {
				
				if (Bdata.level == 1) {
					//buildingData.Info.m_buildingAI.m_info = buildingData.Info;
				}

				if (Bdata.level == 2) {
					//buildingData.Info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level2");
					buildingData.Info.m_buildingAI.m_info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level2");
				}
				if (Bdata.level == 3) {

					buildingData.Info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level3");
		
				}
				if (Bdata.level == 4) {

					buildingData.Info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level4");
		
				}
				if (Bdata.level == 5) {

					buildingData.Info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level5");
		
				}
				Bdata.saveflag = true;
			}

			if (m_levelmax == Bdata.level) { ///If Its reached max level, then dont level up. 
				//buildingData.m_levelUpProgress = (byte)0;
			}
				

			buildingData.m_garbageBuffer = 0;
			buildingData.m_fireHazard = 0;
			buildingData.m_fireIntensity = 0;

			base.SimulationStep (buildingID, ref buildingData, ref frameData);

		}

		public override BuildingInfo GetUpgradeInfo (ushort buildingID, ref Building data)
		{
			
			if (data.Info.m_class.m_level == ItemClass.Level.Level1) {

				//Bdata.level = 2;
				//data.Info.m_buildingAI.m_info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level2");

				return PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level2");

			} else if (data.Info.m_class.m_level == ItemClass.Level.Level2) {

				//Bdata.level = 3;
				//data.Info.m_buildingAI.m_info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level3");
				return PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level3");

			} else if (data.Info.m_class.m_level == ItemClass.Level.Level3) {
				//data.Info.m_buildingAI.m_info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level4");
				//Bdata.level = 4;
				return PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level4");

			} else if (data.Info.m_class.m_level == ItemClass.Level.Level4) {
				//data.Info.m_buildingAI.m_info = PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level5");
				//Bdata.level = 5;
				return PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name + "_Level5");

			} else
				return PrefabCollection<BuildingInfo>.FindLoaded (Bdata.Name);
		}
	}
}