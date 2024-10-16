using Npgsql;
using System.Text;
using Esri;
using Esri.ArcGISRuntime.Geometry;
class Program
{
    static void Main(string[] args)
    {
        string connect = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=GIS";
        NpgsqlConnection conn = new NpgsqlConnection(connect);
        List<string> countyid = new List<string>();
        int listcount = 0;

        conn.Open();

        string sql = "SELECT countyname,countyid FROM \"GIS_table_copy\" GROUP BY countyid,countyname Order BY countyid ASC";

        NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string SQLcountyid = reader["countyid"].ToString();
            countyid.Add(new string($"\'{SQLcountyid}\'"));
            listcount++;
        }
        conn.Close();


        int i = 0;

        while (i < listcount)
        {
            conn.Open();
            sql = $"INSERT INTO \"NewGIS\"(countyid, countyname, geom) SELECT countyid, countyname, ST_Union ( geom ) FROM \"GIS_table_copy\" where countyid = {countyid[i]} GROUP BY countyid, countyname";

            cmd = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader reader1 = cmd.ExecuteReader();
            i++;

            conn.Close();
        }

    }
        
}
