using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURFFactureDetector
{
    interface IDataStoreAccess
    {
        String SaveFace(String username, Byte[] faceBlob);
        List<Face> CallFaces(String userName);
        bool IsUsernamValid(String username);
        String SaveAdmin(String username,String password);
        bool DeleteUser(String username);
        int GetUserId(String username);
        int GenerateUserId();
        String GetUserName(int userId);
        List<String> GetAllUserNames();
    }
}
