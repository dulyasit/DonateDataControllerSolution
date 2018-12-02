using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using UtilityControllers.Models;
using UtilityLib;

namespace DonateDataController.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class PK13DataController : ApiController
    {
        [Route("PK13Data/ListTerm")]
        [HttpGet]
        public IHttpActionResult GetPK13Term()
        {
            List<TermofPK13> result = new List<TermofPK13>();
            UtilLibs Ulib = new UtilLibs();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString = @"select distinct year(DocumentDate) vYear, month(DocumentDate) vMonth from donatedata";
            if (conn.OpenConnection())
            {
                try
                {
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TermofPK13
                            {
                                Month = dataReader["vMonth"].ToString(),
                                Year = dataReader["vYear"].ToString(),
                                TermName = Ulib.getMonthShortName("th", int.Parse(dataReader["vMonth"].ToString())) + " " + (int.Parse(dataReader["vYear"].ToString()) + 543).ToString()
                            });
                        }
                        return Json(result);
                    }
                    else
                    {
                        return BadRequest("Data Empty");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
                return BadRequest("Database connect fail!");
        }

        public PK13Model getPK13Data(DateTime BDate, DateTime EDate)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            UtilLibs Ulib = new UtilLibs();
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            MasterData masterData = GetMasterData();

            result.PartyName = masterData.PartyName;
            result.AnnouDay = DateTime.Now.Day.ToString();
            result.AnnouMonth = Ulib.getMonthShortName("th", DateTime.Now.Month);
            result.AnnouYear = (DateTime.Now.Year + 543).ToString();
            result.SDay = BDate.Day.ToString();
            result.SMonth = Ulib.getMonthShortName("th", BDate.Month);
            result.SYear = (BDate.Year + 543).ToString();
            result.EDay = EDate.Day.ToString();
            result.EMonth = Ulib.getMonthShortName("th", EDate.Month);
            result.EYear = (EDate.Year + 543).ToString();
            result.ErrorMessage = "";
            double SumCash = 0;
            double SumAsset = 0;
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"select tb1.DocumentRunno, tb1.DonatorRunno, tb1.DocumentDate,
                                  tb2.Amount, tb2.DonateTypeRunno, tb2.DetailRunno, 
                                  tb3.DonatorPreName, tb3.DonatorName, tb3.DonatorSurName, tb3.DonatorCitizenId,
                                  tb3.HouseNumber, tb3.Moo, tb3.Building, tb3.Soi, tb3.Road, tb3.Tambon, 
                                  tb3.Amphur, tb3.Province, tb3.Zipcode, tb3.Telephone,
                                  tb3.Career, tb3.Nationality
                                  from donatedata tb1, donatedetaildata tb2, donatordata tb3
                                  where tb1.DocumentRunno = tb2.DocumentRunno
                                  and tb1.DonatorRunno = tb3.DonatorRunno
                                  and tb1.DocumentDate >= @BDATE
                                  and tb1.DocumentDate <= @EDATE
                                  and (tb3.DonatorTaxId is null or tb3.DonatorTaxId = '')
                                  order by DocumentDate, DonatorName";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("BDATE", BDate);
                    qExe.Parameters.AddWithValue("EDATE", EDate);
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        DateTime vDate = new DateTime();
                        PK13DetailModel detail = new PK13DetailModel();

                        while (dataReader.Read())
                        {
                            if (vDate != DateTime.Parse(dataReader["DocumentDate"].ToString()) ||
                                detail.Name != dataReader["DonatorName"].ToString())
                            {
                                if (vDate != new DateTime())
                                    result.DetailData.Add(detail);

                                vDate = DateTime.Parse(dataReader["DocumentDate"].ToString());
                                detail = new PK13DetailModel();
                                detail.PreName = dataReader["DonatorPreName"].ToString();
                                detail.Name = dataReader["DonatorName"].ToString();
                                detail.SurName = dataReader["DonatorSurName"].ToString();
                                detail.Telephone = dataReader["Telephone"].ToString();
                                detail.Addr1 = GetAddrRow1(dataReader["HouseNumber"].ToString(),
                                                           dataReader["Moo"].ToString(),
                                                           dataReader["Building"].ToString(),
                                                           dataReader["Soi"].ToString(),
                                                           dataReader["Road"].ToString());
                                detail.Addr2 = GetAddrRow2(dataReader["Tambon"].ToString(),
                                                           dataReader["Amphur"].ToString(),
                                                           dataReader["Province"].ToString(),
                                                           dataReader["Zipcode"].ToString());
                                detail.CitizenID = dataReader["DonatorCitizenId"].ToString();
                                detail.DateStr = vDate.Day + " " + Ulib.getMonthShortName("th", vDate.Month) + " " +
                                                 (vDate.Year + 543);
                                detail.Career = dataReader["Career"].ToString();
                                detail.Nationality = dataReader["Nationality"].ToString();
                                detail.Cash = 0;
                                detail.Asset = 0;
                            }
                            else
                            {
                                if (dataReader["DonateTypeRunno"].ToString() == "1" ||
                                    dataReader["DonateTypeRunno"].ToString() == "5" ||
                                    dataReader["DonateTypeRunno"].ToString() == "6")
                                {
                                    detail.Cash = detail.Cash + double.Parse(dataReader["Amount"].ToString());
                                    SumCash = SumCash + double.Parse(dataReader["Amount"].ToString());
                                }
                                else
                                {
                                    detail.Asset = detail.Asset + double.Parse(dataReader["Amount"].ToString());
                                    SumAsset = SumAsset + double.Parse(dataReader["Amount"].ToString());
                                }
                            }
                        }
                        result.DetailData.Add(detail);
                        result.TotalCash = SumCash;
                        result.TotalAsset = SumAsset;
                        result.TotalAmount = SumCash + SumAsset;
                        result.TotalAmountStr = Ulib.ThaiBaht(result.TotalAmount.ToString());
                        return result;
                    }
                    else
                    {
                        result.ErrorMessage = "Data Empty";
                        return result;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }
            else
            {
                result.ErrorMessage = "Database connect fail!";
                return result;
            }
        }

        [Route("PK13Data/GetbyDateRange/{S_Day}/{S_Month}/{S_Year}/{E_Day}/{E_Month}/{E_Year}")]
        [HttpGet]
        public IHttpActionResult GetPK13ByDate(int S_Day, int S_Month, int S_Year, int E_Day, int E_Month, int E_Year)
        {
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            DateTime BDate = new DateTime(S_Year, S_Month, S_Day);
            DateTime EDate = new DateTime(E_Year, E_Month, E_Day);

            result = getPK13Data(BDate, EDate);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(result.ErrorMessage);
            return Json(result);
        }
        [Route("PK13Data/{month}/{year}")]
        [HttpGet]
        public IHttpActionResult GetPK13byMonth(int month, int year)
        {
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            DateTime BDate = new DateTime(year, month, 1);
            DateTime EDate = BDate.AddMonths(1).AddDays(-1);
            result = getPK13Data(BDate, EDate);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(result.ErrorMessage);
            return Json(result);
        }

        public PK13Model getPK13_1Data(DateTime BDate, DateTime EDate)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            UtilLibs Ulib = new UtilLibs();
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            MasterData masterData = GetMasterData();

            result.PartyName = masterData.PartyName;
            result.AnnouDay = DateTime.Now.Day.ToString();
            result.AnnouMonth = Ulib.getMonthShortName("th", DateTime.Now.Month);
            result.AnnouYear = (DateTime.Now.Year + 543).ToString();
            result.SDay = BDate.Day.ToString();
            result.SMonth = Ulib.getMonthShortName("th", BDate.Month);
            result.SYear = (BDate.Year + 543).ToString();
            result.EDay = EDate.Day.ToString();
            result.EMonth = Ulib.getMonthShortName("th", EDate.Month);
            result.EYear = (EDate.Year + 543).ToString();
            result.ErrorMessage = "";
            double SumCash = 0;
            double SumAsset = 0;
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    SQLString = @"select tb1.DocumentRunno, tb1.DonatorRunno, tb1.DocumentDate,
                                  tb2.Amount, tb2.DonateTypeRunno, tb2.DetailRunno, 
                                  tb3.DonatorPreName, tb3.DonatorName, tb3.DonatorSurName, tb3.DonatorCitizenId,
                                  tb3.HouseNumber, tb3.Moo, tb3.Building, tb3.Soi, tb3.Road, tb3.Tambon, 
                                  tb3.Amphur, tb3.Province, tb3.Zipcode, tb3.Telephone,
                                  tb3.DonatorTaxId, tb3.DonatorRegisterNo, tb3.ThaiPercent, tb3.ForeignPercent
                                  from donatedata tb1, donatedetaildata tb2, donatordata tb3
                                  where tb1.DocumentRunno = tb2.DocumentRunno
                                  and tb1.DonatorRunno = tb3.DonatorRunno
                                  and tb1.DocumentDate >= @BDATE
                                  and tb1.DocumentDate <= @EDATE
                                  and (tb3.DonatorTaxId is null or tb3.DonatorTaxId = '')
                                  order by DocumentDate, DonatorName";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("BDATE", BDate);
                    qExe.Parameters.AddWithValue("EDATE", EDate);
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        DateTime vDate = new DateTime();
                        PK13DetailModel detail = new PK13DetailModel();

                        while (dataReader.Read())
                        {
                            if (vDate != DateTime.Parse(dataReader["DocumentDate"].ToString()) ||
                                detail.Name != dataReader["DonatorName"].ToString())
                            {
                                if (vDate != new DateTime())
                                    result.DetailData.Add(detail);

                                vDate = DateTime.Parse(dataReader["DocumentDate"].ToString());
                                detail = new PK13DetailModel();
                                detail.PreName = dataReader["DonatorPreName"].ToString();
                                detail.Name = dataReader["DonatorName"].ToString();
                                detail.SurName = dataReader["DonatorSurName"].ToString();
                                detail.Telephone = dataReader["Telephone"].ToString();
                                detail.Addr1 = GetAddrRow1(dataReader["HouseNumber"].ToString(),
                                                           dataReader["Moo"].ToString(),
                                                           dataReader["Building"].ToString(),
                                                           dataReader["Soi"].ToString(),
                                                           dataReader["Road"].ToString());
                                detail.Addr2 = GetAddrRow2(dataReader["Tambon"].ToString(),
                                                           dataReader["Amphur"].ToString(),
                                                           dataReader["Province"].ToString(),
                                                           dataReader["Zipcode"].ToString());                                
                                detail.DateStr = vDate.Day + " " + Ulib.getMonthShortName("th", vDate.Month) + " " +
                                                 (vDate.Year + 543);
                                detail.ThaiPercent = double.Parse(dataReader["ThaiPercent"].ToString());
                                detail.ForeignPercent = double.Parse(dataReader["ForeignPercent"].ToString());
                                detail.DonatorRegisterNo = dataReader["DonatorRegisterNo"].ToString();
                                detail.DonatorTaxId = dataReader["DonatorTaxId"].ToString();
                                detail.Cash = 0;
                                detail.Asset = 0;
                            }
                            else
                            {
                                if (dataReader["DonateTypeRunno"].ToString() == "1" ||
                                    dataReader["DonateTypeRunno"].ToString() == "5" ||
                                    dataReader["DonateTypeRunno"].ToString() == "6")
                                {
                                    detail.Cash = detail.Cash + double.Parse(dataReader["Amount"].ToString());
                                    SumCash = SumCash + double.Parse(dataReader["Amount"].ToString());
                                }
                                else
                                {
                                    detail.Asset = detail.Asset + double.Parse(dataReader["Amount"].ToString());
                                    SumAsset = SumAsset + double.Parse(dataReader["Amount"].ToString());
                                }
                            }
                        }
                        result.DetailData.Add(detail);
                        result.TotalCash = SumCash;
                        result.TotalAsset = SumAsset;
                        result.TotalAmount = SumCash + SumAsset;
                        result.TotalAmountStr = Ulib.ThaiBaht(result.TotalAmount.ToString());
                        return result;
                    }
                    else
                    {
                        result.ErrorMessage = "Data Empty";
                        return result;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }
            else
            {
                result.ErrorMessage = "Database connect fail!";
                return result;
            }
        }

        [Route("PK13_1Data/GetbyDateRange/{S_Day}/{S_Month}/{S_Year}/{E_Day}/{E_Month}/{E_Year}")]
        [HttpGet]
        public IHttpActionResult GetPK13_1ByDate(int S_Day, int S_Month, int S_Year, int E_Day, int E_Month, int E_Year)
        {
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            DateTime BDate = new DateTime(S_Year, S_Month, S_Day);
            DateTime EDate = new DateTime(E_Year, E_Month, E_Day);
            result = getPK13Data(BDate, EDate);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(result.ErrorMessage);
            return Json(result);
        }
        [Route("PK13_1Data/{month}/{year}")]
        [HttpGet]
        public IHttpActionResult GetPK13_1byMonth(int month, int year)
        {
            PK13Model result = new PK13Model();
            result.DetailData = new List<PK13DetailModel>();
            DateTime BDate = new DateTime(year, month, 1);
            DateTime EDate = BDate.AddMonths(1).AddDays(-1);
            result = getPK13Data(BDate, EDate);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(result.ErrorMessage);
            return Json(result);
        }
        private string GetAddrRow1(string HouseNumber, string Moo, string Building, string Soi, string Road)
        {
            string result = "";
            if (!string.IsNullOrEmpty(HouseNumber))
                result = "เลขที่ " + HouseNumber;
            if (!string.IsNullOrEmpty(Moo))
                result = result + " หมู่ที่ " + Moo;
            if (!string.IsNullOrEmpty(Building))
                result = result + " อาคาร " + Building;
            if (!string.IsNullOrEmpty(Soi))
                result = result + " ซอย " + Soi;
            if (!string.IsNullOrEmpty(Road))
                result = result + " ถนน " + Road;
            return result.Trim();
        }
        private string GetAddrRow2(string Tambon, string Amphur, string Province, string Zipcode)
        {
            string result = "";
            if (!string.IsNullOrEmpty(Tambon))
                if (Province.Contains("กรุงเทพ") || Province.Contains("กทม"))
                    result = result + " แขวง " + Tambon;
                else
                    result = result + " ตำบล " + Tambon;
            if (!string.IsNullOrEmpty(Amphur))
                if (Province.Contains("กรุงเทพ") || Province.Contains("กทม"))
                    result = "เขต " + Amphur;
                else
                    result = "อำเภอ " + Amphur;
            if (!string.IsNullOrEmpty(Province))
                result = result + " จังหวัด " + Province;
            if (!string.IsNullOrEmpty(Zipcode))
                result = result + " " + Zipcode;
            return result.Trim();
        }
        private MasterData GetMasterData()
        {
            MasterData result = new MasterData();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                SQLString = @"select * from masterdata";
                MySqlCommand qExe = new MySqlCommand
                {
                    Connection = conn.connection,
                    CommandText = SQLString
                };
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
            }
            return result;
        }
    }
}
