
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Task_1.Data
{

    public class DataAcces
    {

        private SqlConnection myConnection = new SqlConnection();
        private SqlDataAdapter oSqlAdap = new SqlDataAdapter();
        private DataSet Ds = new DataSet();
        SqlCommand oCommand = new SqlCommand();

        public SqlCommand OCommand { get => oCommand; set => oCommand = value; }

        public DataSet ExecuteDataSet(string str, string connectionstr)
        {
            DataSet functionReturnValue = null;
            myConnection = new SqlConnection(connectionstr);
            Ds = new DataSet();
            try
            {
                myConnection.Open();
                oSqlAdap = new SqlDataAdapter(str, myConnection);
                oSqlAdap.Fill(Ds, "T_Temp");
                functionReturnValue = Ds;
            }
            catch (Exception ex)
            {
                myConnection.Close();
                throw new ApplicationException("Exception in Run Query  " + ex.Message.ToString());
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
                oSqlAdap = null;
            }
            return functionReturnValue;
        }
        public static Object IfNull(object obj)
        {
            if (obj == null)
                return "0";
            else if (obj == null)
                return "0";
            else if (obj.ToString() == "")
                return "0";
            else
                return obj;
        }
        public static Object IfStrNull(object obj)
        {
            if (obj == null)
                return "";
            else if (obj == null)
                return "";
            else if (obj.ToString() == "")
                return "";
            else
                return obj;
        }

        public DataTable ExecuteDataTable(string strQuery, string strconnection)
        {

            myConnection = new SqlConnection(strconnection);
            DataTable _retVal = null;
            DataSet oDataSet = new DataSet();
            try
            {
                myConnection.Open();

                if (myConnection.State == ConnectionState.Open)
                {
                    oCommand.Connection = myConnection;
                    oCommand.CommandText = strQuery;
                    oCommand.CommandType = CommandType.Text;
                    oSqlAdap = new SqlDataAdapter(oCommand);
                    if (oSqlAdap != null)
                        oSqlAdap.Fill(oDataSet);
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
                SqlCommand oCommand = null;
                oSqlAdap = null;
            }
            return _retVal = oDataSet.Tables[0];

        }
        public int ExecuteNonQuery(string str, string strconn, string strIdentity = "")
        {
            int _retVal = 0;

            SqlConnection myConnection = new SqlConnection(strconn);
            try
            {
                myConnection.Open();
                oCommand = new SqlCommand(str, myConnection);
                _retVal = oCommand.ExecuteNonQuery();
                if (_retVal > 0)
                {
                    if (strIdentity.Length > 0)
                    {
                        oCommand = new SqlCommand(strIdentity, myConnection);
                        _retVal = Convert.ToInt32(oCommand.ExecuteScalar());
                    }
                }

            }
            catch (Exception ex)
            {
                myConnection.Close();
                throw new ApplicationException("Exception in Run Query  " + ex.Message.ToString());
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
                oCommand = null;
            }
            return _retVal;
        }
        public object ConvertDataTabletoString(DataTable dt)
        {
            try
            {


                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                string abd = JsonConvert.SerializeObject(rows);
                return abd;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }


        }
        public async Task<List<T>> ConvertToList<T>(DataTable dataTable)
        {
            List<T> items = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T item = Activator.CreateInstance<T>();

                // Assuming properties of T match column names in DataTable
                foreach (DataColumn column in dataTable.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.ColumnName);
                    if (prop != null && row[column] != DBNull.Value)
                    {
                        prop.SetValue(item, row[column]);
                    }
                }

                items.Add(item);
            }

            return items;
        }

        public string extrxtjson(string responsecontent)
        {
            int startindex = responsecontent.IndexOf("[");
            int EndIndex = responsecontent.IndexOf("]");
            if (startindex >= 0 && EndIndex >= 0 && EndIndex > startindex)
            {
                return responsecontent.Substring(startindex, EndIndex - startindex + 1);
            }
            throw new InvalidOperationException("failed to exract json");
        }
        public string Decrypturls(string encryptedText)
        {
            using var aesAlg = Aes.Create();
            //var keysection = _configuration.GetSection("AEskeys");
            string enkey =/* keysection["EncryptKey"]*/"3235835740747897878";
            aesAlg.Key = Encoding.UTF8.GetBytes(enkey);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] iv = new byte[aesAlg.IV.Length];
            Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);
            byte[] encryptedMessage = new byte[encryptedBytes.Length];
            Array.Copy(encryptedBytes, 0, encryptedMessage, 0, encryptedMessage.Length);

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);


                try
                {
                    JObject jsonObject = JObject.Parse(decryptedText);
                    return jsonObject.ToString();
                }
                catch (JsonReaderException)
                {

                    return decryptedText;
                }
            }
        }
    }

}
