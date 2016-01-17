﻿using ColossalFramework.IO;
using ColossalFramework;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Math;
using System.Threading;


namespace PloppableRICO
{
	/// <summary>
	/// This class draws the RICO panel, populates it with building buttons, and activates the building tool when buttons are clicked. 
	/// </summary>
	/// 
	public class PloppableTool : ToolBase
	{
		UIButton PloppableButton;
		BuildingInfo BuildingIm;
		UIPanel BuildingPanel;
		UIPanel Tabs;
		bool m_active;
		UIButton BBut;
	
		UISprite resbutton;
		UISprite combutton;
		UISprite offbutton;
		UISprite indbutton;


		int types = 9;
		UISprite TabSprite;
		UISprite[] TabSprites = new UISprite[10];
		UIScrollablePanel[] BuildingPanels = new UIScrollablePanel[10];
		UIButton[] TabButtons = new UIButton[10];

		string[] Names = new string[] {
		
			"ResidentialLow",
			"ResidentialHigh",
			"CommercialLow",
			"CommercialHigh",
			"Office",
			"Industrial",
			"Farming",
			"Forest",
			"Oil",
			"Ore",
		};

		int taboffset = 63;
		int taboffsetstart = 5;


		public void InitGui (List<string[]> BuildingNames)
		{

			UIView uiView = UIView.GetAView ();
			UIComponent refButton = uiView.FindUIComponent ("Policies");
			UIComponent tsBar = uiView.FindUIComponent ("TSBar");


			if (PloppableButton == null) {

				PloppableButton = UIView.GetAView ().FindUIComponent<UITabstrip> ("MainToolstrip").AddUIComponent<UIButton> (); //main button on in game tool strip. Need to add better sprite. 
				PloppableButton.size = new Vector2 (43, 49);
				PloppableButton.eventClick += PloppablebuttonClicked;
				PloppableButton.normalBgSprite = "ZoningOptionMarquee";
				PloppableButton.focusedFgSprite = "ToolbarIconGroup6Focused";
				PloppableButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
				PloppableButton.relativePosition = new Vector2 (800, 0);
				PloppableButton.name = "PloppableButton";


				// failed experiment with sprites. 
				/*
				resbutton = PloppableButton.AddUIComponent<UISprite>();
				combutton =  PloppableButton.AddUIComponent<UISprite>();
				indbutton =  PloppableButton.AddUIComponent<UISprite>();
				offbutton =  PloppableButton.AddUIComponent<UISprite>();

				resbutton.atlas = UIView.GetAView ().FindUIComponent<UIButton> ("CommercialLow").atlas;
				resbutton.relativePosition = new Vector2 (4, 4);
				resbutton.spriteName = "ZoningResidentialHigh";
				resbutton.size = new Vector2 (25, 18);

				combutton.atlas = UIView.GetAView ().FindUIComponent<UIButton> ("CommercialLow").atlas;
				combutton.relativePosition = new Vector2 (4, 20);
				combutton.spriteName = "ZoningCommercialHigh";
				combutton.size = new Vector2 (25, 18);

				offbutton.atlas = UIView.GetAView ().FindUIComponent<UIButton> ("CommercialLow").atlas;
				offbutton.relativePosition = new Vector2 (20, 4);
				offbutton.spriteName = "ZoningOffice";
				offbutton.size = new Vector2 (25, 18);

				indbutton.atlas = UIView.GetAView ().FindUIComponent<UIButton> ("CommercialLow").atlas;
				indbutton.relativePosition = new Vector2 (20, 20);
				indbutton.spriteName = "ZoningIndustrial";
				indbutton.size = new Vector2 (25, 18);

*/

				BuildingPanel = UIView.GetAView ().FindUIComponent ("TSContainer").AddUIComponent<UIPanel> (); //this is the base panel. 
				BuildingPanel.backgroundSprite = "SubcategoriesPanel";
				BuildingPanel.isVisible = false;
				BuildingPanel.name = "PloppableBuildingPanel";
				BuildingPanel.size = new Vector2 (859, 109);
				BuildingPanel.relativePosition = new Vector2 (0, 0);

				Tabs = UIView.GetAView ().FindUIComponent ("PloppableBuildingPanel").AddUIComponent<UIPanel> ();
				Tabs.size = new Vector2 (832, 25);
				Tabs.relativePosition = new Vector2 (13, -25);

				for (int i = 0; i <= types; i++) {

					BuildingPanels [i] = new UIScrollablePanel (); //draw scrollable panels
					BuildingPanels [i] = BuildingPanel.AddUIComponent<UIScrollablePanel> ();
					BuildingPanels [i].size = new Vector2 (763, 109);
					BuildingPanels [i].relativePosition = new Vector2 (50, 0);
					BuildingPanels [i].name = Names [i] + "Panel";
					BuildingPanels [i].isVisible = false;

					BuildingPanels [i].autoLayout = true;
					BuildingPanels [i].autoLayoutStart = LayoutStart.BottomLeft;
					BuildingPanels [i].builtinKeyNavigation = true;
					BuildingPanels [i].autoLayoutDirection = LayoutDirection.Horizontal;
					BuildingPanels [i].clipChildren = true;
					BuildingPanels [i].freeScroll = false;
	
					TabButtons [i] = new UIButton ();  //draw RICO tabstrip. 
					TabButtons [i] = Tabs.AddUIComponent<UIButton> ();
					TabButtons [i].size = new Vector2 (58, 25);
					TabButtons [i].relativePosition = new Vector2 (taboffsetstart, 0);
					TabButtons [i].normalBgSprite = "SubBarButtonBase";
					TabButtons [i].disabledBgSprite = "SubBarButtonBaseDisabled";
					TabButtons [i].pressedBgSprite = "SubBarButtonBasePressed";
					TabButtons [i].hoveredBgSprite = "SubBarButtonBaseHovered";
					TabButtons [i].focusedBgSprite = "SubBarButtonBaseFocused";
					TabButtons [i].state = UIButton.ButtonState.Normal;
					TabButtons [i].isEnabled = enabled;
					TabButtons [i].name = Names [i] + "Button";
					TabButtons [i].tabStrip = true;

					TabSprite = TabButtons [i].AddUIComponent<UISprite> ();

					if (i <= 5) {
						SetSprites (TabSprite, "Zoning" + Names [i]);

					} else {
						SetSprites (TabSprite, "DistrictSpecialization" + Names [i]);
					}
			
					taboffsetstart = taboffsetstart + taboffset;
				}
					
				//Couldnt get this to work in the loop.
		
				TabButtons [0].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [0]);
				TabButtons [1].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [1]);
				TabButtons [2].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [2]);
				TabButtons [3].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [3]);
				TabButtons [4].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [4]);
				TabButtons [5].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [5]);
				TabButtons [6].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [6]);
				TabButtons [7].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [7]);
				TabButtons [8].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [8]);
				TabButtons [9].eventClick += (sender, e) => TabClicked (sender, e, BuildingPanels [9]);


				string[][] Bdnames = new string[][]{ };
				Bdnames = BuildingNames.ToArray ();
				int Offset = 0;

				foreach (string[] test in Bdnames) { //this loops though BuildingNames, and draws a button for each one. 

					BuildingIm = PrefabCollection<BuildingInfo>.FindLoaded (test [0]);
					GenBButton (BuildingIm, Offset, test [1]);
					Offset = Offset + 109;
				}
					
			}
	
		}
//End of InitGUI


		public void DrawPanels (UIScrollablePanel panel, string name)
		{
			
			panel = UIView.GetAView ().FindUIComponent ("PloppableBuildingPanel").AddUIComponent<UIScrollablePanel> ();
			panel.size = new Vector2 (763, 109);
			panel.relativePosition = new Vector2 (50, 0);
			panel.Reset ();
		}

		public void SetSprites (UISprite labe, string sprite)
		{
			UISprite label = labe;
			label.atlas = UIView.GetAView ().FindUIComponent<UIButton> ("CommercialLow").atlas;
			label.relativePosition = new Vector2 (12, 0);
			label.spriteName = sprite;
			label.size = new Vector2 (35, 25);
		}


		void GenBButton (BuildingInfo BuildingIm, int Offset, string type)
		{

			BBut = new UIButton (); //draw button on appropriate panel. 
			if (type == "reslow") {
				BBut = BuildingPanels [0].AddUIComponent<UIButton> ();
			}
			if (type == "reshigh") {
				BBut = BuildingPanels [1].AddUIComponent<UIButton> ();
			}
			if (type == "comlow") {
				BBut = BuildingPanels [2].AddUIComponent<UIButton> ();
			}
			if (type == "comhigh") {
				BBut = BuildingPanels [3].AddUIComponent<UIButton> ();
			}
			if (type == "office") {
				BBut = BuildingPanels [4].AddUIComponent<UIButton> ();
			}
			if (type == "industrial") {
				BBut = BuildingPanels [5].AddUIComponent<UIButton> ();
			}
			if (type == "farming") {
				BBut = BuildingPanels [6].AddUIComponent<UIButton> ();
			}
			if (type == "oil") {
				BBut = BuildingPanels [8].AddUIComponent<UIButton> ();
			}
			if (type == "forest") {
				BBut = BuildingPanels [7].AddUIComponent<UIButton> ();
			}
			if (type == "ore") {
				BBut = BuildingPanels [9].AddUIComponent<UIButton> ();
			}

			BBut.size = new Vector2 (109, 100); //apply settings to building buttons. 
			BBut.atlas = BuildingIm.m_Atlas;
			BBut.normalFgSprite = BuildingIm.m_Thumbnail;
			BBut.focusedFgSprite = BuildingIm.m_Thumbnail + "Focused";
			BBut.hoveredFgSprite = BuildingIm.m_Thumbnail + "Hovered";
			BBut.pressedFgSprite = BuildingIm.m_Thumbnail + "Pressed";
			BBut.disabledFgSprite = BuildingIm.m_Thumbnail + "Disabled";
			BBut.objectUserData = BuildingIm;
			BBut.horizontalAlignment = UIHorizontalAlignment.Center;
			BBut.verticalAlignment = UIVerticalAlignment.Middle;
			BBut.pivot = UIPivotPoint.TopCenter;
			BBut.tooltip = BuildingIm.GetLocalizedTooltip ();
			BBut.atlas.AddSprites (BuildingIm.m_InfoTooltipAtlas.sprites);
			BBut.tooltipAnchor = UITooltipAnchor.Anchored;
			BBut.tooltipBox = UIView.GetAView ().FindUIComponent<UIPanel> ("InfoAdvancedTooltipDetail");
			BBut.isEnabled = enabled;
			BBut.eventClick += (sender, e) => BuildingBClicked (sender, e, BuildingIm);

			//Debug.Log (Convert.ToString (name));

		}

		void BuildingBClicked (UIComponent component, UIMouseEventParameter eventParam, BuildingInfo Binf)
		{

			BuildingTool buildingTool = ToolsModifierControl.SetTool<BuildingTool> ();
			{
				buildingTool.m_prefab = Binf;
				buildingTool.m_relocate = 0;
				BuildingPanel.isVisible = true;
			}
		}

		void PloppablebuttonClicked (UIComponent component, UIMouseEventParameter eventParam)
		{
			component.Focus ();
			enabled = true;
			BuildingPanel.isVisible = true;
		}

		void TabClicked (UIComponent component, UIMouseEventParameter eventParam, UIScrollablePanel panel)
		{
			for (int i = 0; i <= types; i++) {
				BuildingPanels [i].isVisible = false;
			}
			panel.isVisible = true;
			//Debug.Log("ButtonClicked" + panel.name);
		}

		protected override void OnDisable ()
		{
			if (BuildingPanel != null)
				BuildingPanel.isVisible = false;
			base.OnDisable ();
		}

		protected override void OnEnable ()
		{
			UIView.GetAView ().FindUIComponent<UITabstrip> ("MainToolstrip").selectedIndex = -1;
			base.OnEnable ();
		}

		protected override void Awake ()
		{
			//this.m_dataLock = new object();

			base.Awake ();

		}


	}
}



