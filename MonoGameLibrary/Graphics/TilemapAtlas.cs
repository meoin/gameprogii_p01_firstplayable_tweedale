using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TilemapAtlas
{
    // Store dictionarys for texture regions and tilemaps
    private Dictionary<string, Tilemap> _tilemaps;

    /// <summary>
    /// Gets the total number of tilemaps in the atlas.
    /// </summary>
    public int Count { get; }

    private Vector2 _scale;

    /// <summary>
    /// Gets or Sets the scale factor to draw each tilemap at.
    /// </summary>
    public Vector2 Scale 
    {
        get { return _scale; }
        set
        {
            _scale = value;

            foreach(Tilemap tilemap in _tilemaps.Values)
            {
                tilemap.Scale = value;
            }
        }
    }

    /// <summary>
    /// Creates a new tilemap atlas.
    /// </summary>
    public TilemapAtlas()
    {
        _tilemaps = new Dictionary<string, Tilemap>();
    }

    /// <summary>
    /// Adds the given tilemap to this texture atlas with the specified name.
    /// </summary>
    /// <param name="tilemapName">The name of the tilemap to add.</param>
    /// <param name="tilemap">The tilemap to add.</param>
    public void AddTilemap(string tilemapName, Tilemap tilemap)
    {
        _tilemaps.Add(tilemapName, tilemap);
    }

    /// <summary>
    /// Gets the tilemap from this texture atlas with the specified name.
    /// </summary>
    /// <param name="tilemapName">The name of the tilemap to retrieve.</param>
    /// <returns>The tilemap with the specified name.</returns>
    public Tilemap GetTilemap(string tilemapName)
    {
        return _tilemaps[tilemapName];
    }

    /// <summary>
    /// Removes the tilemap with the specified name from this texture atlas.
    /// </summary>
    /// <param name="tilemapName">The name of the tilemap to remove.</param>
    /// <returns>true if the tilemap is removed successfully; otherwise, false.</returns>
    public bool RemoveTilemap(string tilemapName)
    {
        return _tilemaps.Remove(tilemapName);
    }

    /// <summary>
    /// Removes all regions from this texture atlas.
    /// </summary>
    public void Clear()
    {
        _tilemaps.Clear();
    }

    /// <summary>
    /// Creates a new texture atlas based on a texture atlas xml configuration file.
    /// </summary>
    /// <param name="content">The content manager used to load the texture for the atlas.</param>
    /// <param name="fileName">The path to the xml file, relative to the content root directory.</param>
    /// <returns>The texture atlas created by this method.</returns>
    public static TilemapAtlas FromFile(ContentManager content, string fileName)
    {
        TilemapAtlas atlas = new TilemapAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                var tilemaps = root.Element("Tilemaps")?.Elements("Tilemap");

                if (tilemaps != null)
                {
                    Debug.Write("Added tilemaps: ");

                    foreach (var tilemap in tilemaps)
                    {
                        XElement tilesetElement = tilemap.Element("Tileset");

                        string name = tilemap.Attribute("name")?.Value;
                        string regionAttribute = tilesetElement.Attribute("region").Value;
                        string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        int x = int.Parse(split[0]);
                        int y = int.Parse(split[1]);
                        int width = int.Parse(split[2]);
                        int height = int.Parse(split[3]);

                        int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                        int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                        string contentPath = tilesetElement.Value;

                        // Load the texture 2d at the content path
                        Texture2D texture = content.Load<Texture2D>(contentPath);

                        // Create the texture region from the texture
                        TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                        // Create the tileset using the texture region
                        Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                        // The <Tiles> element contains lines of strings where each line
                        // represents a row in the tilemap.  Each line is a space
                        // separated string where each element represents a column in that
                        // row.  The value of the column is the id of the tile in the
                        // tileset to draw for that location.
                        //
                        // Example:
                        // <Tiles>
                        //      00 01 01 02
                        //      03 04 04 05
                        //      03 04 04 05
                        //      06 07 07 08
                        // </Tiles>
                        XElement tilesElement = tilemap.Element("Tiles");

                        // Split the value of the tiles data into rows by splitting on
                        // the new line character
                        string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                        // Split the value of the first row to determine the total number of columns
                        int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                        // Create the tilemap
                        Tilemap map = new Tilemap(tileset, columnCount, rows.Length);

                        // Process each row
                        for (int row = 0; row < rows.Length; row++)
                        {
                            // Split the row into individual columns
                            string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            // Process each column of the current row
                            for (int column = 0; column < columnCount; column++)
                            {
                                // Get the tileset index for this location
                                int tilesetIndex = int.Parse(columns[column]);

                                // Add that region to the tilemap at the row and column location
                                map.SetTile(column, row, tilesetIndex);
                            }
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddTilemap(name, map);
                            Debug.Write($"{name}, ");
                        }
                    }
                    Debug.WriteLine("");
                }
                else
                {
                    Debug.WriteLine("Empty tilemap atlas");
                }

                return atlas;
            }
        }
    }

}