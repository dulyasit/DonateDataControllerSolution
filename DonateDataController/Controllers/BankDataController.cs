using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using UtilityControllers.Models;

namespace DonateDataController.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class BankDataController : ApiController
    {
        [Route("BankData/Add")]
        [HttpPost]
        public IHttpActionResult AddDonateTypeData([FromBody] BankData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"insert into bank_name (bank_code, bank_name) values (@bank_code, @bank_name)";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@bank_code", item.BankCode);
                qExe.Parameters.AddWithValue("@bank_name", item.BankName);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = item.BankCode });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("BankData/Edit")]
        [HttpPost]
        public IHttpActionResult EditDonateTypeData([FromBody] BankData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"UPDATE bank_name SET bank_name = @bank_name WHERE bank_code = @bank_code";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@bank_code", item.BankCode);
                qExe.Parameters.AddWithValue("@bank_name", item.BankName);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("BankData/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteDonateTypeData(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"DELETE FROM bank_name WHERE bank_code = @bank_code";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                if (string.IsNullOrEmpty(id)) return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                qExe.Parameters.AddWithValue("@bank_code", id);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }

        [Route("BankData/ListAll")]
        [HttpGet]
        public IHttpActionResult ListAllDonateType()
        {
            List<BankData> result = new List<BankData>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM bank_name order by bank_code";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        BankData detail = new BankData();
                        detail.BankCode = dataReader["bank_code"].ToString();
                        detail.BankName = dataReader["bank_name"].ToString();
                        result.Add(detail);
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    return Json(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Database connect fail!");
            }
        }
        [Route("BankData/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetDonateTypeData(string id)
        {
            BankData result = new BankData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM bank_name where bank_code = '" + id + @"'
                              order by bank_code";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.BankCode = dataReader["bank_code"].ToString();
                        result.BankName = dataReader["bank_name"].ToString();
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    return Json(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Database connect fail!");
            }
        }
    }
}
