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
    public class PrenameController : ApiController
    {
        [Route("PreName/Add")]
        [HttpPost]
        public IHttpActionResult PreNameAdd([FromBody] PrenameModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"insert into prename (PreName) values (@PreName) ";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PreName", item.PreName);
                    qExe.ExecuteNonQuery();
                    long returnid = qExe.LastInsertedId;
                    conn.CloseConnection();
                    return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = returnid.ToString() });
                }
                catch (Exception e)
                {
                    return Json(new ResultDataModel { success = false, errorMessage = e.Message, returnRunno = "" });
                }
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }

        [Route("PreName/Edit")]
        [HttpPost]
        public IHttpActionResult PreNameEdit([FromBody] PrenameModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"UPDATE prename SET PreName = @PreName WHERE PreNameRunno = @PreNameRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PreNameRunno", item.PreNameRunno);
                    qExe.Parameters.AddWithValue("@PreName", item.PreName);
                    qExe.ExecuteNonQuery();
                    conn.CloseConnection();
                    return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
                }
                catch (Exception e)
                {
                    return Json(new ResultDataModel { success = false, errorMessage = e.Message, returnRunno = "" });
                }
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }

        }
        [Route("PreName/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult PreNameDelete(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString = @"delete from prename where PreNameRunno = @PreNameRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    if (string.IsNullOrEmpty(id))
                        return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                    qExe.Parameters.AddWithValue("@PreNameRunno", id);
                    qExe.ExecuteNonQuery();
                    conn.CloseConnection();
                    return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
                }
                catch (Exception e)
                {
                    return Json(new ResultDataModel { success = false, errorMessage = e.Message, returnRunno = "" });
                }
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("PreName/ListAll")]
        [HttpGet]
        public IHttpActionResult GetPreNameList()
        {
            List<PrenameModel> result = new List<PrenameModel>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                MySqlCommand qExe = new MySqlCommand();
                qExe.Connection = conn.connection;
                SQLString = @"select PreNameRunno, PreName from prename order by PreNameRunno";
                qExe.CommandText = SQLString;
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PrenameModel detail = new PrenameModel();
                    detail.PreNameRunno = int.Parse(dataReader["PreNameRunno"].ToString());
                    detail.PreName = dataReader["PreName"].ToString();
                    result.Add(detail);
                }
                return Json(result);
            }
            else
            {
                return BadRequest("Database connect fail!");
            }
        }
        [Route("PreName/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetPreNameListbyRunno(string id)
        {
            PrenameModel result = new PrenameModel();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                MySqlCommand qExe = new MySqlCommand();
                qExe.Connection = conn.connection;
                SQLString = @"select PreNameRunno, PreName from prename where PreNameRunno = @PreNameRunno";
                qExe.CommandText = SQLString;
                qExe.Parameters.AddWithValue("@PreNameRunno", id);
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PrenameModel detail = new PrenameModel();
                    detail.PreNameRunno = int.Parse(dataReader["PreNameRunno"].ToString());
                    detail.PreName = dataReader["PreName"].ToString();
                    result = detail;
                }
                return Json(result);
            }
            else
            {
                return BadRequest("Database connect fail!");
            }
        }

    }
}
