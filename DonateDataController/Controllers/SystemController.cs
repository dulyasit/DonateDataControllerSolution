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
    public class SystemController : ApiController
    {
        [Route("MasterData/ListAll")]
        [HttpGet]
        public IHttpActionResult GetMasterData()
        {
            List<MasterData> result = new List<MasterData>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MySqlCommand qExe = new MySqlCommand();
                    qExe.Connection = conn.connection;
                    SQLString = @"select * from masterdata";
                    qExe.CommandText = SQLString;
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.Add(new MasterData
                        {
                            PartyName = dataReader["partyname"].ToString(),
                            HouseNumber = dataReader["housenumber"].ToString(),
                            Soi = dataReader["soi"].ToString(),
                            Road = dataReader["road"].ToString(),
                            Moo = dataReader["moo"].ToString(),
                            Building = dataReader["building"].ToString(),
                            Tambon = dataReader["tambon"].ToString(),
                            Amphur = dataReader["amphur"].ToString(),
                            Province = dataReader["province"].ToString(),
                            Zipcode = dataReader["zipcode"].ToString(),
                            Telephone = dataReader["telephone"].ToString(),
                            Slogan = dataReader["Slogan"].ToString(),
                            TaxID = dataReader["TaxID"].ToString()
                        });
                    }
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
        [Route("MasterData/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetMasterDatabyID(string id)
        {
            MasterData result = new MasterData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MySqlCommand qExe = new MySqlCommand();
                    qExe.Connection = conn.connection;
                    SQLString = @"select * from masterdata";
                    qExe.CommandText = SQLString;
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result = new MasterData
                        {
                            PartyName = dataReader["partyname"].ToString(),
                            HouseNumber = dataReader["housenumber"].ToString(),
                            Soi = dataReader["soi"].ToString(),
                            Road = dataReader["road"].ToString(),
                            Moo = dataReader["moo"].ToString(),
                            Building = dataReader["building"].ToString(),
                            Tambon = dataReader["tambon"].ToString(),
                            Amphur = dataReader["amphur"].ToString(),
                            Province = dataReader["province"].ToString(),
                            Zipcode = dataReader["zipcode"].ToString(),
                            Telephone = dataReader["telephone"].ToString(),
                            Slogan = dataReader["Slogan"].ToString(),
                            TaxID = dataReader["TaxID"].ToString()
                        };
                    }
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
        [Route("MasterData/Edit")]
        [HttpPost]
        public IHttpActionResult DonateDataEdit([FromBody] MasterData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"UPDATE masterdata SET partyname = @partyname, housenumber = @housenumber, soi = @soi,
                              road = @road, moo = @moo, building = @building, tambon = @tambon, amphur = @amphur,
                              province = @province, zipcode = @zipcode, telephone = @telephone, Slogan = @Slogan, TaxID = @TaxID";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@partyname", item.PartyName);
                    qExe.Parameters.AddWithValue("@housenumber", item.HouseNumber);
                    qExe.Parameters.AddWithValue("@soi", item.Soi);
                    qExe.Parameters.AddWithValue("@road", item.Road);
                    qExe.Parameters.AddWithValue("@moo", item.Moo);
                    qExe.Parameters.AddWithValue("@building", item.Building);
                    qExe.Parameters.AddWithValue("@tambon", item.Tambon);
                    qExe.Parameters.AddWithValue("@amphur", item.Amphur);
                    qExe.Parameters.AddWithValue("@province", item.Province);
                    qExe.Parameters.AddWithValue("@zipcode", item.Zipcode);
                    qExe.Parameters.AddWithValue("@telephone", item.Telephone);
                    qExe.Parameters.AddWithValue("@Slogan", item.Slogan);
                    qExe.Parameters.AddWithValue("@TaxID", item.TaxID);
                    qExe.ExecuteNonQuery();
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
        [Route("RunnumberData/ListAll")]
        [HttpGet]
        public IHttpActionResult GetRunnumberData()
        {
            List<RunnumberDataModel> result = new List<RunnumberDataModel>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MySqlCommand qExe = new MySqlCommand();
                    qExe.Connection = conn.connection;
                    SQLString = @"select * from sy_runnumber order by rn_key";
                    qExe.CommandText = SQLString;
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.Add(new RunnumberDataModel
                        {
                            rn_key = dataReader["rn_key"].ToString(),
                            rn_desc = dataReader["rn_desc"].ToString(),
                            rn_format = dataReader["rn_format"].ToString(),
                            rn_runnumber = int.Parse(dataReader["rn_runnumber"].ToString()),
                            rn_book = int.Parse(dataReader["rn_book"].ToString()),
                            rn_bookno = int.Parse(dataReader["rn_bookno"].ToString())
                        });
                    }
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
        [Route("RunnumberData/Edit")]
        [HttpPost]
        public IHttpActionResult RunnumberDataEdit([FromBody] RunnumberDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"UPDATE sy_runnumber SET rn_key = @rn_key,
                                  rn_desc = @rn_desc, rn_format = @rn_format,
                                  rn_runnumber = @rn_runnumber, rn_book = @rn_book,
                                  rn_bookno = @rn_bookno WHERE rn_key = @rn_key";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@rn_key", item.rn_key);
                    qExe.Parameters.AddWithValue("@rn_desc", item.rn_desc);
                    qExe.Parameters.AddWithValue("@rn_format", item.rn_format);
                    qExe.Parameters.AddWithValue("@rn_runnumber", item.rn_runnumber);
                    qExe.Parameters.AddWithValue("@rn_book", item.rn_book);
                    qExe.Parameters.AddWithValue("@rn_bookno", item.rn_bookno);
                    qExe.ExecuteNonQuery();
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

    }
}
