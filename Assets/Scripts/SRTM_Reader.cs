using SRTM;
using SRTM.Sources.USGS;

public class SRTM_Reader
{
    public SRTMData heightMap;

    public SRTM_Reader(string filename)
    {
        heightMap = new SRTMData(filename, new USGSSource());
    }

    public int GetElevationAtSync(double lat, double lon)
    {
        return (int) heightMap.GetElevation(lat, lon);
    }
}
