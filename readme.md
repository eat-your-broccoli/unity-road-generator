# Generate roads from OSM map data

by Luca Göttle (198319) <lgoettle@stud.hs-heilbronn.de>
<br>
Link to project: https://github.com/eat-your-broccoli/unity-road-generator

# Running the project

When you have setup the project explained in the later steps, you can generate roads and terrain from OSM and SRTM data.

For this, an menu entry is created called `OSM`. There, click on the item `Generate Map from OSM data`.


# Requirements for running the project

## NuGet Package Manager

Dependencies are managed using the NuGetForUnity package. This used release is available [here](https://github.com/GlitchEnzo/NuGetForUnity/releases/tag/v3.0.5).
An installation manual can be found [here](https://github.com/GlitchEnzo/NuGetForUnity/blob/master/README.md#how-do-i-use-nugetforunity).

## map data

There is one osm map available. This map is of flein. If you want to run the project for your own map, go to [OSM](https://github.com/GlitchEnzo/NuGetForUnity/blob/master/README.md#how-do-i-use-nugetforunity) and export your own map in the `.osm` format. This map can then be placed inside the `Assets/OSM/` directory. 
Then, to generate the roads from this file, you need to set the `osmPath` variable of your `MapGenerator` instance. You can do this in the `MenuEntries` file.
Additionally, you need to set the bounds of the map manually on the `MapGenerator` instance. For doing so, you can open your osm file in a text editor.
At the top of the file, there should be a `<bounds>` tag where you can find the minimum/maximum latitude/longitude.

Please note, that the file you use should not exceed 4km per side. This is the size limit of one Unity Terrain tile. 

## SRTM data

There is one SRTM data file available. It covers the region of 49N,09E to 50N,10E. If your desired osm map is not in those bounds, you need to add your own SRTM files. You can download these files [here](https://dwtkns.com/srtm30m/). Please note that NASA login credentials are required for downloading the tiles. For this, you need to accept their terms and conditions.
The downloaded `.hgt` files need to be placed in the `Assets/SRTM/` directory. No further configuration is needed. The library used will automatically take all data sources from this directory.

# Development / Research

## Plotting OSM roads

Tasks:
- understanding OSM data
- finding a library that can read OSM data
- finding a conversion from lat/lon to x/y -> Mercator Projection
- plotting OSM data

Understanding the OSM data was quite easy. A good explanaition was found [here](https://learn.opengeoedu.de/en/opendata/vorlesung/freiwillig-erhobene-daten/openstreetmap/datenmodell). 

The libarary that i found was [OsmSharp](http://www.osmsharp.com/). Here, the most time was spent on getting the library to work. For this, the NuGet package was downloaded from [here](https://www.nuget.org/packages/OsmSharp/7.0.0-pre018) and then placed in a plugins directory. Additional dependencies had to be installed manually. This was later changed to a plugin-managed solution.

The conversion of coordinates was rather easy. An implementation of the Mercator Projection was found on [openstreetmap](https://wiki.openstreetmap.org/wiki/Mercator) that was used in the `MercatorProjection` class.

Plotting OSM data was in itself a trial and error process. The [OsmSharp docs](http://docs.itinero.tech/docs/osmsharp/index.html) were helpful in the process. First, all Nodes were plotted using Cubes, then only Nodes belonging to a named way were plotted. Later, a LineRenderer was used. This LineRenderer received the positions of the Nodes belonging to a way. 

## Adding height to OSM roads
Tasks:
- finding elevation sources -> resulted in SRTM
- understanding SRTM 
- implementing an algorithm for reading SRTM elevation data -> discarded
- finding a library that handles reading SRTM data
  - finding a NuGet Package Manager
- add height information to roads

Finding elevation sources was rather straight forward. A standard is SRTM. It took longer to understand SRTM data and implementing a way to read the data. The SRTM file format has no headers. It is an array of 2 two-byte Little Endian Encodings of height information. One themself has to calculate which two bytes need to be read and converted from the file. A good starting point for me was [this](https://gis.stackexchange.com/questions/43743/extracting-elevation-from-hgt-file) stackoverflow question.
Later, this was changed to a library because my implementation yielded no satisfactory results (and was quite slow).

A library that I found was [SRTM from Itinero](https://github.com/itinero/srtm). I used the same approach to add the library to Unity as the OsmSharp library. This time, I couldn't get it to work after many hours. 
A solution was to use the `NuGetForUnity`-plugin. This allows easy management of dependencies.

Adding the height to the roads was itself quite easy. The `SRTM` library returns the height to the position provided. This is then added to the Vector3 objects representing the positions.

## Generate terrain from SRTM data
Tasks:
- understanding terrain
- creating terrain through a script

This was the part I struggled the most with. The Unity Terrain is itself okay-ish documented. The documentation, however, mostly refers to the Unity Editor. Documentation for using the terrain in scripts is scarce. A life-saver was the YouTube channel `Brackeys`. Their video on [Procedural Generation \[of Unity Terrain\]](https://www.youtube.com/watch?v=vFvwyu_ZKfU) helped me understand how terrain height works and how to access the height information of the terrain.

The second part, using the height of the SRTM data in the terrain was even more problematic. The basic idea was to iterate over every point in the terrain starting from a known position. For e.g., I knew the position of the index 0,0 translated to certain latitude/longitude. Then, I iterated over every point in the matrix, converted the x,y coordinates into latitude and longitude, and then used the SRTM library to return the elevation.
It took quite some time to understand that the value needs to be between [0, 1]. The value is the percentage of the max height provided to the terrain. An honourable mention to the [Terrain Documentation](https://docs.unity3d.com/ScriptReference/TerrainData.SetHeights.html) for actually mentioning this.
Another issue was that the indices work as y,x. This caused me to have buggy results in the beginning.


## terrain and road fixes
Tasks: 
- finding a way to smooth terrain
- making roads and terrain fit
- terrain cut-off

One problem was that SRTM is mapped in 30x30m tiles. This means, the height information is rather inprecise. A result was, that the terrain looked minecraft-like. A good few days was spent on finding a way to blur the Terrain. 

This can be easily done in the Editor view. Howevery, documentation for doing this in a script is scarce to none. This [unity forum entry](https://answers.unity.com/questions/648824/terrain-smoothing.html) did not help in the end. 
<br>
This [reddit post](https://www.reddit.com/r/Unity3D/comments/ge80tj/smooth_terrain_heightmap_with_code/) also did not yield no results. 
<br>
Most other solutions referred to a gaussian blur on the heightmap itself. This means, using a blur on a grayscale image of the heightmap, and then passing this into the Terrain. However, this was not applicable.

After a lot of trial end error, I implemented a crude blur function with varying kernel sizes. These shift over the heightmap matrix and average the height over the neighbors of the cell. This resulted in smoother terrain, but also oversmoothing. Places where there were steep inclines in few meters were spread to lesser inclines.


Now, the terrain and the roads did not fit together at all. The first solution was to use a `LocalizedMercatorProjection`, which can be passed an offset to reduce the produced values (and thus improving floating point accuracy). On the other hand, Terrain and Roads now had the same starting point of reference.
<br>
Additionally, I had to change the order I created the roads and the terrain. In the old approach, first the roads were created, and then the terrain.
Now, first the terrain is created and smoothed. Then, this smoothed terrain is passed to the Road generation and used as the height information provider. For this, the class `TerrainHeightInfo` was created. Otherwise, the roads would use the original srtm data, whilst the terrain uses a blurred version.


## change road generation
Tasks:
- painting roads on the terrain -> yielded no result
- generate roads in a library -> yielded no result

Some research was spent into finding a way to manually paint a terrain. This was discarded, as it was to time intensive. For example, I would have to infer from the Nodes of a road, which points of the terrain have to be painted.
Additionally, I did not get it to work that I can paint the terrain with a script.

Some further research was spent on finding a library that allows creation of roads via script. However, most libraries such as [RoadArchitect](https://github.com/MicroGSD/RoadArchitect) focus on creating roads in an Editor.

# References

## Code

`MercatorProjection`: This code was taken from https://wiki.openstreetmap.org/wiki/Mercator
<br>`Terrain_Generator`: The foundation and inspiration was taken from the Brackeys tutorial found here: https://www.youtube.com/watch?v=vFvwyu_ZKfU
<br>`OSM_Map_Generator`: The function `PlotMap(string filePath)` was directly taken from the documentation from OsmSharp: https://github.com/OsmSharp/core/tree/develop/samples


## Assets

`OSM/`: These files were obtained from https://www.openstreetmap.org/
<br>`SRTM/`: These files were obtained from https://dwtkns.com/srtm30m/

## Libraries
`OsmSharp`: This library was used for parsing .osm-files: http://www.osmsharp.com/
<br>`SRTM`: This library was used for reading SRTM data: https://github.com/itinero/srtm

## Plugins
`NuGetForUnity`: This library handles the Libraries mentioned above: https://github.com/GlitchEnzo/NuGetForUnity/releases/tag/v3.0.5