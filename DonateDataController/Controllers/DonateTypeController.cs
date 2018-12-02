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
    public class DonateTypeController : ApiController
    {
        [Route("DonateType/Add")]
        [HttpPost]
        public IHttpActionResult AddDonateTypeData([FromBody] DonateTypeData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"insert into donatetype (DonateTypeName) values (@DonateTypeName)";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@DonateTypeName", item.DonateTypeName);
                qExe.ExecuteNonQuery();
                long returnid = qExe.LastInsertedId;
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = returnid.ToString() });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("DonateType/Edit")]
        [HttpPost]
        public IHttpActionResult EditDonateTypeData([FromBody] DonateTypeData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"UPDATE donatetype SET DonateTypeRunno = @DonateTypeRunno, DonateTypeName = @DonateTypeName WHERE DonateTypeRunno = @DonateTypeRunno";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@DonateTypeRunno", item.DonateTypeRunno);
                qExe.Parameters.AddWithValue("@DonateTypeName", item.DonateTypeName);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("DonateType/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteDonateTypeData(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"DELETE FROM donatetype WHERE DonateTypeRunno = @DonateTypeRunno";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                if (string.IsNullOrEmpty(id)) return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                qExe.Parameters.AddWithValue("@DonateTypeRunno", id);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }

        [Route("DonateType/ListAll")]
        [HttpGet]
        public IHttpActionResult ListAllDonateType()
        {
            List<DonateTypeData> result = new List<DonateTypeData>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM donatetype order by DonateTypeRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        DonateTypeData detail = new DonateTypeData();
                        detail.DonateTypeRunno = int.Parse(dataReader["DonateTypeRunno"].ToString());
                        detail.DonateTypeName = dataReader["DonateTypeName"].ToString();
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
        [Route("DonateType/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetDonateTypeData(string id)
        {
            DonateTypeData result = new DonateTypeData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM donatetype where DonateTypeRunno = '" + id + @"'
                              order by DonateTypeRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.DonateTypeRunno = int.Parse(dataReader["DonateTypeRunno"].ToString());
                        result.DonateTypeName = dataReader["DonateTypeName"].ToString();
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
