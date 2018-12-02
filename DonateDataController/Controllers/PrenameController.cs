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
                    SQLString = @"INSERT INTO prename (PreNameID, PreName, PersonFlag, CorporateFlag,
                                  Blank1, Blank2, Blank3, Blank4, Blank5, Blank6)
                                  VALUES (@PreNameID, @PreName, @PersonFlag, @CorporateFlag,
                                  @Blank1, @Blank2, @Blank3, @Blank4, @Blank5, @Blank6);";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PreNameID", item.PreNameID);
                    qExe.Parameters.AddWithValue("@PreName", item.PreName);
                    qExe.Parameters.AddWithValue("@PersonFlag", item.PersonFlag);
                    qExe.Parameters.AddWithValue("@CorporateFlag", item.CorporateFlag);
                    qExe.Parameters.AddWithValue("@Blank1", item.Blank1);
                    qExe.Parameters.AddWithValue("@Blank2", item.Blank2);
                    qExe.Parameters.AddWithValue("@Blank3", item.Blank3);
                    qExe.Parameters.AddWithValue("@Blank4", item.Blank4);
                    qExe.Parameters.AddWithValue("@Blank5", item.Blank5);
                    qExe.Parameters.AddWithValue("@Blank6", item.Blank6);
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
                    SQLString = @"UPDATE prename SET PreNameID = @PreNameID, PreName = @PreName,
                                  PersonFlag = @PersonFlag, CorporateFlag = @CorporateFlag, Blank1 = @Blank1,
                                  Blank2 = @Blank2, Blank3 = @Blank3, Blank4 = @Blank4, Blank5 = @Blank5,
                                  Blank6 = @Blank6 WHERE PreNameID = @PreNameID;";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PreNameID", item.PreNameID);
                    qExe.Parameters.AddWithValue("@PreName", item.PreName);
                    qExe.Parameters.AddWithValue("@PersonFlag", item.PersonFlag);
                    qExe.Parameters.AddWithValue("@CorporateFlag", item.CorporateFlag);
                    qExe.Parameters.AddWithValue("@Blank1", item.Blank1);
                    qExe.Parameters.AddWithValue("@Blank2", item.Blank2);
                    qExe.Parameters.AddWithValue("@Blank3", item.Blank3);
                    qExe.Parameters.AddWithValue("@Blank4", item.Blank4);
                    qExe.Parameters.AddWithValue("@Blank5", item.Blank5);
                    qExe.Parameters.AddWithValue("@Blank6", item.Blank6);
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
                    string sqlString = @"delete from prename where PreNameID = @PreNameID";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    if (string.IsNullOrEmpty(id))
                        return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                    qExe.Parameters.AddWithValue("@PreNameID", id);
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
                SQLString = @"select * from prename order by PreNameID";
                qExe.CommandText = SQLString;
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PrenameModel detail = new PrenameModel();
                    detail.PreNameID = dataReader["PreNameID"].ToString();
                    detail.PreName = dataReader["PreName"].ToString();
                    detail.PersonFlag = dataReader["PersonFlag"].ToString();
                    detail.CorporateFlag = dataReader["CorporateFlag"].ToString();
                    detail.Blank1 = dataReader["Blank1"].ToString();
                    detail.Blank2 = dataReader["Blank2"].ToString();
                    detail.Blank3 = dataReader["Blank3"].ToString();
                    detail.Blank4 = dataReader["Blank4"].ToString();
                    detail.Blank5 = dataReader["Blank5"].ToString();
                    detail.Blank6 = dataReader["Blank6"].ToString();
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
                SQLString = @"select * from prename where PreNameID = @PreNameID";
                qExe.CommandText = SQLString;
                qExe.Parameters.AddWithValue("@PreNameRunno", id);
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PrenameModel detail = new PrenameModel();
                    detail.PreNameID = dataReader["PreNameID"].ToString();
                    detail.PreName = dataReader["PreName"].ToString();
                    detail.PersonFlag = dataReader["PersonFlag"].ToString();
                    detail.CorporateFlag = dataReader["CorporateFlag"].ToString();
                    detail.Blank1 = dataReader["Blank1"].ToString();
                    detail.Blank2 = dataReader["Blank2"].ToString();
                    detail.Blank3 = dataReader["Blank3"].ToString();
                    detail.Blank4 = dataReader["Blank4"].ToString();
                    detail.Blank5 = dataReader["Blank5"].ToString();
                    detail.Blank6 = dataReader["Blank6"].ToString();
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
