using UnityEngine;

public enum TileTypes
{
	Brick,
	BrickLeft,
	BrickRight,
	Floor,
	FloorLeft,
	FloorRight,
	Roof,
	RoofLeft,
	RoofRight,
	Shutter,
	Window,
	BrickMid,
	BrickCrossbar
}
;

[System.Serializable]
public class BuildTile
{
	public bool m_isBlown;
	public bool m_isBurning;
	public TileTypes m_type;
	public GameObject m_tile;
	public GameObject m_blownTile;
};
