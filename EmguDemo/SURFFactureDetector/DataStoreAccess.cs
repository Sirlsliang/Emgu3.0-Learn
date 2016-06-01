using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Configuration;

namespace SURFFactureDetector
{
    class DataStoreAccess : IDataStoreAccess
    {
        private SQLiteConnection sqliteConnection;

        public DataStoreAccess( )
        {
            string dataBasePath = ConfigurationManager.AppSettings["dbPath"];
            sqliteConnection = new SQLiteConnection(dataBasePath);
            
        }
        
        
        public List<Face> CallFaces(string userName)
        {
            var faces = new List<Face>();
            try
            {
                sqliteConnection.Open();
                var query = userName.ToLower().Equals("ALL_USERS".ToLower()) ? "SELECT * FROM faces" : "SELECT * FROM faces where username=@username";
                var cmd = new SQLiteCommand(query, sqliteConnection);
                if (!userName.ToLower().Equals("ALL_USERS".ToLower())) cmd.Parameters.AddWithValue("username", userName);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return null;
                while (result.Read())
                {
                    var face = new Face
                    {
                        Image = (byte[])result["faceSample"],
                        Id = Convert.ToInt32(result["id"]),
                        Label = (String)result["username"],
                        UserId = Convert.ToInt32(result["userId"])
                    };
                    faces.Add(face);
                }
                faces = faces.OrderBy(f => f.Id).ToList<Face>();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally {
                sqliteConnection.Close();
            }
            return faces;
             
        }

        public bool DeleteUser(string username)
        {
            var toReturn = false;
            try
            {
                sqliteConnection.Open();
                var query = "DELETE FROM faces WHERE username=@username";
                var cmd = new SQLiteCommand(query, sqliteConnection);
                cmd.Parameters.AddWithValue("username", username);
                var result = cmd.ExecuteNonQuery();
                if (result > 0) toReturn = true;
            }
            catch (Exception ex) {
                return toReturn;
            }
            finally {
                sqliteConnection.Close();
            }
            return toReturn;
            
             
        }

        public int GenerateUserId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
            
        }

        public List<string> GetAllUserNames()
        {
            var username = new List<string>();
            try
            {
                sqliteConnection.Open();
                var query = "SELECT DISTINCT username from faces";
                var cmd = new SQLiteCommand(query, sqliteConnection);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    username.Add((String)result["username"]);
                }
                username.Sort();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally {
                sqliteConnection.Close();
            }
            return username;
        }

        public int GetUserId(string username)
        {
            var userId = 0;
            try
            {
                sqliteConnection.Open();

                var selectQuery = "select userId from faces where username=@username limit 1";
                var cmd = new SQLiteCommand(selectQuery,sqliteConnection);
                cmd.Parameters.AddWithValue("username", username);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return 0;
                while (result.Read()) {
                    userId = Convert.ToInt32(result["userId"]);
                }
            }
            catch {
                return userId;
            }
            finally {
                sqliteConnection.Close();
            }
            return userId;
        }

        public string GetUserName(int userId)
        {
            var username = "";
            try {
                sqliteConnection.Open();
                var query = "SELECT username from faces where userId=@userId LIMIT 1";
                var cmd = new SQLiteCommand(query, sqliteConnection);
                cmd.Parameters.AddWithValue("userId", userId);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return username;
                while (result.Read()) {
                    username = (String)result["username"];
                }
            }
            catch {
                return username;
            }
            finally {
                sqliteConnection.Close();
            }
            return username;
        }

        public bool IsUsernamValid(string username)
        {
            throw new NotImplementedException();
        }

        public string SaveAdmin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public string SaveFace(string username, byte[] faceBlob)
        {
            try
            {
                var existingUserId = GetUserId(username);
                if (existingUserId == 0) existingUserId = GenerateUserId();
                sqliteConnection.Open();
                var insertQuery = "insert into faces(username,faceSample,userId) values(@username,@faceSample,@userId)";
                var cmd = new SQLiteCommand(insertQuery,sqliteConnection);
                cmd.Parameters.AddWithValue("username",username);
                cmd.Parameters.AddWithValue("userId",existingUserId);
                cmd.Parameters.Add("faceSample",System.Data.DbType.Binary,faceBlob.Length).Value = faceBlob;
                var result = cmd.ExecuteNonQuery();
                return String.Format("{0} face(s) saved successfuly",result);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally {
                sqliteConnection.Close();
            }

        }
    }
}
