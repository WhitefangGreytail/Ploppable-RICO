using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace PloppableRICO
{
	public class ExtendedLoading : LoadingExtensionBase
	{
		public PloppableTool PloppableTool;
		System.Collections.Generic.List<string[]> BNames = new System.Collections.Generic.List<string[]>{}; //This passes a list of prefab names from LoadRICO to PloppableTool. 
		static GameObject TabUpdaterObject;
		TabUpdater TabUpdater;
		LoadRICO LoadXML = new LoadRICO();
		InfoTabs Tabs;
		UIPanel ZonedPanel;
		UIPanel ServicePanel;

		private LoadMode _mode;

		public override void OnLevelLoaded (LoadMode mode)
		{
			base.OnLevelLoaded (mode);

			if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
				return;

			/////////////////////////////// Load XML settings and apply them to prefabs. Based on the Sub-Buildings Enabler mod. 

			LoadXML.Run (BNames);

			/////////////////////////////Load info panel tabs. This based on the Extended Building Info mod. 
	
			if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
				return;
			_mode = mode;

			/////////////Create tabs
			Tabs = (InfoTabs)UIView.GetAView().AddUIComponent(typeof(InfoTabs));

			//Zoned Panel
			ZonedPanel = UIView.Find<UIPanel> ("(Library) ZonedBuildingWorldInfoPanel");
			ZonedPanel.eventVisibilityChanged += ZonedPanel_eventVisibilityChanged; //Allign tabs to Zoned panel
		
			//Service panel
			ServicePanel = UIView.Find<UIPanel> ("(Library) CityServiceWorldInfoPanel");
			ServicePanel.eventVisibilityChanged += ServicePanel_eventVisibilityChanged;//Allign tabs to Service panel
			ServicePanel.size = new Vector2(436,321); //make service panel same size as zoned panel. 

			TabUpdaterObject = new GameObject ("TabUpdaterObject"); //This runs and updates the tabs 
			TabUpdater = TabUpdaterObject.AddComponent<TabUpdater> (); 
			TabUpdater.servicePanel = ServicePanel.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel> ();
			TabUpdater.zonedPanel = ZonedPanel.gameObject.transform.GetComponentInChildren<ZonedBuildingWorldInfoPanel> ();

		

			///////////////Load the RICO panel. This is based on the Terraform Tool mod. 
		
			if (PloppableTool == null) {
				GameObject gameController = GameObject.FindWithTag ("GameController");
				PloppableTool = gameController.AddComponent<PloppableTool> ();
				PloppableTool.name = "PloppableTool";
				PloppableTool.InitGui (BNames);
				PloppableTool.enabled = false;
				GameObject.FindObjectOfType<ToolController> ().Tools [0].enabled = true;
			}


			/////////////////Deploy BuildingTool Detour. This is based on the Larger Footprints mod. 
	
			Detour.BuildingToolDetour.Deploy ();

		}
			
		public override void OnReleased()
		{
			Detour.BuildingToolDetour.Revert();
		}

		public void ServicePanel_eventVisibilityChanged (UIComponent component, bool value)  //Service panel
		{
			Tabs.AlignTo (ServicePanel, UIAlignAnchor.TopLeft);
			Tabs.relativePosition = new Vector2 (13, -25);
			TabUpdater.ServiceLast = 1; //force tabs to update.
			TabUpdater.Update();
			//Debug.Log ("Service Updated");
		}

		public void ZonedPanel_eventVisibilityChanged (UIComponent component, bool value) //Zoned panel
		{

			Tabs.AlignTo (ZonedPanel, UIAlignAnchor.TopLeft);
			Tabs.relativePosition = new Vector2 (13, -25);
			TabUpdater.ZonedLast = 1; //force tabs to update.
			TabUpdater.Update();
			//Debug.Log ("Zoned Updated");

		}

		public override void OnLevelUnloading ()
		{
			if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
				return;

			//Clean up tabs

			GameObject.Destroy (Tabs);

			if (TabUpdaterObject != null) {
				GameObject.Destroy (TabUpdaterObject);

			}
		
		//RICO ploppables need a non private item class assigned to pass though the game reload. 

			for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount (); i++) {
				
				var prefab = PrefabCollection<BuildingInfo>.GetLoaded (i);

				if (prefab.m_buildingAI is PloppableRICO.PloppableExtractor || prefab.m_buildingAI  is PloppableResidential || prefab.m_buildingAI  is PloppableOffice || prefab.m_buildingAI is PloppableCommercial || prefab.m_buildingAI  is PloppableIndustrial) {

					prefab.m_class = ItemClassCollection.FindClass ("Beautification Item"); //Just assign any RICO prefab a ploppable Itemclass so it will load. It gets set back once the mod loads. 

					prefab.InitializePrefab ();
				}
			}
		}
	}
}
