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
    public class DonatorController : ApiController
    {
        [Route("Donator/Add")]
        [HttpPost]
        public IHttpActionResult AddDonatorData([FromBody] DonatorData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"INSERT INTO donatordata (DonatorId, DonatorPreName, DonatorName, DonatorSurName,
                              DonatorCitizenId, DonatorRegisterNo, DonatorTaxId, HouseNumber, Soi, Road, Moo, Building, Tambon,
                              Amphur, Province, ZipCode, Telephone) VALUES (@DonatorId, @DonatorPreName,
                              @DonatorName, @DonatorSurName, @DonatorCitizenId, @DonatorRegisterNo, @DonatorTaxId, @HouseNumber,
                              @Soi, @Road, @Moo, @Building, @Tambon, @Amphur, @Province, @ZipCode, @Telephone)";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@DonatorId", item.DonatorId);
                qExe.Parameters.AddWithValue("@DonatorPreName", item.DonatorPreName);
                qExe.Parameters.AddWithValue("@DonatorName", item.DonatorName);
                qExe.Parameters.AddWithValue("@DonatorSurName", item.DonatorSurName);
                qExe.Parameters.AddWithValue("@DonatorCitizenId", item.DonatorCitizenId);
                qExe.Parameters.AddWithValue("@DonatorRegisterNo", item.DonatorRegisterNo);
                qExe.Parameters.AddWithValue("@DonatorTaxId", item.DonatorTaxId);
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
        [Route("Donator/Edit")]
        [HttpPost]
        public IHttpActionResult EditDonatorData([FromBody] DonatorData item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"UPDATE donatordata SET DonatorRunno = @DonatorRunno, DonatorId = @DonatorId, DonatorPreName = @DonatorPreName,
                              DonatorName = @DonatorName, DonatorSurName = @DonatorSurName, DonatorCitizenId = @DonatorCitizenId,
                              DonatorRegisterNo = @DonatorRegisterNo, DonatorTaxId = @DonatorTaxId, HouseNumber = @HouseNumber,
                              Soi = @Soi, Road = @Road, Moo = @Moo, Building = @Building, Tambon = @Tambon, Amphur = @Amphur,
                              Province = @Province, Zipcode = @Zipcode, Telephone = @Telephone WHERE DonatorRunno = @DonatorRunno ";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                qExe.Parameters.AddWithValue("@DonatorRunno", item.DonatorRunno);
                qExe.Parameters.AddWithValue("@DonatorId", item.DonatorId);
                qExe.Parameters.AddWithValue("@DonatorPreName", item.DonatorPreName);
                qExe.Parameters.AddWithValue("@DonatorName", item.DonatorName);
                qExe.Parameters.AddWithValue("@DonatorSurName", item.DonatorSurName);
                qExe.Parameters.AddWithValue("@DonatorCitizenId", item.DonatorCitizenId);
                qExe.Parameters.AddWithValue("@DonatorRegisterNo", item.DonatorRegisterNo);
                qExe.Parameters.AddWithValue("@DonatorTaxId", item.DonatorTaxId);
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
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }
        [Route("Donator/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteDonatorData(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"DELETE FROM donatordata WHERE DonatorRunno = @DonatorRunno";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
                if (string.IsNullOrEmpty(id)) return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                qExe.Parameters.AddWithValue("@DonatorRunno", id);
                qExe.ExecuteNonQuery();
                conn.CloseConnection();
                return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = "" });
            }
            else
            {
                return Json(new ResultDataModel { success = false, errorMessage = "Database connect fail!", returnRunno = "" });
            }
        }

        [Route("Donator/ListAll")]
        [HttpGet]
        public IHttpActionResult ListAllDonator()
        {
            List<DonatorData> result = new List<DonatorData>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM donatordata order by DonatorId";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        DonatorData detail = new DonatorData();
                        detail.DonatorRunno = int.Parse(dataReader["DonatorRunno"].ToString());
                        detail.DonatorId = dataReader["DonatorId"].ToString();
                        detail.DonatorPreName = dataReader["DonatorPreName"].ToString();
                        detail.DonatorName = dataReader["DonatorName"].ToString();
                        detail.DonatorSurName = dataReader["DonatorSurName"].ToString();
                        detail.DonatorCitizenId = dataReader["DonatorCitizenId"].ToString();
                        detail.DonatorRegisterNo = dataReader["DonatorRegisterNo"].ToString();
                        detail.DonatorTaxId = dataReader["DonatorTaxId"].ToString();
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
                        detail.DonatorFullName = dataReader["donatorPrename"].ToString() + dataReader["donatorname"].ToString() +
                                                 "  " + dataReader["donatorSurname"].ToString();
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
        [Route("Donator/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetDonatorData(string id)
        {
            DonatorData result = new DonatorData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"SELECT * FROM donatordata where DonatorRunno = '" + id + @"'
                              order by DonatorId";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.DonatorRunno = int.Parse(dataReader["DonatorRunno"].ToString());
                        result.DonatorId = dataReader["DonatorId"].ToString();
                        result.DonatorPreName = dataReader["DonatorPreName"].ToString();
                        result.DonatorName = dataReader["DonatorName"].ToString();
                        result.DonatorSurName = dataReader["DonatorSurName"].ToString();
                        result.DonatorCitizenId = dataReader["DonatorCitizenId"].ToString();
                        result.DonatorRegisterNo = dataReader["DonatorRegisterNo"].ToString();
                        result.DonatorTaxId = dataReader["DonatorTaxId"].ToString();
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
                        result.DonatorFullName = dataReader["donatorPrename"].ToString() + dataReader["donatorname"].ToString() +
                                                 "  " + dataReader["donatorSurname"].ToString();
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
