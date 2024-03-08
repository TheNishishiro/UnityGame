﻿using System;
using System.Diagnostics;
using UnityEngine;

namespace DefaultNamespace.Data
{
	public class ConfigurationFile
	{
		public int ConfigurationVersion { get; set; }
		public bool Vsync { get; set; }
		public int WindowMode { get; set; }
		public int GrassRenderDistance { get; set; }
		public int GrassDensity { get; set; }
		public int ShadowQuality { get; set; }
		public int Quality { get; set; }
		public int LodLevel { get; set; }
		public int AntiAliasing { get; set; }
		public int RenderScaling { get; set; }
		public int PresetIndex { get; set; }
		public bool IsDiscordEnabled { get; set; }
		public int ResolutionWidth { get; set; }
		public int ResolutionHeight { get; set; }
		public uint RefreshRate { get; set; }
		public int RenderDistance { get; set; }
		public bool Use3dGrass { get; set; }
		public int ObjectDensity { get; set; }
		public int TextureQuality { get; set; }
		public float Volume { get; set; }
		public string Username { get; set; }
		public bool RenderCoopProjectiles { get; set; }

		public ConfigurationFile Default()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Vsync = true;
				Quality = 1;
				GrassDensity = 1;
				GrassRenderDistance = 1;
				RenderScaling = 2;
				ShadowQuality = 0;
				LodLevel = 2;
				AntiAliasing = 0;
				IsDiscordEnabled = false;
				PresetIndex = 1;
				RenderDistance = 1;
				Use3dGrass = false;
				ObjectDensity = 1;
				Volume = 1;
				TextureQuality = 2;
				ConfigurationVersion = 8;
				RenderCoopProjectiles = true;
			}
			else
			{
				Vsync = true;
				Quality = 4;
				GrassDensity = 3;
				GrassRenderDistance = 3;
				RenderScaling = 4;
				ShadowQuality = 5;
				LodLevel = 4;
				AntiAliasing = 4;
				IsDiscordEnabled = false;
				PresetIndex = 4;
				ConfigurationVersion = 0;
				RenderDistance = 1;
				Use3dGrass = false;
				ObjectDensity = 1;
				Volume = 1;
				TextureQuality = 4;
				RenderCoopProjectiles = false;
			}

			return Update();
		}

		public ConfigurationFile Update()
		{
			if (ConfigurationVersion == 0)
			{
				WindowMode = 0;
				ResolutionWidth = Screen.currentResolution.width;
				ResolutionHeight = Screen.currentResolution.height;
				RefreshRate = Screen.currentResolution.refreshRateRatio.numerator;
				ConfigurationVersion = 1;
			}
			if (ConfigurationVersion == 1)
			{
				switch(RenderScaling)
				{
					case >= 2 and <= 4:
						RenderScaling += 2;
						break; 
				}
				ConfigurationVersion = 2;
			}
			if (ConfigurationVersion == 2)
			{
				if (LodLevel == 3)
					LodLevel = 4;
				ConfigurationVersion = 3;
			}
			if (ConfigurationVersion == 3)
			{
				RenderDistance = 1;
				ConfigurationVersion = 4;
			}
			if (ConfigurationVersion == 4)
			{
				ObjectDensity = 1;
				ConfigurationVersion = 5;
			}
			if (ConfigurationVersion == 5)
			{
				Volume = 1;
				ConfigurationVersion = 6;
			}
			if (ConfigurationVersion == 6)
			{
				TextureQuality = Quality switch
				{
					0 => 2,
					1 => 3,
					_ => 4
				};

				ConfigurationVersion = 7;
			}
			if (ConfigurationVersion == 7)
			{
				RenderCoopProjectiles = true;
				ConfigurationVersion = 8;
			}

			return this;
		}
	}
}