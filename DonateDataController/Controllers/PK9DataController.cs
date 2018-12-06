using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using UtilityControllers.Models;
using UtilityLib;

namespace DonateDataController.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class PK9DataController : ApiController
    {
        [Route("PK9Data/Add")]
        [HttpPost]
        public IHttpActionResult PK9DataAdd([FromBody] ReceiptDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            UtilLibs utilLibs = new UtilLibs();
            DocumentNumber docNum = new DocumentNumber();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"INSERT INTO receiptdata(DocumentBookNumber, DocumentNumber, DocumentDate, PayerType, 
                                         PayerRunno, AsReceiptTo, AsReceiptToRemark,
                                         ReceiptAmount, ReceiptPaytype, ReceiptDate, ReceiptBank, ReceiptBillNumber, 
                                         ReceiptChqBank, ReceiptChqNumber, ReceiptOther)
                                         VALUES (@DocumentBookNumber, @DocumentNumber, @DocumentDate, @PayerType, @PayerRunno, 
                                         @AsReceiptTo, @AsReceiptToRemark, @ReceiptAmount,
                                         @ReceiptPaytype, @ReceiptDate, @ReceiptBank, @ReceiptBillNumber, @ReceiptChqBank, 
                                         @ReceiptChqNumber, @ReceiptOther) ";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    docNum = utilLibs.GetDocumentNo("pk9");
                    qExe.Parameters.AddWithValue("@documentbooknumber", docNum.BookNo);
                    qExe.Parameters.AddWithValue("@documentnumber", docNum.DocumentNo);
                    qExe.Parameters.AddWithValue("@documentdate", item.DocumentDate);
                    qExe.Parameters.AddWithValue("@PayerType", item.PayerType);
                    qExe.Parameters.AddWithValue("@PayerRunno", item.PayerRunno);
                    qExe.Parameters.AddWithValue("@AsReceiptTo", item.AsReceiptTo);
                    qExe.Parameters.AddWithValue("@AsReceiptToRemark", item.AsReceiptToRemark);
                    qExe.Parameters.AddWithValue("@ReceiptAmount", item.ReceiptAmount);
                    qExe.Parameters.AddWithValue("@ReceiptPaytype", item.ReceiptPayType);
                    qExe.Parameters.AddWithValue("@ReceiptDate", item.ReceiptDate);
                    qExe.Parameters.AddWithValue("@ReceiptBank", item.ReceiptBank);
                    qExe.Parameters.AddWithValue("@ReceiptBillNumber", item.ReceiptBillNumber);
                    qExe.Parameters.AddWithValue("@ReceiptChqBank", item.ReceiptChqBank);
                    qExe.Parameters.AddWithValue("@ReceiptChqNumber", item.ReceiptChqNumber);
                    qExe.Parameters.AddWithValue("@ReceiptOther", item.ReceiptOther);
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
        [Route("PK9Data/Edit")]
        [HttpPost]
        public IHttpActionResult PK9DataEdit([FromBody] ReceiptDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"UPDATE receiptdata SET DocumentRunno = @DocumentRunno, DocumentBookNumber = @DocumentBookNumber,
                                         DocumentNumber = @DocumentNumber, DocumentDate = @DocumentDate, PayerType = @PayerType,
                                         PayerRunno = @PayerRunno, AsReceiptTo = @AsReceiptTo, AsReceiptToRemark = @AsReceiptToRemark,
                                         ReceiptAmount = @ReceiptAmount, ReceiptPaytype = @ReceiptPaytype, ReceiptDate = @ReceiptDate,
                                         ReceiptBank = @ReceiptBank, ReceiptBillNumber = @ReceiptBillNumber, ReceiptChqBank = @ReceiptChqBank,
                                         ReceiptChqNumber = @ReceiptChqNumber, ReceiptOther = @ReceiptOther WHERE DocumentRunno = @DocumentRunno;";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@documentrunno", item.DocumentRunno);
                    qExe.Parameters.AddWithValue("@documentbooknumber", item.DocumentBookNumber);
                    qExe.Parameters.AddWithValue("@documentnumber", item.DocumentNumber);
                    qExe.Parameters.AddWithValue("@documentdate", item.DocumentDate);
                    qExe.Parameters.AddWithValue("@PayerType", item.PayerType);
                    qExe.Parameters.AddWithValue("@PayerRunno", item.PayerRunno);
                    qExe.Parameters.AddWithValue("@AsReceiptTo", item.AsReceiptTo);
                    qExe.Parameters.AddWithValue("@AsReceiptToRemark", item.AsReceiptToRemark);
                    qExe.Parameters.AddWithValue("@ReceiptAmount", item.ReceiptAmount);
                    qExe.Parameters.AddWithValue("@ReceiptPaytype", item.ReceiptPayType);
                    qExe.Parameters.AddWithValue("@ReceiptDate", item.ReceiptDate);
                    qExe.Parameters.AddWithValue("@ReceiptBank", item.ReceiptBank);
                    qExe.Parameters.AddWithValue("@ReceiptBillNumber", item.ReceiptBillNumber);
                    qExe.Parameters.AddWithValue("@ReceiptChqBank", item.ReceiptChqBank);
                    qExe.Parameters.AddWithValue("@ReceiptChqNumber", item.ReceiptChqNumber);
                    qExe.Parameters.AddWithValue("@ReceiptOther", item.ReceiptOther);
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
        [Route("PK9Data/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult PK9DataDelete(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"delete from receiptdata where documentrunno = @documentrunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new ResultDataModel { success = false, errorMessage = "Document Key is null!", returnRunno = "" });
                    }
                    qExe.Parameters.AddWithValue("@documentrunno", id);
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
        [Route("PK9Data/ListAll")]
        [HttpGet]
        public IHttpActionResult PK9ListAll()
        {
            UtilLibs utilLibs = new UtilLibs();
            MasterData masterData = GetMasterData();
            List<ReceiptDataModel> result = new List<ReceiptDataModel>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString = @"select tb1.*, 
                                         tb2.MemberId PayerID, tb2.MemberPreName PreName, 
                                         tb2.MemberName Name, tb2.MemberSurname SurName,
                                         tb2.HouseNumber HouseNum, tb2.Soi, tb2.Road, 
                                         tb2.Moo, tb2.Building, tb2.Tambon,
                                         tb2.Amphur, tb2.Province, tb2.Zipcode, 
                                         tb2.Telephone, tb2.MemberCitizenId CitizenID
                                         from receiptdata tb1, memberdata tb2
                                         where tb1.PayerRunno = tb2.MemberRunno
                                         and tb1.PayerType = '1'
                                         union
                                         select tb1.*, 
                                         tb2.DonatorId PayerID, tb2.DonatorPreName PreName, 
                                         tb2.DonatorName Name, tb2.DonatorSurName SurName,
                                         tb2.HouseNumber, tb2.Soi, tb2.Road, 
                                         tb2.Moo, tb2.Building, tb2.Tambon,
                                         tb2.Amphur, tb2.Province, tb2.Zipcode, 
                                         tb2.Telephone, tb2.DonatorCitizenId CitizenID
                                         from receiptdata tb1, donatordata tb2
                                         where tb1.PayerRunno = tb2.DonatorRunno
                                         and tb1.PayerType = '2'
                                         order by DocumentRunno";
                    /*
                    string sqlString = @"select tb1.*, tb2.* from receiptdata tb1, memberdata tb2
                                         where tb1.PayerRunno = tb2.MemberRunno
                                         order by tb1.DocumentRunno";
                    */
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        string CashFlag = "";
                        string BankFlag = "";
                        string BillFlag = "";
                        string ChqFlag = "";
                        string OtherFlag = "";
                        if (string.IsNullOrEmpty(dataReader["ReceiptBank"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptBillNumber"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptOther"].ToString()))
                            CashFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptBank"].ToString()) &&
                                 string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()))
                            BankFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptBillNumber"].ToString()))
                            BillFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()))
                            ChqFlag = "Y";
                        else
                            OtherFlag = "Y";
                        ReceiptDataModel detail = new ReceiptDataModel
                        {
                            DocumentRunno = int.Parse(dataReader["documentrunno"].ToString()),
                            DocumentBookNumber = dataReader["documentbooknumber"].ToString(),
                            DocumentNumber = dataReader["documentnumber"].ToString(),
                            DocumentDate = Convert.ToDateTime(dataReader["DocumentDate"].ToString(), new CultureInfo("en-US")),
                            DocumentDateStr = Convert.ToDateTime(dataReader["DocumentDate"].ToString()).ToString("dd/MM/yyyy"),
                            PayerType = dataReader["PayerType"].ToString(),
                            PayerRunno = int.Parse(dataReader["PayerRunno"].ToString()),
                            AsReceiptTo = dataReader["AsReceiptTo"].ToString(),
                            AsReceiptToRemark = dataReader["AsReceiptToRemark"].ToString(),
                            ReceiptAmount = double.Parse(dataReader["ReceiptAmount"].ToString()),
                            HouseNumber = dataReader["HouseNum"].ToString(),
                            Soi = dataReader["Soi"].ToString(),
                            Road = dataReader["Road"].ToString(),
                            Moo = dataReader["Moo"].ToString(),
                            Building = dataReader["Building"].ToString(),
                            Tambon = dataReader["Tambon"].ToString(),
                            Amphur = dataReader["Amphur"].ToString(),
                            Province = dataReader["Province"].ToString(),
                            Zipcode = dataReader["Zipcode"].ToString(),
                            Telephone = dataReader["Telephone"].ToString(),
                            PayerId = dataReader["PayerID"].ToString(),
                            PayerName = dataReader["PreName"] + dataReader["Name"].ToString() + "   " + dataReader["SurName"],
                            ReceiptPayType = dataReader["ReceiptPayType"].ToString(),
                            ReceiptDate = Convert.ToDateTime(dataReader["ReceiptDate"].ToString(), new CultureInfo("en-US")),
                            ReceiptDateStr = Convert.ToDateTime(dataReader["ReceiptDate"].ToString()).ToString("dd/MM/yyyy"),
                            ReceiptBank = dataReader["ReceiptBank"].ToString(),
                            BankName = utilLibs.GetBankName(dataReader["ReceiptBank"].ToString()),
                            ReceiptBillNumber = dataReader["ReceiptBillNumber"].ToString(),
                            ReceiptChqBank = dataReader["ReceiptChqBank"].ToString(),
                            ReceiptChqNumber = dataReader["ReceiptChqNumber"].ToString(),
                            ReceiptOther = dataReader["ReceiptOther"].ToString(),
                            CitizenId = dataReader["CitizenID"].ToString(),
                            AmountStr = utilLibs.ThaiBaht(dataReader["ReceiptAmount"].ToString()),
                            PartyName = masterData.PartyName,
                            PartyTel = masterData.Telephone,
                            PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID,
                            PartyAddr1 = GetAddrRow1(masterData),
                            PartyAddr2 = GetAddrRow2(masterData),
                            CashFlag = CashFlag,
                            BankFlag = BankFlag,
                            BillFlag = BillFlag,
                            ChqFlag = ChqFlag,
                            OtherFlag = OtherFlag
                        };
                        result.Add(detail);
                    }
                    dataReader.Close();
                    conn.CloseConnection();
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

        [Route("PK9Data/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult PK9GetByID(string id)
        {
            UtilLibs utilLibs = new UtilLibs();
            MasterData masterData = GetMasterData();
            ReceiptDataModel result = new ReceiptDataModel();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString;
                    if (!string.IsNullOrEmpty(id))
                        sqlString = @"select tb1.*, 
                                      tb2.MemberId PayerID, tb2.MemberPreName PreName, 
                                      tb2.MemberName Name, tb2.MemberSurname SurName,
                                      tb2.HouseNumber HouseNum, tb2.Soi, tb2.Road, 
                                      tb2.Moo, tb2.Building, tb2.Tambon,
                                      tb2.Amphur, tb2.Province, tb2.Zipcode, 
                                      tb2.Telephone, tb2.MemberCitizenId CitizenID
                                      from receiptdata tb1, memberdata tb2
                                      where tb1.PayerRunno = tb2.MemberRunno
                                      and tb1.PayerType = '1'
                                      and tb1.DocumentRunno = @DocumentRunno
                                      union
                                      select tb1.*, 
                                      tb2.DonatorId PayerID, tb2.DonatorPreName PreName, 
                                      tb2.DonatorName Name, tb2.DonatorSurName SurName,
                                      tb2.HouseNumber, tb2.Soi, tb2.Road, 
                                      tb2.Moo, tb2.Building, tb2.Tambon,
                                      tb2.Amphur, tb2.Province, tb2.Zipcode, 
                                      tb2.Telephone, tb2.DonatorCitizenId CitizenID
                                      from receiptdata tb1, donatordata tb2
                                      where tb1.PayerRunno = tb2.DonatorRunno
                                      and tb1.PayerType = '2'
                                      and tb1.DocumentRunno = @DocumentRunno";
                    else
                        return Json("Document Number is blank!");

                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    qExe.Parameters.AddWithValue("@DocumentRunno", id);

                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        string CashFlag = "";
                        string BankFlag = "";
                        string BillFlag = "";
                        string ChqFlag = "";
                        string OtherFlag = "";
                        if (string.IsNullOrEmpty(dataReader["ReceiptBank"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptBillNumber"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()) &&
                            string.IsNullOrEmpty(dataReader["ReceiptOther"].ToString()))
                            CashFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptBank"].ToString()) &&
                                 string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()))
                            BankFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptBillNumber"].ToString()))
                            BillFlag = "Y";
                        else if (!string.IsNullOrEmpty(dataReader["ReceiptChqBank"].ToString()))
                            ChqFlag = "Y";
                        else
                            OtherFlag = "Y";
                        result.DocumentRunno = int.Parse(dataReader["documentrunno"].ToString());
                        result.DocumentBookNumber = dataReader["documentbooknumber"].ToString();
                        result.DocumentNumber = dataReader["documentnumber"].ToString();
                        result.DocumentDate = Convert.ToDateTime(dataReader["documentdate"].ToString(), new CultureInfo("en-US"));
                        result.DocumentDateStr = Convert.ToDateTime(dataReader["DocumentDate"].ToString()).ToString("dd/MM/yyyy");
                        result.PayerType = dataReader["PayerType"].ToString();
                        result.PayerRunno = int.Parse(dataReader["PayerRunno"].ToString());
                        result.AsReceiptTo = dataReader["AsReceiptTo"].ToString();
                        result.AsReceiptToRemark = dataReader["AsReceiptToRemark"].ToString();
                        result.ReceiptAmount = double.Parse(dataReader["ReceiptAmount"].ToString());
                        result.HouseNumber = dataReader["HouseNum"].ToString();
                        result.Soi = dataReader["Soi"].ToString();
                        result.Road = dataReader["Road"].ToString();
                        result.Moo = dataReader["Moo"].ToString();
                        result.Building = dataReader["Building"].ToString();
                        result.Tambon = dataReader["Tambon"].ToString();
                        result.Amphur = dataReader["Amphur"].ToString();
                        result.Province = dataReader["Province"].ToString();
                        result.Zipcode = dataReader["Zipcode"].ToString();
                        result.Telephone = dataReader["Telephone"].ToString();
                        result.PayerId = dataReader["PayerID"].ToString();
                        result.PayerName = dataReader["PreName"].ToString() + dataReader["Name"].ToString() + "   " + dataReader["SurName"].ToString();
                        result.ReceiptPayType = dataReader["ReceiptPayType"].ToString();
                        result.ReceiptDate = Convert.ToDateTime(dataReader["ReceiptDate"].ToString(), new CultureInfo("en-US"));
                        result.ReceiptDateStr = Convert.ToDateTime(dataReader["ReceiptDate"].ToString()).ToString("dd/MM/yyyy");
                        result.ReceiptBank = dataReader["ReceiptBank"].ToString();
                        result.BankName = utilLibs.GetBankName(dataReader["ReceiptBank"].ToString());
                        result.ReceiptBillNumber = dataReader["ReceiptBillNumber"].ToString();
                        result.ReceiptChqBank = dataReader["ReceiptChqBank"].ToString();
                        result.ReceiptChqNumber = dataReader["ReceiptChqNumber"].ToString();
                        result.ReceiptOther = dataReader["ReceiptOther"].ToString();
                        result.CitizenId = dataReader["CitizenID"].ToString();
                        result.AmountStr = utilLibs.ThaiBaht(dataReader["ReceiptAmount"].ToString());
                        result.PartyName = masterData.PartyName;
                        result.PartyTel = masterData.Telephone;
                        result.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        result.PartyAddr1 = GetAddrRow1(masterData);
                        result.PartyAddr2 = GetAddrRow2(masterData);
                        result.CashFlag = CashFlag;
                        result.BankFlag = BankFlag;
                        result.BillFlag = BillFlag;
                        result.ChqFlag = ChqFlag;
                        result.OtherFlag = OtherFlag;
                    }
                    dataReader.Close();
                    conn.CloseConnection();
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
        private string GetAddrRow1(MasterData masterData)
        {
            string result = "";
            if (!string.IsNullOrEmpty(masterData.HouseNumber))
                result = "เลขที่ " + masterData.HouseNumber;
            if (!string.IsNullOrEmpty(masterData.Moo))
                result = result + " หมู่ที่ " + masterData.Moo;
            if (!string.IsNullOrEmpty(masterData.Building))
                result = result + " อาคาร " + masterData.Building;
            if (!string.IsNullOrEmpty(masterData.Soi))
                result = result + " ซอย " + masterData.Soi;
            if (!string.IsNullOrEmpty(masterData.Road))
                result = result + " ถนน " + masterData.Road;
            if (!string.IsNullOrEmpty(masterData.Tambon))
                if (masterData.Province.Contains("กรุงเทพ") || masterData.Province.Contains("กทม"))
                    result = result + " แขวง " + masterData.Tambon;
                else
                    result = result + " ตำบล " + masterData.Tambon;
            return result.Trim();
        }
        private string GetAddrRow2(MasterData masterData)
        {
            string result = "";
            if (!string.IsNullOrEmpty(masterData.Amphur))
                if (masterData.Province.Contains("กรุงเทพ") || masterData.Province.Contains("กทม"))
                    result = "เขต " + masterData.Amphur;
                else
                    result = "อำเภอ " + masterData.Amphur;
            if (!string.IsNullOrEmpty(masterData.Province))
                result = result + " จังหวัด " + masterData.Province;
            if (!string.IsNullOrEmpty(masterData.Zipcode))
                result = result + " " + masterData.Zipcode;
            if (!string.IsNullOrEmpty(masterData.Telephone))
                result = result + " โทร. " + masterData.Telephone;
            return result.Trim();
        }
    }
}
