using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using UtilityControllers.Models;
using UtilityLib;

namespace DonateDataController.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class GenDocumentController : ApiController
    {
        [Route("GenDocumentPK11/{documentRunno}")]
        [HttpPost]
        public IHttpActionResult GenDocumentPK11(string documentRunno)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            UtilLibs utilLibs = new UtilLibs();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"select docs.*, dt.DonateAssetType 
                                         from donatedetaildata docs left join donatetype dt 
                                                 on docs.DonateTypeRunno = dt.DonateTypeRunno 
                                         where DocumentRunno = @DocumentRunno
                                         and dt.DonateAssetType = '1'
                                         and (docs.DocRefNo is null or docs.DocRefNo = '')
                                         order by docs.DetailRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("DocumentRunno", documentRunno);
                    MySqlDataReader detailReader = qExe.ExecuteReader();
                    bool CashType = false;
                    bool BillType = false;
                    bool ChqueType = false;
                    double DonateAmount = 0;
                    List<int> RunnoList = new List<int>();
                    while (detailReader.Read())
                    {
                        if (string.IsNullOrEmpty(detailReader["BankChqueNo"].ToString()) &&
                            string.IsNullOrEmpty(detailReader["Bill"].ToString()))
                            CashType = true;
                        else if (!string.IsNullOrEmpty(detailReader["BankChqueNo"].ToString()) &&
                                 string.IsNullOrEmpty(detailReader["Bill"].ToString()))
                            ChqueType = true;
                        else if (string.IsNullOrEmpty(detailReader["BankChqueNo"].ToString()) &&
                                 !string.IsNullOrEmpty(detailReader["Bill"].ToString()))
                            BillType = true;
                        DonateAmount = DonateAmount + double.Parse(detailReader["Amount"].ToString());
                        RunnoList.Add(int.Parse(detailReader["DetailRunno"].ToString()));
                    }
                    detailReader.Close();

                    if (CashType || ChqueType || BillType)
                    {
                        SQLString = @"select * from donatedata where DocumentRunno = @DocumentRunno";
                        qExe = new MySqlCommand
                        {
                            Connection = conn.connection,
                            CommandText = SQLString
                        };
                        qExe.Parameters.AddWithValue("DocumentRunno", documentRunno);
                        detailReader = qExe.ExecuteReader();
                        DocumentNumber docNum = new DocumentNumber();
                        DateTime docDate = DateTime.Now;
                        int DonatorRunno = -1;
                        while (detailReader.Read())
                        {
                            DonatorRunno = int.Parse(detailReader["DonatorRunno"].ToString());
                        }
                        detailReader.Close();
                        if (DonateAmount > 0 && DonatorRunno != -1)
                        {
                            docNum = utilLibs.GetDocumentNo("pk11");
                            SQLString = @"INSERT INTO document11 (DocumentType, DocBookNo, DocNo,
                                          DocDate, DonatorRunno, DonateAmount, CashFlag, BillFlag, ChqueFlag)
                                          VALUES (@DocumentType, @DocBookNo, @DocNo, @DocDate, @DonatorRunno,
                                          @DonateAmount, @CashFlag, @BillFlag, @ChqueFlag)";
                            qExe = new MySqlCommand
                            {
                                Connection = conn.connection,
                                CommandText = SQLString
                            };
                            qExe.Parameters.AddWithValue("DocumentType", "PK11");
                            qExe.Parameters.AddWithValue("DocBookNo", docNum.BookNo);
                            qExe.Parameters.AddWithValue("DocNo", docNum.DocumentNo);
                            qExe.Parameters.AddWithValue("DocDate", docDate);
                            qExe.Parameters.AddWithValue("DonatorRunno", DonatorRunno);
                            qExe.Parameters.AddWithValue("DonateAmount", DonateAmount);
                            if (CashType)
                                qExe.Parameters.AddWithValue("CashFlag", "Y");
                            else
                                qExe.Parameters.AddWithValue("CashFlag", "N");
                            if (BillType)
                                qExe.Parameters.AddWithValue("BillFlag", "Y");
                            else
                                qExe.Parameters.AddWithValue("BillFlag", "N");
                            if (ChqueType)
                                qExe.Parameters.AddWithValue("ChqueFlag", "Y");
                            else
                                qExe.Parameters.AddWithValue("ChqueFlag", "N");
                            qExe.ExecuteNonQuery();
                            long returnid = qExe.LastInsertedId;
                            SQLString = @"update donatedetaildata set DocRefNo = @DocRefNo, DocRefBook = @DocRefBook, DocType = @DocType
                                          where DetailRunno = @DetailRunno";
                            qExe = new MySqlCommand
                            {
                                Connection = conn.connection,
                                CommandText = SQLString
                            };
                            foreach (var runno in RunnoList)
                            {
                                qExe.Parameters.Clear();
                                qExe.Parameters.AddWithValue("DetailRunno", runno);
                                qExe.Parameters.AddWithValue("DocRefNo", docNum.DocumentNo);
                                qExe.Parameters.AddWithValue("DocRefBook", docNum.BookNo);
                                qExe.Parameters.AddWithValue("DocType", "PK11");
                                qExe.ExecuteNonQuery();
                            }
                            conn.CloseConnection();
                            return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = returnid.ToString() });
                        }
                        else
                        {
                            return Json(new ResultDataModel { success = false, errorMessage = "ข้อมูลไม่ถูกต้องไม่สามารถสร้างเอกสาร พ.ก. 11 ได้  หรือทำการสร้างเอกสารไปแล้ว", returnRunno = "" });
                        }
                    }
                    else
                        return Json(new ResultDataModel { success = true, errorMessage = "ไม่มีข้อมูลในประเภทเอกสาร พ.ก. 11  หรือทำการสร้างเอกสารไปแล้ว", returnRunno = "" });
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
        [Route("GenDocumentPK12/{documentRunno}")]
        [HttpPost]
        public IHttpActionResult GenDocumentPK12(string documentRunno)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            UtilLibs utilLibs = new UtilLibs();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"select docs.*, dt.DonateAssetType 
                                         from donatedetaildata docs left join donatetype dt 
                                                 on docs.DonateTypeRunno = dt.DonateTypeRunno 
                                         where DocumentRunno = @DocumentRunno
                                         and dt.DonateAssetType = '2'
                                         and (docs.DocRefNo is null or docs.DocRefNo = '')
                                         order by docs.DetailRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("DocumentRunno", documentRunno);
                    MySqlDataReader detailReader = qExe.ExecuteReader();
                    bool AssetFlag = false;
                    bool BenefitFlag = false;
                    double DonateAmount = 0;
                    double BenefitAmount = 0;
                    double AssetAmount = 0;
                    string AssetDesc = "";
                    string BenefitDesc = "";
                    List<int> RunnoList = new List<int>();
                    while (detailReader.Read())
                    {
                        if (!string.IsNullOrEmpty(detailReader["Benefit"].ToString()))
                        {
                            BenefitFlag = true;
                            BenefitAmount = BenefitAmount + double.Parse(detailReader["Amount"].ToString());
                            BenefitDesc = detailReader["Benefit"].ToString();
                        }
                        if (!string.IsNullOrEmpty(detailReader["Asset"].ToString()))
                        {
                            AssetFlag = true;
                            AssetAmount = AssetAmount + double.Parse(detailReader["Amount"].ToString());
                            AssetDesc = detailReader["Asset"].ToString();
                        }
                        DonateAmount = DonateAmount + double.Parse(detailReader["Amount"].ToString());
                        RunnoList.Add(int.Parse(detailReader["DetailRunno"].ToString()));
                    }
                    detailReader.Close();

                    if (BenefitFlag || AssetFlag)
                    {
                        SQLString = @"select * from donatedata where DocumentRunno = @DocumentRunno";
                        qExe = new MySqlCommand
                        {
                            Connection = conn.connection,
                            CommandText = SQLString
                        };
                        qExe.Parameters.AddWithValue("DocumentRunno", documentRunno);
                        detailReader = qExe.ExecuteReader();
                        DocumentNumber docNum = new DocumentNumber();
                        DateTime docDate = DateTime.Now;
                        int DonatorRunno = -1;
                        while (detailReader.Read())
                        {
                            DonatorRunno = int.Parse(detailReader["DonatorRunno"].ToString());
                        }
                        detailReader.Close();
                        if (DonateAmount > 0 && DonatorRunno != -1)
                        {
                            docNum = utilLibs.GetDocumentNo("pk12");
                            SQLString = @"INSERT INTO document12 (DocumentType, DocBookNo, DocNo, DocDate, DonatorRunno, AssetFlag,
                                          AssetAmount, BenefitFlag, BenefitAmount, DonateAmount, AssetDesc, BenefitDesc) VALUES
                                          (@DocumentType, @DocBookNo, @DocNo, @DocDate, @DonatorRunno, @AssetFlag, @AssetAmount, @BenefitFlag,
                                           @BenefitAmount, @DonateAmount, @AssetDesc, @BenefitDesc)";
                            qExe = new MySqlCommand
                            {
                                Connection = conn.connection,
                                CommandText = SQLString
                            };
                            qExe.Parameters.AddWithValue("DocumentType", "PK12");
                            qExe.Parameters.AddWithValue("DocBookNo", docNum.BookNo);
                            qExe.Parameters.AddWithValue("DocNo", docNum.DocumentNo);
                            qExe.Parameters.AddWithValue("DocDate", docDate);
                            qExe.Parameters.AddWithValue("DonatorRunno", DonatorRunno);
                            qExe.Parameters.AddWithValue("DonateAmount", DonateAmount);
                            qExe.Parameters.AddWithValue("AssetAmount", AssetAmount);
                            qExe.Parameters.AddWithValue("BenefitAmount", BenefitAmount);
                            if (AssetFlag)
                                qExe.Parameters.AddWithValue("AssetFlag", "Y");
                            else
                                qExe.Parameters.AddWithValue("AssetFlag", "N");
                            if (BenefitFlag)
                                qExe.Parameters.AddWithValue("BenefitFlag", "Y");
                            else
                                qExe.Parameters.AddWithValue("BenefitFlag", "N");
                            qExe.Parameters.AddWithValue("AssetDesc", AssetDesc);
                            qExe.Parameters.AddWithValue("BenefitDesc", BenefitDesc);
                            qExe.ExecuteNonQuery();
                            long returnid = qExe.LastInsertedId;
                            SQLString = @"update donatedetaildata set DocRefNo = @DocRefNo, DocRefBook = @DocRefBook, DocType = @DocType
                                          where DetailRunno = @DetailRunno";
                            qExe = new MySqlCommand
                            {
                                Connection = conn.connection,
                                CommandText = SQLString
                            };
                            foreach (var runno in RunnoList)
                            {
                                qExe.Parameters.Clear();
                                qExe.Parameters.AddWithValue("DetailRunno", runno);
                                qExe.Parameters.AddWithValue("DocRefNo", docNum.DocumentNo);
                                qExe.Parameters.AddWithValue("DocRefBook", docNum.BookNo);
                                qExe.Parameters.AddWithValue("DocType", "PK12");
                                qExe.ExecuteNonQuery();
                            }
                            conn.CloseConnection();
                            return Json(new ResultDataModel { success = true, errorMessage = "", returnRunno = returnid.ToString() });
                        }
                        else
                        {
                            return Json(new ResultDataModel { success = false, errorMessage = "ข้อมูลไม่ถูกต้องไม่สามารถสร้างเอกสาร พ.ก. 12 ได้  หรือทำการสร้างเอกสารไปแล้ว", returnRunno = "" });
                        }
                    }
                    else
                        return Json(new ResultDataModel { success = true, errorMessage = "ไม่มีข้อมูลในประเภทเอกสาร พ.ก. 12 หรือทำการสร้างเอกสารไปแล้ว", returnRunno = "" });
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

        [Route("DocPK11/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetDocPK11byID(string id)
        {
            UtilLibs utilLibs = new UtilLibs();            
            PK11Model result = new PK11Model();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MasterData masterData = GetMasterData();
                    SQLString = @"select doc.*, dt.*
                                  from document11 doc left join donatordata dt on doc.DonatorRunno = dt.DonatorRunno
                                  where doc.DocumentRunno = '" + id + "'";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        result.PartyName = masterData.PartyName;
                        result.PartyTel = masterData.Telephone;
                        result.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        result.PartyAddr1 = GetAddrRow1(masterData);
                        result.PartyAddr2 = GetAddrRow2(masterData);
                        result.DocBookNo = dataReader["DocBookNo"].ToString();
                        result.DocNum = dataReader["DocNo"].ToString();
                        result.DocDate = DateTime.Parse(dataReader["DocDate"].ToString());
                        result.DocDateStr = DateTime.Parse(dataReader["DocDate"].ToString()).ToString("dd/MM/yyyy");
                        result.DonatorName = (dataReader["DonatorPreName"].ToString() + dataReader["DonatorName"].ToString() + "   " + dataReader["DonatorSurName"].ToString()).Trim();
                        result.CitizenID = dataReader["DonatorCitizenID"].ToString();
                        result.RegisterID = dataReader["DonatorregisterNo"].ToString();
                        result.TaxID = dataReader["DonatorTaxId"].ToString();
                        result.HouseNum = dataReader["HouseNumber"].ToString();
                        result.Moo = dataReader["Moo"].ToString();
                        result.Building = dataReader["Building"].ToString();
                        result.Soi = dataReader["Soi"].ToString();
                        result.Road = dataReader["Road"].ToString();
                        result.Tambon = dataReader["Tambon"].ToString();
                        result.Amphur = dataReader["Amphur"].ToString();
                        result.Province = dataReader["Province"].ToString();
                        result.Zipcode = dataReader["Zipcode"].ToString();
                        result.Telephone = dataReader["Telephone"].ToString();
                        result.CashFlag = dataReader["CashFlag"].ToString();
                        result.BillFlag = dataReader["BillFlag"].ToString();
                        result.ChqueFlag = dataReader["ChqueFlag"].ToString();
                        result.Amount = double.Parse(dataReader["DonateAmount"].ToString());
                        result.AmountStr = utilLibs.ThaiBaht(dataReader["DonateAmount"].ToString());
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
        [Route("DocPK11/ListAll")]
        [HttpGet]
        public IHttpActionResult GetDocPK11ListAll()
        {
            List<PK11Model> result = new List<PK11Model>();
            UtilLibs utilLibs = new UtilLibs();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MasterData masterData = GetMasterData();
                    SQLString = @"select doc.*, dt.*
                                  from document11 doc left join donatordata dt on doc.DonatorRunno = dt.DonatorRunno order by DocBookNo, DocNo";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        PK11Model row = new PK11Model();
                        row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        row.PartyName = masterData.PartyName;
                        row.PartyTel = masterData.Telephone;
                        row.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        row.PartyAddr1 = GetAddrRow1(masterData);
                        row.PartyAddr2 = GetAddrRow2(masterData);
                        row.DocBookNo = dataReader["DocBookNo"].ToString();
                        row.DocNum = dataReader["DocNo"].ToString();
                        row.DocDate = DateTime.Parse(dataReader["DocDate"].ToString());
                        row.DocDateStr = DateTime.Parse(dataReader["DocDate"].ToString()).ToString("dd/MM/yyyy");
                        row.DonatorName = (dataReader["DonatorPreName"].ToString() + dataReader["DonatorName"].ToString() + "   " + dataReader["DonatorSurName"].ToString()).Trim();
                        row.CitizenID = dataReader["DonatorCitizenID"].ToString();
                        row.RegisterID = dataReader["DonatorregisterNo"].ToString();
                        row.TaxID = dataReader["DonatorTaxId"].ToString();
                        row.HouseNum = dataReader["HouseNumber"].ToString();
                        row.Moo = dataReader["Moo"].ToString();
                        row.Building = dataReader["Building"].ToString();
                        row.Soi = dataReader["Soi"].ToString();
                        row.Road = dataReader["Road"].ToString();
                        row.Tambon = dataReader["Tambon"].ToString();
                        row.Amphur = dataReader["Amphur"].ToString();
                        row.Province = dataReader["Province"].ToString();
                        row.Zipcode = dataReader["Zipcode"].ToString();
                        row.Telephone = dataReader["Telephone"].ToString();
                        row.CashFlag = dataReader["CashFlag"].ToString();
                        row.BillFlag = dataReader["BillFlag"].ToString();
                        row.ChqueFlag = dataReader["ChqueFlag"].ToString();
                        row.Amount = double.Parse(dataReader["DonateAmount"].ToString());
                        row.AmountStr = utilLibs.ThaiBaht(dataReader["DonateAmount"].ToString());
                        result.Add(row);
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
        [Route("DocPK12/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetDocPK12byID(string id)
        {
            PK12Model result = new PK12Model();
            UtilLibs utilLibs = new UtilLibs();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MasterData masterData = GetMasterData();
                    SQLString = @"select doc.*, dt.*
                                  from document12 doc left join donatordata dt on doc.DonatorRunno = dt.DonatorRunno
                                  where doc.DocumentRunno = '" + id + "'";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        result.PartyName = masterData.PartyName;
                        result.PartyTel = masterData.Telephone;
                        result.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        result.PartyAddr1 = GetAddrRow1(masterData);
                        result.PartyAddr2 = GetAddrRow2(masterData);
                        result.DocBookNo = dataReader["DocBookNo"].ToString();
                        result.DocNum = dataReader["DocNo"].ToString();
                        result.DocDate = DateTime.Parse(dataReader["DocDate"].ToString());
                        result.DocDateStr = DateTime.Parse(dataReader["DocDate"].ToString()).ToString("dd/MM/yyyy");
                        result.DonatorName = (dataReader["DonatorPreName"].ToString() + dataReader["DonatorName"].ToString() + "   " + dataReader["DonatorSurName"].ToString()).Trim();
                        result.CitizenID = dataReader["DonatorCitizenID"].ToString();
                        result.RegisterID = dataReader["DonatorregisterNo"].ToString();
                        result.TaxID = dataReader["DonatorTaxId"].ToString();
                        result.HouseNum = dataReader["HouseNumber"].ToString();
                        result.Moo = dataReader["Moo"].ToString();
                        result.Building = dataReader["Building"].ToString();
                        result.Soi = dataReader["Soi"].ToString();
                        result.Road = dataReader["Road"].ToString();
                        result.Tambon = dataReader["Tambon"].ToString();
                        result.Amphur = dataReader["Amphur"].ToString();
                        result.Province = dataReader["Province"].ToString();
                        result.Zipcode = dataReader["Zipcode"].ToString();
                        result.Telephone = dataReader["Telephone"].ToString();
                        result.AssetFlag = dataReader["AssetFlag"].ToString();
                        result.AssetAmount = double.Parse(dataReader["AssetAmount"].ToString());
                        result.AssetAmountStr = utilLibs.ThaiBaht(dataReader["AssetAmount"].ToString());
                        result.AssetDesc = dataReader["AssetDesc"].ToString();
                        result.BenefitFlag = dataReader["BenefitFlag"].ToString();
                        result.BenefitAmount = double.Parse(dataReader["BenefitAmount"].ToString());
                        result.BenefitAmountStr = utilLibs.ThaiBaht(dataReader["BenefitAmount"].ToString());
                        result.BenefitDesc = dataReader["BenefitDesc"].ToString();
                        result.Amount = double.Parse(dataReader["DonateAmount"].ToString());
                        result.AmountStr = utilLibs.ThaiBaht(dataReader["DonateAmount"].ToString());
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
        [Route("DocPK12/ListAll")]
        [HttpGet]
        public IHttpActionResult GetDocPK12ListAll()
        {
            List<PK12Model> result = new List<PK12Model>();
            UtilLibs utilLibs = new UtilLibs();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            string SQLString;
            if (conn.OpenConnection())
            {
                try
                {
                    MasterData masterData = GetMasterData();
                    SQLString = @"select doc.*, dt.*
                                  from document12 doc left join donatordata dt on doc.DonatorRunno = dt.DonatorRunno
                                  order by DocBookNo, DocNo";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        PK12Model row = new PK12Model();
                        row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        row.PartyName = masterData.PartyName;
                        row.PartyTel = masterData.Telephone;
                        row.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        row.PartyAddr1 = GetAddrRow1(masterData);
                        row.PartyAddr2 = GetAddrRow2(masterData);
                        row.DocBookNo = dataReader["DocBookNo"].ToString();
                        row.DocNum = dataReader["DocNo"].ToString();
                        row.DocDate = DateTime.Parse(dataReader["DocDate"].ToString());
                        row.DocDateStr = DateTime.Parse(dataReader["DocDate"].ToString()).ToString("dd/MM/yyyy");
                        row.DonatorName = (dataReader["DonatorPreName"].ToString() + dataReader["DonatorName"].ToString() + "   " + dataReader["DonatorSurName"].ToString()).Trim();
                        row.CitizenID = dataReader["DonatorCitizenID"].ToString();
                        row.RegisterID = dataReader["DonatorregisterNo"].ToString();
                        row.TaxID = dataReader["DonatorTaxId"].ToString();
                        row.HouseNum = dataReader["HouseNumber"].ToString();
                        row.Moo = dataReader["Moo"].ToString();
                        row.Building = dataReader["Building"].ToString();
                        row.Soi = dataReader["Soi"].ToString();
                        row.Road = dataReader["Road"].ToString();
                        row.Tambon = dataReader["Tambon"].ToString();
                        row.Amphur = dataReader["Amphur"].ToString();
                        row.Province = dataReader["Province"].ToString();
                        row.Zipcode = dataReader["Zipcode"].ToString();
                        row.Telephone = dataReader["Telephone"].ToString();
                        row.AssetFlag = dataReader["AssetFlag"].ToString();
                        row.AssetAmount = double.Parse(dataReader["AssetAmount"].ToString());
                        row.AssetAmountStr = utilLibs.ThaiBaht(dataReader["AssetAmount"].ToString());
                        row.AssetDesc = dataReader["AssetDesc"].ToString();
                        row.BenefitFlag = dataReader["BenefitFlag"].ToString();
                        row.BenefitAmount = double.Parse(dataReader["BenefitAmount"].ToString());
                        row.BenefitAmountStr = utilLibs.ThaiBaht(dataReader["BenefitAmount"].ToString());
                        row.BenefitDesc = dataReader["BenefitDesc"].ToString();
                        row.Amount = double.Parse(dataReader["DonateAmount"].ToString());
                        row.AmountStr = utilLibs.ThaiBaht(dataReader["DonateAmount"].ToString());
                        result.Add(row);
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
