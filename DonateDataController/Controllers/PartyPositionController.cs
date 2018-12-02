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
    public class PartyPositionController : ApiController
    {
        [Route("PartyPosition/Add")]
        [HttpPost]
        public IHttpActionResult PartyPositionAdd([FromBody] PartyPosition item)
        {            
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"insert into partyposition (PositionNo, PositionName) values (@PositionNo, @PositionName) ";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PositionNo", item.PositionNo);
                    qExe.Parameters.AddWithValue("@PositionName", item.PositionName);
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
        [Route("PartyPosition/Edit")]
        [HttpPost]
        public IHttpActionResult PartyPositionEdit([FromBody] PartyPosition item)
        {
            List<PartyPosition> result = new List<PartyPosition>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"update partyposition set PositionRunno = @PositionRunno, PositionNo = @PositionNo, 
                                  PositionName = @PositionName where PositionRunno = @PositionRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@PositionRunno", item.PositionRunno);
                    qExe.Parameters.AddWithValue("@PositionNo", item.PositionNo);
                    qExe.Parameters.AddWithValue("@PositionName", item.PositionName);
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
        [Route("PartyPosition/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult PartyPositionDelete(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString = @"delete from PartyPosition where PositionRunno = @PositionRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    if (string.IsNullOrEmpty(id))
                        return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                    qExe.Parameters.AddWithValue("@PositionRunno", id);
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
        [Route("PartyPosition/ListAll")]
        [HttpGet]
        public IHttpActionResult GetPartyPositionList()
        {
            List<PartyPosition> result = new List<PartyPosition>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                MySqlCommand qExe = new MySqlCommand();
                qExe.Connection = conn.connection;
                SQLString = @"select positionrunno, positionno, positionname from partyposition
                              order by positionno";
                qExe.CommandText = SQLString;
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PartyPosition detail = new PartyPosition();
                    detail.PositionRunno = int.Parse(dataReader["PositionRunno"].ToString());
                    detail.PositionNo = int.Parse(dataReader["PositionNo"].ToString());
                    detail.PositionName = dataReader["PositionName"].ToString();
                    result.Add(detail);
                }
                return Json(result);
            }
            else
            {
                return BadRequest("Database connect fail!");
            }
        }
        [Route("PartyPosition/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetPartyPositionListbyRunno(string id)
        {
            PartyPosition result = new PartyPosition();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                MySqlCommand qExe = new MySqlCommand();
                qExe.Connection = conn.connection;
                SQLString = @"select positionrunno, positionno, positionname from partyposition
                              where PositionRunno = @PositionRunno
                              order by positionno";
                qExe.CommandText = SQLString;
                qExe.Parameters.AddWithValue("@PositionRunno", id);
                MySqlDataReader dataReader = qExe.ExecuteReader();
                while (dataReader.Read())
                {
                    PartyPosition detail = new PartyPosition();
                    detail.PositionRunno = int.Parse(dataReader["PositionRunno"].ToString());
                    detail.PositionNo = int.Parse(dataReader["PositionNo"].ToString());
                    detail.PositionName = dataReader["PositionName"].ToString();
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
