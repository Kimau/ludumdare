using UnityEngine;
using System.Collections.Generic;

public class BuildLogic : MonoBehaviour
{
	public int m_rowHeight = 8;
	public int m_rowWidth = 10;
	public BuildTile[,] m_tiles;
	public BuildTile[] m_prefabList;
	public BoxCollider2D m_cameraBox;
	public ParticleSystem m_explode;
	public HostageLogic m_hostageStamp;

	public float m_minSafeTime;
	public float m_maxSafeTime;
	public float m_timeTillBlow;
	[Range(0f,1f)]
	public float
		m_chanceBlowIsRandom;
	[Range(0f,1f)]
	public float
		m_chanceCatchingFire;

	Dictionary<TileTypes,BuildTile>	m_prefabs;
	bool m_doBoom;

	// Use this for initialization
	void Start()
	{
		// Setup Dictionary
		m_prefabs = new Dictionary<TileTypes, BuildTile>();
		for( int i = 0; i < m_prefabList.Length; i++ )
			m_prefabs[ m_prefabList[ i ].m_type ] = m_prefabList[ i ];

		GenerateBuilding();

		m_doBoom = false;
		for( int y = 0; y < m_rowHeight; ++y )
			for( int x = 0; x < m_rowWidth; ++x )
				if(Random.value < 0.2f)
					BlowTile(x,y);
		m_doBoom = true;

		// Time till next explosion
		m_timeTillBlow = m_minSafeTime + Random.value * (m_maxSafeTime-m_minSafeTime);
	}

	void GenerateBuilding()
	{
		// Clear Out
		SpriteRenderer[] spList = GetComponentsInChildren<SpriteRenderer>();
		for( int s = 0; s < spList.Length; s++ )
			Destroy( spList[ s ].gameObject );
		// Setup New
		m_tiles = new BuildTile[ m_rowWidth, m_rowHeight ];
		int x = 0;
		int y = 0;

		// Floor
		CreateTile( 0, 0, TileTypes.FloorLeft );
		CreateTile( m_rowWidth - 1, 0, TileTypes.FloorRight );
		for( x = 1; x < (m_rowWidth - 1); ++x )
			CreateTile( x, 0, TileTypes.Floor );

		// Body of Building
		int brickChange = 25;
		int midChance = 10;
		int barChance = 10;
		int windowChance = 0;
		int shutterChance = 0;
		for( y = 1; y < (m_rowHeight - 1); ++y )
		{
			CreateTile( 0, y, TileTypes.BrickLeft );
			CreateTile( m_rowWidth - 1, y, TileTypes.BrickRight );
			for( x = 1; x < (m_rowWidth - 1); ++x )
			{
				if( (x % 2) > 0 )
				{
					windowChance += 20;
					shutterChance += 20;
				}
				barChance += 10;
				midChance += 10;
				// Random Pick
				int probChance = Mathf.Max( 0, brickChange ) + Mathf.Max( 0, midChance ) + Mathf.Max( 0, barChance ) + Mathf.Max( 0, windowChance ) + Mathf.Max( 0, shutterChance );
				int tChoice = Random.Range( 0, probChance );
				// Mid
				if( tChoice < midChance )
				{
					CreateTile( x, y, TileTypes.BrickMid );
					midChance = -10;
					barChance -= 10;
					continue;
				}
				else
				{
					tChoice -= midChance;
				}
				// Crossbar
				if( tChoice < barChance )
				{
					CreateTile( x, y, TileTypes.BrickCrossbar );
					barChance = -10;
					midChance -= 10;
					continue;
				}
				else
				{
					tChoice -= barChance;
				}
				// Window
				if( tChoice < windowChance )
				{
					CreateTile( x, y, TileTypes.Window );
					windowChance = -20;
					shutterChance -= 20;
					continue;
				}
				else
				{
					tChoice -= windowChance;
				}
				// Shutter
				if( tChoice < shutterChance )
				{
					CreateTile( x, y, TileTypes.Shutter );
					shutterChance = -15;
					windowChance -= 10;
					continue;
				}
				else
				{
					tChoice -= shutterChance;
				}
				// Hole
				CreateTile( x, y, TileTypes.Brick );
			}
		}

		// Roof
		CreateTile( 0, m_rowHeight - 1, TileTypes.RoofLeft );
		CreateTile( m_rowWidth - 1, m_rowHeight - 1, TileTypes.RoofRight );
		for( x = 1; x < (m_rowWidth - 1); ++x )
		{
			CreateTile( x, m_rowHeight - 1, TileTypes.Roof );
			// Place Hostages on Roof
			GameObject ho = Instantiate(m_hostageStamp.gameObject) as GameObject;
			ho.transform.parent = m_tiles[x,m_rowHeight - 1].m_tile.transform;
			ho.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
		}


		// Setup Camera restriction box
		m_cameraBox.size = new Vector2( 4.0f * m_rowWidth, 4.0f * m_rowHeight );
		m_cameraBox.center = new Vector2( -2.0f, m_cameraBox.size.y * 0.5f - 2.0f );
	}

	public void AddTileRef( GameObject go, int x, int y)
	{
		TileRef tr = go.AddComponent<TileRef>();
		tr.x = x; 
		tr.y = y;
	}

	public void CreateTile( int x, int y, TileTypes tt)
	{
		CreateTile( x, y, tt, false);
	}

	public void CreateTile( int x, int y, TileTypes tt, bool blown )
	{
		if(m_tiles[ x, y ] == null)
			m_tiles[ x, y ] = new BuildTile();

		if(m_prefabs.ContainsKey(tt) == false)
		{
			Debug.LogError("Cannot Find Prefab for " + tt);
			return;
		}

		m_tiles[ x, y ].m_type = tt;
		m_tiles[ x, y ].m_isBurning = false;

		if(m_prefabs[ tt ].m_blownTile)
		{
			m_tiles[ x, y ].m_isBlown = blown;

			m_tiles[ x, y ].m_blownTile = Instantiate( m_prefabs[ tt ].m_blownTile) as GameObject;
			m_tiles[ x, y ].m_blownTile.name = "Tile Row" + y + " Col" + x + " " + tt + " X";
			m_tiles[ x, y ].m_blownTile.transform.parent = transform;
			m_tiles[ x, y ].m_blownTile.transform.localPosition = new Vector3( 4.0f * (x - m_rowWidth*0.5f), 4.0f * y, 0.0f );
			AddTileRef(m_tiles[ x, y ].m_blownTile, x, y);
			m_tiles[ x, y ].m_blownTile.SetActive(blown);


			BurnContain[] bcList = m_tiles[ x, y ].m_blownTile.GetComponentsInChildren<BurnContain>(true);
			foreach(BurnContain bc in bcList)
			{
				AddTileRef(bc.gameObject, x, y);
				bc.gameObject.SetActive(false);
			}
		}
		else
		{
			blown = false;
		}

		m_tiles[ x, y ].m_tile = Instantiate( m_prefabs[ tt ].m_tile) as GameObject;
		m_tiles[ x, y ].m_tile.name = "Tile Row" + y + " Col" + x + " " + tt;
		m_tiles[ x, y ].m_tile.transform.parent = transform;
		m_tiles[ x, y ].m_tile.transform.localPosition = new Vector3( 4.0f * (x - m_rowWidth*0.5f), 4.0f * y, 0.0f );
		AddTileRef(m_tiles[ x, y ].m_tile, x, y);
		m_tiles[ x, y ].m_tile.SetActive(!blown);


	}

	/// <summary>
	/// Blows Up the tile.
	/// </summary>
	public bool BlowTile(int x, int y)
	{
		if(m_tiles[ x, y ] == null)
			CreateTile(x,y,TileTypes.Brick, true);

		if(m_tiles[ x, y ].m_isBlown || (m_tiles[ x, y ].m_blownTile == null))
			return false;	// Cannot Blow Tile

		m_tiles[ x, y ].m_isBlown = true;
		m_tiles[ x, y ].m_isBurning = false;
		m_tiles[ x, y ].m_blownTile.SetActive(true);
		m_tiles[ x, y ].m_tile.SetActive(false);

		// Is Burning
		BurnContain[] bcList = m_tiles[ x, y ].m_blownTile.GetComponentsInChildren<BurnContain>(true);
		if(bcList.Length > 0)
		{
			bool isBurn = Random.value < m_chanceCatchingFire;

			m_tiles[ x, y ].m_isBurning = isBurn;
			foreach(BurnContain bc in bcList)
				bc.gameObject.SetActive(isBurn);
		}

		if(m_doBoom)
		{
			GameObject boom = Instantiate(m_explode.gameObject) as GameObject;
			boom.transform.position = m_tiles[x,y].m_blownTile.transform.position;
			Debug.Log("Blew up " + x.ToString() + ":" + y.ToString());
		}

		return true;
  }

	/// <summary>
	/// Burns the out tile.
	/// </summary>
	public void BurnOutTile(int x, int y)
	{
		m_tiles[ x, y ].m_isBurning = false;

		BurnContain[] bcList = m_tiles[ x, y ].m_blownTile.GetComponentsInChildren<BurnContain>(true);
		if(bcList.Length > 0)
		{
			m_tiles[ x, y ].m_isBurning = false;
			foreach(BurnContain bc in bcList)
				bc.gameObject.SetActive(false);
		}

		Debug.Log("Burned out " + x + ":" + y);
	}

	/// <summary>
	/// Gets the safe nieghbour.
	/// </summary>
	public TileRef GetSafeNieghbour(TileRef tr)
	{
		List<TileRef> stepToList = new List<TileRef>();
		if((tr.x-1 >= 0) && (m_tiles[tr.x-1, tr.y].m_isBlown == false))
			stepToList.Add(m_tiles[tr.x-1, tr.y].m_tile.GetComponent<TileRef>());
		if((tr.x+1 < m_rowWidth) && (m_tiles[tr.x+1, tr.y].m_isBlown == false))
			stepToList.Add(m_tiles[tr.x+1, tr.y].m_tile.GetComponent<TileRef>());
		if((tr.y-1 >= 0) && (m_tiles[tr.x, tr.y-1].m_isBlown == false))
			stepToList.Add(m_tiles[tr.x, tr.y-1].m_tile.GetComponent<TileRef>());
		if((tr.y+1 < m_rowHeight) && (m_tiles[tr.x, tr.y+1].m_isBlown == false))
			stepToList.Add(m_tiles[tr.x, tr.y+1].m_tile.GetComponent<TileRef>());
		
		if(stepToList.Count <= 0)
			return null;
		
		return stepToList[Random.Range(0,stepToList.Count)];
	}

	/// <summary>
	/// Spreads the fire.
	/// </summary>
	bool SpreadFire(BurnContain bc)
	{
		TileRef trSource = bc.GetComponent<TileRef>();
		TileRef	tr = GetSafeNieghbour(trSource);
		if(tr == null)
		{
			BurnOutTile(trSource.x, trSource.y);
			return false;
		}
		else
		{
			BlowTile(tr.x, tr.y);
			return true;
		}
	}

	public int[] GetShuffleIDList(int len)
	{
		List<float> sl = new List<float>(len);
		for( int i = 0; i < len; i++ )
			sl.Add(i + Random.value * 0.9f);

		sl.Sort((a,b) => Mathf.FloorToInt((a - Mathf.Floor(a) - b + Mathf.Floor(b))*100f));

		int[] rl = new int[len];
		for( int i = 0; i < len; i++ )
			rl[i] = Mathf.FloorToInt(sl[i]);

		return rl;
	}
    
  // Update is called once per frame
	void Update()
	{
		m_timeTillBlow -= Time.deltaTime;
		if(m_timeTillBlow < 0.0f)
		{
			bool foundTile = false;

			BurnContain[] bcList = GetComponentsInChildren<BurnContain>(false);
			if((bcList.Length > 0) && (Random.value > m_chanceBlowIsRandom))
			{
				int[] rl = GetShuffleIDList(bcList.Length);
				int i=0;
				while(!foundTile && (i < bcList.Length))
					foundTile = SpreadFire(bcList[rl[i++]]);
			}

			// Blow Up Random Tile on the Map
			int[] xl = GetShuffleIDList(m_rowWidth);
			int[] yl = GetShuffleIDList(m_rowHeight);
			for (int xi = 0; (!foundTile) && (xi < m_rowWidth); xi++)
				for (int yi = 0; (!foundTile) && (yi < m_rowHeight); yi++)
					foundTile = BlowTile(xl[xi],yl[yi]);

			// Time till next explosion
			m_timeTillBlow = m_minSafeTime + Random.value * (m_maxSafeTime-m_minSafeTime);
		}
	}
}
