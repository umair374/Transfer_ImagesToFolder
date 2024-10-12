using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using Oracle.ManagedDataAccess.Types;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

class Program
{
    static void Main()
    {
        string connectionString = "User Id=SOLEHRE;Password=SOLEHRESOLAPPS;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.236)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)))";

        using (OracleConnection connection = new OracleConnection(connectionString))
        {
            connection.Open();

            string sql = "SELECT emppic, empid FROM HR_EMPLOYEEPIC";

            using (OracleCommand command = new OracleCommand(sql, connection))
            { command.InitialLONGFetchSize = -1;
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OracleBinary oracleBinaryData = reader.GetOracleBinary(reader.GetOrdinal("emppic"));
                        byte[] imageData = oracleBinaryData.IsNull ? null : oracleBinaryData.Value;

                        string imageId = reader["empid"].ToString();
                        if (imageData == null || imageData.Length == 0)
                        {
                            Console.WriteLine($"No image data for image_id: {imageId}");
                            continue;
                        }

                        SaveImageToLocalFolder(imageData, imageId);
                    }
                }
            }
        }
    }

    static void SaveImageToLocalFolder(byte[] imageData, string imageId)
    {
        string folderPath = @"\\192.168.1.236\Emp_Pics\"; 
        string filePath = Path.Combine(folderPath, $"{imageId}.jpg"); 

        Directory.CreateDirectory(folderPath);

        File.WriteAllBytes(filePath, imageData);
        Console.WriteLine($"Image saved to: {filePath}  IMAGE DATA : {imageData}");
    }
}
