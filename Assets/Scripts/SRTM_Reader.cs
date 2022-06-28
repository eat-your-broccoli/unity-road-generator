using SRTM;
using SRTM.Sources.USGS;
using UnityEngine;


/**
 * @class SRTM_Reader
 * 
 * wrapper for SRTM class
 * provides height data
 */ 
public class SRTM_Reader
{
    public SRTMData heightMap;

    public SRTM_Reader(string filename)
    {
        heightMap = new SRTMData(filename, new USGSSource());
    }

    public int GetElevationAtSync(double lat, double lon)
    {
        double? x = heightMap.GetElevationBilinear(lat, lon);
        return x.HasValue ? (int) x : 0;
    }
}
