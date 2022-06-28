using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * @class LocalizedMercatorProjection
 * 
 * mercator projection for converting between lat,lon and x,y
 * can set an offset for limiting the values returned, improving float precision
 */

public class LocalizedMercatorProjection
{
    public float x_offset = 0f;
    public float y_offset = 0f;

    public double lonToX(double lon)
    {
        return MercatorProjection.lonToX(lon) - x_offset;
    }

    public double xToLon(double x)
    {
        return MercatorProjection.xToLon(x + x_offset);
    }

    public double latToY(double lat)
    {
        return MercatorProjection.latToY(lat) - y_offset;
    }

    public double yToLat(double y)
    {
        return MercatorProjection.yToLat(y + y_offset);
    }

    public void CalcAndSetOffset(float min_lat, float min_lon)
    {
        this.x_offset = (float)MercatorProjection.lonToX(min_lon);
        this.y_offset = (float)MercatorProjection.latToY(min_lat);
    }
}
