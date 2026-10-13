namespace LauncherCoreLib.Models.Winfs;

public enum SupportedWinfsDatabase
{
	Invalid,
	[DatabaseDeployedRelativePath("bmap.bin")]
	BASEMAP,
	[DatabaseDeployedRelativePath("nav_db2.bin")]
	JEPP_NAV,
	[DatabaseDeployedRelativePath("nav_db2_grm.bin")]
	GARMIN_NAV,
	[DatabaseDeployedRelativePath("terrain.tdb")]
	TERRAIN,
	[DatabaseDeployedRelativePath("terrain.odb2")]
	FIXED_OBSTACLE,
	[DatabaseDeployedRelativePath("terrain_heli.odb2")]
	ROTOR_OBSTACLE,
	[DatabaseDeployedRelativePath("safetaxi.bin")]
	SAFETAXI,
	[DatabaseDeployedRelativePath("fc_tpc")]
	FLITECHARTS,
	[DatabaseDeployedRelativePath("apt_dir.gca")]
	AIRPORT_DIRECTORY,
	[DatabaseDeployedRelativePath("nav_heli_db2.bin")]
	JEPP_ROTOR_NAV,
	[DatabaseDeployedRelativePath("nav_heli_db2_grm.bin")]
	GARMIN_ROTOR_NAV
}
