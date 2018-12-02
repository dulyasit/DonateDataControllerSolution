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
    public class MemberController : ApiController
    {
        [Route("Member/Add")]
        [HttpPost]
        public IHttpActionResult AddMemberData([FromBody] MemberData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"INSERT INTO memberdata
                              (MemberId, MemberPreName, MemberName, MemberSurname, PositionNo,
                              BirthDate, HouseNumber, Soi, Road, Moo, Building, Tambon, Amphur,
                              Province, ZipCode, Telephone, MemberCitizenID)
                              VALUES (@MemberId, @MemberPreName, @MemberName,
                              @MemberSurname, @PositionNo, @BirthDate, @HouseNumber, @Soi,
                              @Road, @Moo, @Building, @Tambon, @Amphur, @Province, @ZipCode, @Telephone, @MemberCitizenID)";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@MemberId", item.MemberId);
                qExe.Parameters.AddWithValue("@MemberPreName", item.MemberPreName);
                qExe.Parameters.AddWithValue("@MemberName", item.MemberName);
                qExe.Parameters.AddWithValue("@MemberSurname", item.MemberSurname);
                qExe.Parameters.AddWithValue("@PositionNo", item.PositionNo);
                qExe.Parameters.AddWithValue("@BirthDate", item.BirthDate);
                qExe.Parameters.AddWithValue("@HouseNumber", item.HouseNumber);
                qExe.Parameters.AddWithValue("@Soi", item.Soi);
                qExe.Parameters.AddWithValue("@Road", item.Road);
                qExe.Parameters.AddWithValue("@Moo", item.Moo);
                qExe.Parameters.AddWithValue("@Building", item.Building);
                qExe.Parameters.AddWithValue("@Tambon", item.Tambon);
                qExe.Parameters.AddWithValue("@Amphur", item.Amphur);
                qExe.Parameters.AddWithValue("@Province", item.Province);
                qExe.Parameters.AddWithValue("@ZipCode", item.ZipCode);
                qExe.Parameters.AddWithValue("@Telephone", item.Telephone);
                qExe.Parameters.AddWithValue("@MemberCitizenID", item.MemberCitizenID);
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
        [Route("Member/Edit")]
        [HttpPost]
        public IHttpActionResult EditMemberData([FromBody] MemberData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"UPDATE memberdata SET MemberId = @MemberId, MemberPreName = @MemberPreName,
                              MemberName = @MemberName, MemberSurname = @MemberSurname, PositionNo = @PositionNo, BirthDate = @BirthDate,
                              HouseNumber = @HouseNumber, Soi = @Soi, Road = @Road, Moo = @Moo, Building = @Building, Tambon = @Tambon,
                              Amphur = @Amphur, Province = @Province, ZipCode = @ZipCode, Telephone = @Telephone,
                              MemberCitizenID = @MemberCitizenID WHERE MemberRunno = @MemberRunno";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@MemberRunno", item.MemberRunno);
                qExe.Parameters.AddWithValue("@MemberId", item.MemberId);
                qExe.Parameters.AddWithValue("@MemberPreName", item.MemberPreName);
                qExe.Parameters.AddWithValue("@MemberName", item.MemberName);
                qExe.Parameters.AddWithValue("@MemberSurname", item.MemberSurname);
                qExe.Parameters.AddWithValue("@PositionNo", item.PositionNo);
                qExe.Parameters.AddWithValue("@BirthDate", item.BirthDate);
                qExe.Parameters.AddWithValue("@HouseNumber", item.HouseNumber);
                qExe.Parameters.AddWithValue("@Soi", item.Soi);
                qExe.Parameters.AddWithValue("@Road", item.Road);
                qExe.Parameters.AddWithValue("@Moo", item.Moo);
                qExe.Parameters.AddWithValue("@Building", item.Building);
                qExe.Parameters.AddWithValue("@Tambon", item.Tambon);
                qExe.Parameters.AddWithValue("@Amphur", item.Amphur);
                qExe.Parameters.AddWithValue("@Province", item.Province);
                qExe.Parameters.AddWithValue("@ZipCode", item.ZipCode);
                qExe.Parameters.AddWithValue("@Telephone", item.Telephone);
                qExe.Parameters.AddWithValue("@MemberCitizenID", item.MemberCitizenID);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("Member/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteMemberData(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"DELETE FROM memberdata WHERE MemberRunno = @MemberRunno";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                if (string.IsNullOrEmpty(id)) return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                qExe.Parameters.AddWithValue("@MemberRunno", id);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }

        [Route("Member/ListAll")]
        [HttpGet]
        public IHttpActionResult ListAllMember()
        {
            List<MemberData> result = new List<MemberData>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT t1.*, t2.PositionName FROM memberdata t1 left join partyposition t2 on t1.PositionNo = t2.PositionNo order by MemberId";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        MemberData detail = new MemberData();
                        detail.MemberRunno = int.Parse(dataReader["MemberRunno"].ToString());
                        detail.MemberId = dataReader["MemberId"].ToString();
                        detail.MemberPreName = dataReader["MemberPreName"].ToString();
                        detail.MemberName = dataReader["MemberName"].ToString();
                        detail.MemberSurname = dataReader["MemberSurname"].ToString();
                        detail.PositionNo = int.Parse(dataReader["PositionNo"].ToString());
                        detail.PositionName = dataReader["PositionName"].ToString();
                        detail.HouseNumber = dataReader["HouseNumber"].ToString();
                        detail.Soi = dataReader["Soi"].ToString();
                        detail.Road = dataReader["Road"].ToString();
                        detail.Moo = dataReader["Moo"].ToString();
                        detail.Building = dataReader["Building"].ToString();
                        detail.Tambon = dataReader["Tambon"].ToString();
                        detail.Amphur = dataReader["Amphur"].ToString();
                        detail.Province = dataReader["Province"].ToString();
                        detail.ZipCode = dataReader["ZipCode"].ToString();
                        detail.Telephone = dataReader["Telephone"].ToString();
                        detail.MemberFullName = dataReader["memberPrename"].ToString() + dataReader["membername"].ToString() +
                                                "   " + dataReader["MemberSurName"].ToString();
                        detail.MemberCitizenID = dataReader["MemberCitizenID"].ToString();
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
        [Route("Member/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetMemberData(string id)
        {
            MemberData result = new MemberData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT t1.*, t2.PositionName 
                              FROM memberdata t1 left join partyposition t2 on t1.PositionNo = t2.PositionNo
                              where MemberRunno = '" + id + @"'
                              order by MemberId";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.MemberRunno = int.Parse(dataReader["MemberRunno"].ToString());
                        result.MemberId = dataReader["MemberId"].ToString();
                        result.MemberPreName = dataReader["MemberPreName"].ToString();
                        result.MemberName = dataReader["MemberName"].ToString();
                        result.MemberSurname = dataReader["MemberSurname"].ToString();
                        result.PositionNo = int.Parse(dataReader["PositionNo"].ToString());
                        result.PositionName = dataReader["PositionName"].ToString();
                        result.HouseNumber = dataReader["HouseNumber"].ToString();
                        result.Soi = dataReader["Soi"].ToString();
                        result.Road = dataReader["Road"].ToString();
                        result.Moo = dataReader["Moo"].ToString();
                        result.Building = dataReader["Building"].ToString();
                        result.Tambon = dataReader["Tambon"].ToString();
                        result.Amphur = dataReader["Amphur"].ToString();
                        result.Province = dataReader["Province"].ToString();
                        result.ZipCode = dataReader["ZipCode"].ToString();
                        result.Telephone = dataReader["Telephone"].ToString();
                        result.MemberFullName = dataReader["memberPrename"].ToString() + dataReader["membername"].ToString() +
                                                "   " + dataReader["MemberSurName"].ToString();
                        result.MemberCitizenID = dataReader["MemberCitizenID"].ToString();
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
