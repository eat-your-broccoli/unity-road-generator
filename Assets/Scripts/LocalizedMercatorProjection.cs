using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
}
