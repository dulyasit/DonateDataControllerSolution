using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using UtilityControllers.Models;

namespace DonateDataController.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class DonateDataController : ApiController
    {
        [Route("DonateData/Add")]
        [HttpPost]
        public IHttpActionResult DonateDataAdd([FromBody] DonateDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLDetailString =
                        @"INSERT INTO donatedetaildata (DocumentRunno, DetailRunno, Description, Amount, Remark)
                      VALUES (@DocumentRunno, @DetailRunno, @Description, @Amount, @Remark)";
                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailString
                    };
                    var detail = item.DonateDetail;

                    string SQLString =
                        @"INSERT INTO donatedata (WriteAt, DocumentDate, MemberRunno, MemberId,
                      DonateType, DonateObjective, DonatorRunno, DonatorId, DonateAmount)
                      VALUES (@WriteAt, @DocumentDate, @MemberRunno, @MemberId,
                      @DonateType, @DonateObjective, @DonatorRunno, @DonatorId, @DonateAmount )";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@WriteAt", item.WriteAt);
                    qExe.Parameters.AddWithValue("@DocumentDate", item.DocumentDate);
                    qExe.Parameters.AddWithValue("@MemberRunno", item.MemberRunno);
                    qExe.Parameters.AddWithValue("@MemberId", item.MemberId);
                    qExe.Parameters.AddWithValue("@DonateType", item.DonateType);
                    qExe.Parameters.AddWithValue("@DonateObjective", item.DonateObjective);
                    qExe.Parameters.AddWithValue("@DonatorRunno", item.DonatorRunno);
                    qExe.Parameters.AddWithValue("@DonatorId", item.DonatorId);
                    qExe.Parameters.AddWithValue("@DonateAmount", item.DonateAmount);
                    qExe.ExecuteNonQuery();
                    long returnid = qExe.LastInsertedId;
                    if (detail != null)
                    {
                        for (int i = 0; i <= detail.Count - 1; i++)
                        {
                            qDetailExe.Parameters.Clear();
                            qDetailExe.Parameters.AddWithValue("@DocumentRunno", returnid);
                            qDetailExe.Parameters.AddWithValue("@DetailRunno", i + 1);
                            qDetailExe.Parameters.AddWithValue("@Description", detail[i].Description);
                            qDetailExe.Parameters.AddWithValue("@Amount", detail[i].Amount);
                            qDetailExe.Parameters.AddWithValue("@Remark", detail[i].Remark);
                            qDetailExe.ExecuteNonQuery();
                        }
                    }
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

        [Route("DonateData/Edit")]
        [HttpPost]
        public IHttpActionResult DonateDataEdit([FromBody] DonateDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString =
                        @"UPDATE donatedata SET WriteAt = @WriteAt, DocumentDate = @DocumentDate, MemberRunno = @MemberRunno,
                                     MemberId = @MemberId, DonateType = @DonateType, DonateObjective = @DonateObjective, DonatorRunno = @DonatorRunno,
                                     DonatorId = @DonatorId, DonateAmount = @DonateAmount WHERE DocumentRunno = @DocumentRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    qExe.Parameters.AddWithValue("@documentrunno", item.DocumentRunno);
                    qExe.Parameters.AddWithValue("@WriteAt", item.WriteAt);
                    qExe.Parameters.AddWithValue("@DocumentDate", item.DocumentDate);
                    qExe.Parameters.AddWithValue("@MemberRunno", item.MemberRunno);
                    qExe.Parameters.AddWithValue("@MemberId", item.MemberId);
                    qExe.Parameters.AddWithValue("@DonateType", item.DonateType);
                    qExe.Parameters.AddWithValue("@DonateObjective", item.DonateObjective);
                    qExe.Parameters.AddWithValue("@DonatorRunno", item.DonatorRunno);
                    qExe.Parameters.AddWithValue("@DonatorId", item.DonatorId);
                    qExe.Parameters.AddWithValue("@DonateAmount", item.DonateAmount);
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

        [Route("DonateData/Delete/{id}")]
        [HttpPost]
        public IHttpActionResult DonateDataDelete(string id)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString = @"delete from donatedata where DocumentRunno = @DocumentRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    if (string.IsNullOrEmpty(id))
                        return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                    qExe.Parameters.AddWithValue("@DocumentRunno", id);
                    qExe.ExecuteNonQuery();
                    sqlString = "delete from donatedetaildata where DocumentRunno = @DocumentRunno";
                    qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    qExe.Parameters.AddWithValue("@DocumentRunno", id);
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

        [Route("DonateData/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult DonateDataListbyRunno(string id)
        {
            string AddressGenerate;
            DonateDataModel result = new DonateDataModel();
            result.DonateDetail = new List<DonateDetailDataModel>();
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string sqlString;
                    if (!string.IsNullOrEmpty(id))
                        sqlString = @"select doc.*, docs.DetailRunno, docs.Description, docs.Amount, docs.Remark, 
                                  mem.MemberRunno, mem.MemberId, mem.MemberPreName, mem.MemberName, mem.MemberSurname,
                                  mem.PositionNo, mem.BirthDate, mem.HouseNumber mHouseNumber, mem.Soi mSoi, mem.Road mRoad, 
                                  mem.Moo mMoo, mem.Building mBuilding, mem.Tambon mTambon, mem.Amphur mAmphur, 
                                  mem.Province mProvince, mem.Zipcode mZipcode, mem.Telephone mTelephone,
                                  don.DonatorRunno, don.DonatorId, don.DonatorPreName, don.DonatorName, don.DonatorSurName,
                                  don.DonatorCitizenId, don.DonatorRegisterNo, don.DonatorTaxId, don.HouseNumber dHouseNumber,
                                  don.Soi dSoi, don.Road dRoad, don.Moo dMoo, don.Building dBuilding, don.Tambon dTambon,
                                  don.Amphur dAmphur, don.Province dProvince, don.Zipcode dZipcode, don.Telephone dTelephone, par.positionName,
                                  (select sum(Amount) from donatedetaildata where DocumentRunno = doc.DocumentRunno) SumAmt,
                                  dotype.DonateTypeName,
                                  (select count(*) from donatedetaildata where DocumentRunno = doc.DocumentRunno) detailnum
                                  from donatedata doc left join donatedetaildata docs on doc.DocumentRunno = docs.DocumentRunno
                                  left join memberdata mem on mem.MemberRunno = doc.MemberRunno
                                  left join donatordata don on don.DonatorRunno = doc.DonatorRunno
                                  left join partyposition par on par.PositionNo = mem.PositionNO
                                  left join donatetype dotype on dotype.DonateTypeRunno = doc.DonateType
                                  where doc.DocumentRunno = @DocumentRunno
                                  order by DocumentRunno, DetailRunno";
                    else
                        return Json("Document Number is blank!");
                    string sqlDetail =
                        @"select docs.*, 
                          (select  DonateAssetType from donatetype where DonateTypeRunno = docs.DonateTypeRunno) AssetType
                          from donatedetaildata docs where DocumentRunno = @DocumentRunno order by DetailRunno";
                    MySqlCommand qDetail = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlDetail
                    };
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = sqlString
                    };
                    qExe.Parameters.AddWithValue("@DocumentRunno", id);
                    qDetail.Parameters.AddWithValue("@DocumentRunno", id);
                    MySqlDataReader detailReader = qDetail.ExecuteReader();
                    bool CashFlag = false;
                    bool AssetFlag = false;
                    bool BenefitFlag = false;
                    int iNumber = 1;
                    while (detailReader.Read())
                    {
                        DonateDetailDataModel detailRow = new DonateDetailDataModel();
                        detailRow.DocumentRunno = int.Parse(detailReader["DocumentRunno"].ToString());
                        detailRow.DetailRunno = int.Parse(detailReader["detailrunno"].ToString());
                        detailRow.Description = detailReader["description"].ToString();
                        detailRow.Amount = double.Parse(detailReader["amount"].ToString());
                        detailRow.Remark = detailReader["remark"].ToString();
                        detailRow.DetailNo = iNumber;
                        if (detailReader["AssetType"].ToString() == "1")
                            CashFlag = true;
                        if (detailReader["AssetType"].ToString() == "2")
                            AssetFlag = true;
                        if (detailReader["AssetType"].ToString() == "3")
                            BenefitFlag = true;
                        detailRow.Bill = detailReader["Bill"].ToString();
                        detailRow.BankChqueNo = detailReader["BankChqueNo"].ToString();
                        detailRow.Asset = detailReader["Asset"].ToString();
                        detailRow.Benefit = detailReader["Benefit"].ToString();
                        detailRow.BankCode = detailReader["BankCode"].ToString();
                        detailRow.DocRefBook = detailReader["DocRefBook"].ToString();
                        detailRow.DocRefNo = detailReader["DocRefNo"].ToString();
                        detailRow.DocType = detailReader["DocType"].ToString();
                        result.DonateDetail.Add(detailRow);
                        iNumber = iNumber + 1;
                    }
                    detailReader.Close();
                    MasterData masterData = GetMasterData();
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        result.WriteAt = dataReader["writeat"].ToString();
                        result.DocumentDate = Convert.ToDateTime(dataReader["DocumentDate"].ToString());
                        result.DocumentDateStr = Convert.ToDateTime(dataReader["DocumentDate"].ToString()).ToString("dd/MM/yyyy");
                        result.MemberRunno = int.Parse(dataReader["MemberRunno"].ToString());
                        result.DonateTypeRunno = int.Parse(dataReader["DonateType"].ToString());
                        result.DonateType = dataReader["DonateTypeName"].ToString();
                        result.DonateObjective = dataReader["DonateObjective"].ToString();
                        result.MemberId = dataReader["MemberId"].ToString();
                        result.MemberName = dataReader["memberPrename"].ToString() + dataReader["membername"].ToString() +
                                            "   " + dataReader["MemberSurName"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["birthdate"].ToString()))
                        {
                            result.MemberBirthdate = Convert.ToDateTime(dataReader["birthdate"].ToString());
                            result.MemberAge = DateTime.Now.Year - Convert.ToDateTime(dataReader["birthdate"].ToString()).Year;
                        }
                        else
                        {
                            result.MemberBirthdate = null;
                            result.MemberAge = null;
                        }
                        AddressGenerate = "";
                        if (!string.IsNullOrEmpty(dataReader["mhousenumber"].ToString()))
                            AddressGenerate = AddressGenerate + " บ้านเลขที่ " + dataReader["mhousenumber"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mmoo"].ToString()))
                            AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["mmoo"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mbuilding"].ToString()))
                            AddressGenerate = AddressGenerate + " อาคาร " + dataReader["mbuilding"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["msoi"].ToString()))
                            AddressGenerate = AddressGenerate + " ซอย " + dataReader["msoi"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mroad"].ToString()))
                            AddressGenerate = AddressGenerate + " ถนน " + dataReader["mroad"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mtambon"].ToString()))
                            AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["mtambon"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mamphur"].ToString()))
                            AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["mamphur"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mprovince"].ToString()))
                            AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["mprovince"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["mzipcode"].ToString()))
                            AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["mzipcode"].ToString();
                        result.MemberAddress = AddressGenerate.Trim();
                        result.MemberTelephone = dataReader["mtelephone"].ToString();
                        result.PositionNo = int.Parse(dataReader["positionNo"].ToString());
                        result.MemberPosition = dataReader["PositionName"].ToString();
                        result.DonatorRunno = int.Parse(dataReader["DonatorRunno"].ToString());
                        result.DonatorId = dataReader["DonatorId"].ToString();
                        result.DonatorName = dataReader["donatorPrename"].ToString() + dataReader["donatorname"].ToString() +
                                             "  " + dataReader["donatorSurname"].ToString();
                        result.DonatorCitizenId = dataReader["DonatorCitizenId"].ToString();
                        result.DonatorRegisterNO = dataReader["donatorregisterno"].ToString();
                        result.DonatorTaxID = dataReader["donatortaxid"].ToString();
                        AddressGenerate = "";
                        if (!string.IsNullOrEmpty(dataReader["dhousenumber"].ToString()))
                            AddressGenerate = " บ้านเลขที่ " + dataReader["dhousenumber"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dmoo"].ToString()))
                            AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["dmoo"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dbuilding"].ToString()))
                            AddressGenerate = AddressGenerate + " อาคาร " + dataReader["dbuilding"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dsoi"].ToString()))
                            AddressGenerate = AddressGenerate + " ซอย " + dataReader["dsoi"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["droad"].ToString()))
                            AddressGenerate = AddressGenerate + " ถนน " + dataReader["droad"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dtambon"].ToString()))
                            AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["dtambon"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["damphur"].ToString()))
                            AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["damphur"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dprovince"].ToString()))
                            AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["dprovince"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["dzipcode"].ToString()))
                            AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["dzipcode"].ToString();
                        result.DonatorAddress = AddressGenerate.Trim();
                        result.DonatorTelephone = dataReader["dtelephone"].ToString();

                        result.PartyName = masterData.PartyName;
                        result.PartyTel = masterData.Telephone;
                        result.PartyTaxID = "เลขประจำตัวผู้เสียภาษี " + masterData.TaxID;
                        result.PartyAddr1 = GetAddrRow1(masterData);
                        result.PartyAddr2 = GetAddrRow2(masterData);
                        result.MemberHouseNumber = dataReader["mhousenumber"].ToString();
                        result.MemberSoi = dataReader["msoi"].ToString();
                        result.MemberRoad = dataReader["mroad"].ToString();
                        result.MemberMoo = dataReader["mmoo"].ToString();
                        result.MemberBuilding = dataReader["mbuilding"].ToString();
                        result.MemberTambon = dataReader["mtambon"].ToString();
                        result.MemberAmphur = dataReader["mamphur"].ToString();
                        result.MemberProvince = dataReader["mprovince"].ToString();
                        result.MemberZipCode = dataReader["mzipcode"].ToString();
                        result.DonatorHouseNumber = dataReader["dhousenumber"].ToString();
                        result.DonatorSoi = dataReader["dsoi"].ToString();
                        result.DonatorRoad = dataReader["droad"].ToString();
                        result.DonatorMoo = dataReader["dmoo"].ToString();
                        result.DonatorBuilding = dataReader["dbuilding"].ToString();
                        result.DonatorTambon = dataReader["dtambon"].ToString();
                        result.DonatorAmphur = dataReader["damphur"].ToString();
                        result.DonatorProvince = dataReader["dprovince"].ToString();
                        result.DonatorZipCode = dataReader["dzipcode"].ToString();
                        if (CashFlag)
                            result.CashFlag = "Y";
                        else
                            result.CashFlag = "N";
                        if (AssetFlag)
                            result.AssetFlag = "Y";
                        else
                            result.AssetFlag = "N";
                        if (BenefitFlag)
                            result.BenefitFlag = "Y";
                        else
                            result.BenefitFlag = "N";
                        // if (!string.IsNullOrEmpty(dataReader["DonateAmount"].ToString()))
                        if (!string.IsNullOrEmpty(dataReader["SumAmt"].ToString()))
                            result.DonateAmount = double.Parse(dataReader["SumAmt"].ToString());
                        else
                            result.DonateAmount = 0;
                        result.DonateDetailCount = int.Parse(dataReader["detailnum"].ToString());
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

        [Route("DonateData/ListAll")]
        [HttpGet]
        public IHttpActionResult DonateDataList()
        {
            DateTime Tmp = new DateTime();
            List<DonateDataModel> result = new List<DonateDataModel>();
            DonateDataModel row = null;
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"select doc.*, docs.DetailRunno, docs.Description, docs.Amount, docs.Remark, 
                                  mem.MemberRunno, mem.MemberId, mem.MemberPreName, mem.MemberName, mem.MemberSurname,
                                  mem.PositionNo, mem.BirthDate, mem.HouseNumber mHouseNumber, mem.Soi mSoi, mem.Road mRoad, 
                                  mem.Moo mMoo, mem.Building mBuilding, mem.Tambon mTambon, mem.Amphur mAmphur, 
                                  mem.Province mProvince, mem.Zipcode mZipcode, mem.Telephone mTelephone,
                                  don.DonatorRunno, don.DonatorId, don.DonatorPreName, don.DonatorName, don.DonatorSurName,
                                  don.DonatorCitizenId, don.DonatorRegisterNo, don.DonatorTaxId, don.HouseNumber dHouseNumber,
                                  don.Soi dSoi, don.Road dRoad, don.Moo dMoo, don.Building dBuilding, don.Tambon dTambon,
                                  don.Amphur dAmphur, don.Province dProvince, don.Zipcode dZipcode, don.Telephone dTelephone, par.positionName,
                                  (select sum(Amount) from donatedetaildata where DocumentRunno = doc.DocumentRunno) SumAmt,
                                  dotype.DonateTypeName,
                                  (select count(*) from donatedetaildata where DocumentRunno = doc.DocumentRunno) detailnum
                                  from donatedata doc left join donatedetaildata docs on doc.DocumentRunno = docs.DocumentRunno
                                  left join memberdata mem on mem.MemberRunno = doc.MemberRunno
                                  left join donatordata don on don.DonatorRunno = doc.DonatorRunno
                                  left join partyposition par on par.PositionNo = mem.PositionNO
                                  left join donatetype dotype on dotype.DonateTypeRunno = doc.DonateType
                                  where 1 = 1 order by DocumentRunno, DetailRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    int RunnoBreak = 0;
                    int DetailRunnoBreak = 0;
                    string AddressGenerate;
                    while (dataReader.Read())
                    {
                        if (RunnoBreak != int.Parse(dataReader["DocumentRunno"].ToString()))
                        {
                            if (row != null)
                                result.Add(row);
                            row = new DonateDataModel();
                            row.DonateDetail = new List<DonateDetailDataModel>();
                            RunnoBreak = int.Parse(dataReader["DocumentRunno"].ToString());
                            DetailRunnoBreak = 0;

                            row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                            row.WriteAt = dataReader["writeat"].ToString();
                            Tmp = Convert.ToDateTime(dataReader["DocumentDate"].ToString());
                            row.DocumentDate = Tmp;
                            row.DocumentDateStr = Tmp.ToString("dd/MM/yyyy");
                            //row.DocumentDateStr = DateTime.Parse(dataReader["DocumentDate"].ToString()).ToString("dd/MM/yyyy");                            
                            row.MemberRunno = int.Parse(dataReader["MemberRunno"].ToString());
                            row.DonateTypeRunno = int.Parse(dataReader["DonateType"].ToString());
                            row.DonateType = dataReader["DonateTypeName"].ToString();
                            row.DonateObjective = dataReader["DonateObjective"].ToString();
                            row.MemberId = dataReader["MemberId"].ToString();
                            row.MemberName = dataReader["memberPrename"].ToString() + dataReader["membername"].ToString() +
                                             "   " + dataReader["MemberSurName"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["birthdate"].ToString()))
                                row.MemberBirthdate = Convert.ToDateTime(dataReader["birthdate"].ToString());
                            else
                                row.MemberBirthdate = null;
                            AddressGenerate = "";
                            if (!string.IsNullOrEmpty(dataReader["mhousenumber"].ToString()))
                                AddressGenerate = " บ้านเลขที่ " + dataReader["mhousenumber"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mmoo"].ToString()))
                                AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["mmoo"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mbuilding"].ToString()))
                                AddressGenerate = AddressGenerate + " อาคาร " + dataReader["mbuilding"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["msoi"].ToString()))
                                AddressGenerate = AddressGenerate + " ซอย " + dataReader["msoi"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mroad"].ToString()))
                                AddressGenerate = AddressGenerate + " ถนน " + dataReader["mroad"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mtambon"].ToString()))
                                AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["mtambon"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mamphur"].ToString()))
                                AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["mamphur"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mprovince"].ToString()))
                                AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["mprovince"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mzipcode"].ToString()))
                                AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["mzipcode"].ToString();
                            row.MemberAddress = AddressGenerate.Trim();
                            row.MemberTelephone = dataReader["mtelephone"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["positionNo"].ToString()))
                            {
                                row.PositionNo = int.Parse(dataReader["positionNo"].ToString());
                                row.MemberPosition = dataReader["PositionName"].ToString();
                            }
                            row.DonatorRunno = int.Parse(dataReader["DonatorRunno"].ToString());
                            row.DonatorId = dataReader["DonatorId"].ToString();
                            row.DonatorName = dataReader["donatorPrename"].ToString() + dataReader["donatorname"].ToString() +
                                              "  " + dataReader["donatorSurname"].ToString();
                            row.DonatorRegisterNO = dataReader["donatorregisterno"].ToString();
                            row.DonatorTaxID = dataReader["donatortaxid"].ToString();
                            AddressGenerate = "";
                            if (!string.IsNullOrEmpty(dataReader["dhousenumber"].ToString()))
                                AddressGenerate = " บ้านเลขที่ " + dataReader["dhousenumber"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dmoo"].ToString()))
                                AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["dmoo"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dbuilding"].ToString()))
                                AddressGenerate = AddressGenerate + " อาคาร " + dataReader["dbuilding"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dsoi"].ToString()))
                                AddressGenerate = AddressGenerate + " ซอย " + dataReader["dsoi"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["droad"].ToString()))
                                AddressGenerate = AddressGenerate + " ถนน " + dataReader["droad"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dtambon"].ToString()))
                                AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["dtambon"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["damphur"].ToString()))
                                AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["damphur"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dprovince"].ToString()))
                                AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["dprovince"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dzipcode"].ToString()))
                                AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["dzipcode"].ToString();
                            row.DonatorAddress = AddressGenerate.Trim();
                            row.DonatorTelephone = dataReader["dtelephone"].ToString();

                            row.MemberHouseNumber = dataReader["mhousenumber"].ToString();
                            row.MemberSoi = dataReader["msoi"].ToString();
                            row.MemberRoad = dataReader["mroad"].ToString();
                            row.MemberMoo = dataReader["mmoo"].ToString();
                            row.MemberBuilding = dataReader["mbuilding"].ToString();
                            row.MemberTambon = dataReader["mtambon"].ToString();
                            row.MemberAmphur = dataReader["mamphur"].ToString();
                            row.MemberProvince = dataReader["mprovince"].ToString();
                            row.MemberZipCode = dataReader["mzipcode"].ToString();
                            row.DonatorHouseNumber = dataReader["dhousenumber"].ToString();
                            row.DonatorSoi = dataReader["dsoi"].ToString();
                            row.DonatorRoad = dataReader["droad"].ToString();
                            row.DonatorMoo = dataReader["dmoo"].ToString();
                            row.DonatorBuilding = dataReader["dbuilding"].ToString();
                            row.DonatorTambon = dataReader["dtambon"].ToString();
                            row.DonatorAmphur = dataReader["damphur"].ToString();
                            row.DonatorProvince = dataReader["dprovince"].ToString();
                            row.DonatorZipCode = dataReader["dzipcode"].ToString();

                            if (!string.IsNullOrEmpty(dataReader["SumAmt"].ToString()))
                                // if (!string.IsNullOrEmpty(dataReader["DonateAmount"].ToString()))
                                row.DonateAmount = double.Parse(dataReader["SumAmt"].ToString());
                            else
                                row.DonateAmount = 0;
                            row.DonateDetailCount = int.Parse(dataReader["detailnum"].ToString());
                        }
                        if (!string.IsNullOrEmpty(dataReader["DetailRunno"].ToString()))
                        {
                            if (DetailRunnoBreak != int.Parse(dataReader["DetailRunno"].ToString()))
                            {
                                DonateDetailDataModel detailRow = new DonateDetailDataModel();
                                detailRow.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                                detailRow.DetailRunno = int.Parse(dataReader["detailrunno"].ToString());
                                detailRow.Description = dataReader["description"].ToString();
                                detailRow.Amount = double.Parse(dataReader["amount"].ToString());
                                detailRow.Remark = dataReader["remark"].ToString();
                                row.DonateDetail.Add(detailRow);
                                DetailRunnoBreak = int.Parse(dataReader["DetailRunno"].ToString());
                            }
                        }
                    }
                    if (row != null)
                        result.Add(row);
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
        [Route("DonateData/ListAllLast/10")]
        [HttpGet]
        public IHttpActionResult DonateDataListOrderbyDESC()
        {
            DateTime Tmp = new DateTime();
            List<DonateDataModel> result = new List<DonateDataModel>();
            DonateDataModel row = null;
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLString = @"select doc.*, docs.DetailRunno, docs.Description, docs.Amount, docs.Remark, 
                                  mem.MemberRunno, mem.MemberId, mem.MemberPreName, mem.MemberName, mem.MemberSurname,
                                  mem.PositionNo, mem.BirthDate, mem.HouseNumber mHouseNumber, mem.Soi mSoi, mem.Road mRoad, 
                                  mem.Moo mMoo, mem.Building mBuilding, mem.Tambon mTambon, mem.Amphur mAmphur, 
                                  mem.Province mProvince, mem.Zipcode mZipcode, mem.Telephone mTelephone,
                                  don.DonatorRunno, don.DonatorId, don.DonatorPreName, don.DonatorName, don.DonatorSurName,
                                  don.DonatorCitizenId, don.DonatorRegisterNo, don.DonatorTaxId, don.HouseNumber dHouseNumber,
                                  don.Soi dSoi, don.Road dRoad, don.Moo dMoo, don.Building dBuilding, don.Tambon dTambon,
                                  don.Amphur dAmphur, don.Province dProvince, don.Zipcode dZipcode, don.Telephone dTelephone, par.positionName,
                                  (select sum(Amount) from donatedetaildata where DocumentRunno = doc.DocumentRunno) SumAmt,
                                  dotype.DonateTypeName,
                                  (select count(*) from donatedetaildata where DocumentRunno = doc.DocumentRunno) detailnum
                                  from donatedata doc left join donatedetaildata docs on doc.DocumentRunno = docs.DocumentRunno
                                  left join memberdata mem on mem.MemberRunno = doc.MemberRunno
                                  left join donatordata don on don.DonatorRunno = doc.DonatorRunno
                                  left join partyposition par on par.PositionNo = mem.PositionNO
                                  left join donatetype dotype on dotype.DonateTypeRunno = doc.DonateType
                                  where 1 = 1 order by doc.DocumentDate Desc, doc.Last_Update Desc, doc.DocumentRunno, docs.DetailRunno";
                    MySqlCommand qExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLString
                    };
                    MySqlDataReader dataReader = qExe.ExecuteReader();
                    int RunnoBreak = 0;
                    int DetailRunnoBreak = 0;
                    string AddressGenerate;
                    while (dataReader.Read())
                    {
                        if (RunnoBreak != int.Parse(dataReader["DocumentRunno"].ToString()))
                        {
                            if (row != null)
                                result.Add(row);
                            row = new DonateDataModel();
                            row.DonateDetail = new List<DonateDetailDataModel>();
                            RunnoBreak = int.Parse(dataReader["DocumentRunno"].ToString());
                            DetailRunnoBreak = 0;

                            row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                            row.WriteAt = dataReader["writeat"].ToString();
                            Tmp = Convert.ToDateTime(dataReader["DocumentDate"].ToString());
                            row.DocumentDate = Tmp;
                            row.DocumentDateStr = Tmp.ToString("dd/MM/yyyy");
                            //row.DocumentDateStr = DateTime.Parse(dataReader["DocumentDate"].ToString()).ToString("dd/MM/yyyy");                            
                            row.MemberRunno = int.Parse(dataReader["MemberRunno"].ToString());
                            row.DonateTypeRunno = int.Parse(dataReader["DonateType"].ToString());
                            row.DonateType = dataReader["DonateTypeName"].ToString();
                            row.DonateObjective = dataReader["DonateObjective"].ToString();
                            row.MemberId = dataReader["MemberId"].ToString();
                            row.MemberName = dataReader["memberPrename"].ToString() + dataReader["membername"].ToString() +
                                             "   " + dataReader["MemberSurName"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["birthdate"].ToString()))
                                row.MemberBirthdate = Convert.ToDateTime(dataReader["birthdate"].ToString());
                            else
                                row.MemberBirthdate = null;
                            AddressGenerate = "";
                            if (!string.IsNullOrEmpty(dataReader["mhousenumber"].ToString()))
                                AddressGenerate = " บ้านเลขที่ " + dataReader["mhousenumber"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mmoo"].ToString()))
                                AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["mmoo"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mbuilding"].ToString()))
                                AddressGenerate = AddressGenerate + " อาคาร " + dataReader["mbuilding"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["msoi"].ToString()))
                                AddressGenerate = AddressGenerate + " ซอย " + dataReader["msoi"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mroad"].ToString()))
                                AddressGenerate = AddressGenerate + " ถนน " + dataReader["mroad"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mtambon"].ToString()))
                                AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["mtambon"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mamphur"].ToString()))
                                AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["mamphur"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mprovince"].ToString()))
                                AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["mprovince"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["mzipcode"].ToString()))
                                AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["mzipcode"].ToString();
                            row.MemberAddress = AddressGenerate.Trim();
                            row.MemberTelephone = dataReader["mtelephone"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["positionNo"].ToString()))
                            {
                                row.PositionNo = int.Parse(dataReader["positionNo"].ToString());
                                row.MemberPosition = dataReader["PositionName"].ToString();
                            }
                            row.DonatorRunno = int.Parse(dataReader["DonatorRunno"].ToString());
                            row.DonatorId = dataReader["DonatorId"].ToString();
                            row.DonatorName = dataReader["donatorPrename"].ToString() + dataReader["donatorname"].ToString() +
                                              "  " + dataReader["donatorSurname"].ToString();
                            row.DonatorRegisterNO = dataReader["donatorregisterno"].ToString();
                            row.DonatorTaxID = dataReader["donatortaxid"].ToString();
                            AddressGenerate = "";
                            if (!string.IsNullOrEmpty(dataReader["dhousenumber"].ToString()))
                                AddressGenerate = " บ้านเลขที่ " + dataReader["dhousenumber"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dmoo"].ToString()))
                                AddressGenerate = AddressGenerate + " หมู่ที่ " + dataReader["dmoo"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dbuilding"].ToString()))
                                AddressGenerate = AddressGenerate + " อาคาร " + dataReader["dbuilding"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dsoi"].ToString()))
                                AddressGenerate = AddressGenerate + " ซอย " + dataReader["dsoi"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["droad"].ToString()))
                                AddressGenerate = AddressGenerate + " ถนน " + dataReader["droad"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dtambon"].ToString()))
                                AddressGenerate = AddressGenerate + " ตำบล/แขวง " + dataReader["dtambon"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["damphur"].ToString()))
                                AddressGenerate = AddressGenerate + " อำเภอ/เขต " + dataReader["damphur"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dprovince"].ToString()))
                                AddressGenerate = AddressGenerate + " จังหวัด " + dataReader["dprovince"].ToString();
                            if (!string.IsNullOrEmpty(dataReader["dzipcode"].ToString()))
                                AddressGenerate = AddressGenerate + " รหัสไปรษณีย์ " + dataReader["dzipcode"].ToString();
                            row.DonatorAddress = AddressGenerate.Trim();
                            row.DonatorTelephone = dataReader["dtelephone"].ToString();
                            row.MemberHouseNumber = dataReader["mhousenumber"].ToString();

                            row.MemberSoi = dataReader["msoi"].ToString();
                            row.MemberRoad = dataReader["mroad"].ToString();
                            row.MemberMoo = dataReader["mmoo"].ToString();
                            row.MemberBuilding = dataReader["mbuilding"].ToString();
                            row.MemberTambon = dataReader["mtambon"].ToString();
                            row.MemberAmphur = dataReader["mamphur"].ToString();
                            row.MemberProvince = dataReader["mprovince"].ToString();
                            row.MemberZipCode = dataReader["mzipcode"].ToString();
                            row.DonatorHouseNumber = dataReader["dhousenumber"].ToString();
                            row.DonatorSoi = dataReader["dsoi"].ToString();
                            row.DonatorRoad = dataReader["droad"].ToString();
                            row.DonatorMoo = dataReader["dmoo"].ToString();
                            row.DonatorBuilding = dataReader["dbuilding"].ToString();
                            row.DonatorTambon = dataReader["dtambon"].ToString();
                            row.DonatorAmphur = dataReader["damphur"].ToString();
                            row.DonatorProvince = dataReader["dprovince"].ToString();
                            row.DonatorZipCode = dataReader["dzipcode"].ToString();

                            if (!string.IsNullOrEmpty(dataReader["SumAmt"].ToString()))
                                // if (!string.IsNullOrEmpty(dataReader["DonateAmount"].ToString()))
                                row.DonateAmount = double.Parse(dataReader["SumAmt"].ToString());
                            else
                                row.DonateAmount = 0;
                            row.DonateDetailCount = int.Parse(dataReader["detailnum"].ToString());
                        }
                        if (!string.IsNullOrEmpty(dataReader["DetailRunno"].ToString()))
                        {
                            if (DetailRunnoBreak != int.Parse(dataReader["DetailRunno"].ToString()))
                            {
                                DonateDetailDataModel detailRow = new DonateDetailDataModel();
                                detailRow.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                                detailRow.DetailRunno = int.Parse(dataReader["detailrunno"].ToString());
                                detailRow.Description = dataReader["description"].ToString();
                                detailRow.Amount = double.Parse(dataReader["amount"].ToString());
                                detailRow.Remark = dataReader["remark"].ToString();
                                row.DonateDetail.Add(detailRow);
                                DetailRunnoBreak = int.Parse(dataReader["DetailRunno"].ToString());
                            }
                        }
                    }
                    if (row != null)
                        result.Add(row);
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

        [Route("DonateDetailData/Add")]
        [HttpPost]
        public IHttpActionResult AddDonateDetailData([FromBody] DonateDetailDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    int typeHardcode = 0;
                    string
                    SQLDetailString =
                        @"INSERT INTO donatedetaildata(DocumentRunno, Description, Amount, Remark, DonateTypeRunno,
                                        Bill, BankChqueNo, Asset, Benefit, BankCode)
                                        VALUES(@DocumentRunno, @Description, @Amount, @Remark, @DonateTypeRunno,
                                        @Bill, @BankChqueNo, @Asset, @Benefit, @BankCode) ";

                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailString
                    };
                    qDetailExe.Parameters.Clear();
                    qDetailExe.Parameters.AddWithValue("@DocumentRunno", item.DocumentRunno);

                    qDetailExe.Parameters.AddWithValue("@Amount", item.Amount);
                    qDetailExe.Parameters.AddWithValue("@Remark", item.Remark);
                    if (string.IsNullOrEmpty(item.Bill) &&
                        string.IsNullOrEmpty(item.BankChqueNo) &&
                        string.IsNullOrEmpty(item.Asset) &&
                        string.IsNullOrEmpty(item.Benefit))
                    {
                        typeHardcode = 1;
                        qDetailExe.Parameters.AddWithValue("@Description", "เงินสด");
                    }
                    else if (!string.IsNullOrEmpty(item.Bill))
                    {
                        typeHardcode = 5;
                        qDetailExe.Parameters.AddWithValue("@Description", "ตั๋วเงินเลขที่ " + item.Bill);
                    }
                    else if (!string.IsNullOrEmpty(item.BankChqueNo))
                    {
                        typeHardcode = 6;
                        qDetailExe.Parameters.AddWithValue("@Description", "เช็คธนาคารเลขที่ " + item.BankChqueNo);
                    }
                    else if (!string.IsNullOrEmpty(item.Asset))
                    {
                        typeHardcode = 2;
                        qDetailExe.Parameters.AddWithValue("@Description", item.Asset);
                    }
                    else if (!string.IsNullOrEmpty(item.Benefit))
                    {
                        typeHardcode = 3;
                        qDetailExe.Parameters.AddWithValue("@Description", item.Benefit);
                    }
                    qDetailExe.Parameters.AddWithValue("@DonateTypeRunno", typeHardcode);
                    qDetailExe.Parameters.AddWithValue("@Bill", item.Bill);
                    qDetailExe.Parameters.AddWithValue("@BankChqueNo", item.BankChqueNo);
                    qDetailExe.Parameters.AddWithValue("@Asset", item.Asset);
                    qDetailExe.Parameters.AddWithValue("@Benefit", item.Benefit);
                    qDetailExe.Parameters.AddWithValue("@BankCode", item.BankCode);
                    qDetailExe.ExecuteNonQuery();
                    long returnid = qDetailExe.LastInsertedId;
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

        [Route("DonateDetailData/Edit")]
        [HttpPost]
        public IHttpActionResult EditDonateDetailData([FromBody] DonateDetailDataModel item)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    int typeHardcode = 0;
                    string
                    SQLDetailString = @"UPDATE donatedetaildata SET DocumentRunno = @DocumentRunno,
                                        Description = @Description, Amount = @Amount, Remark = @Remark,
                                        DonateTypeRunno = @DonateTypeRunno, Bill = @Bill, BankChqueNo = @BankChqueNo,
                                        Asset = @Asset, Benefit = @Benefit, BankCode = @BankCode WHERE DetailRunno = @DetailRunno";
                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailString
                    };
                    qDetailExe.Parameters.Clear();
                    qDetailExe.Parameters.AddWithValue("@DocumentRunno", item.DocumentRunno);
                    qDetailExe.Parameters.AddWithValue("@DetailRunno", item.DetailRunno);
                    qDetailExe.Parameters.AddWithValue("@Amount", item.Amount);
                    qDetailExe.Parameters.AddWithValue("@Remark", item.Remark);

                    if (string.IsNullOrEmpty(item.Bill) &&
                        string.IsNullOrEmpty(item.BankChqueNo) &&
                        string.IsNullOrEmpty(item.Asset) &&
                        string.IsNullOrEmpty(item.Benefit))
                    {
                        typeHardcode = 1;
                        qDetailExe.Parameters.AddWithValue("@Description", "เงินสด");
                    }
                    else if (!string.IsNullOrEmpty(item.Bill))
                    {
                        typeHardcode = 5;
                        qDetailExe.Parameters.AddWithValue("@Description", "ตั๋วเงินเลขที่ " + item.Bill);
                    }
                    else if (!string.IsNullOrEmpty(item.BankChqueNo))
                    {
                        typeHardcode = 6;
                        qDetailExe.Parameters.AddWithValue("@Description", "เช็คธนาคารเลขที่ " + item.BankChqueNo);
                    }
                    else if (!string.IsNullOrEmpty(item.Asset))
                    {
                        typeHardcode = 2;
                        qDetailExe.Parameters.AddWithValue("@Description", item.Asset);
                    }
                    else if (!string.IsNullOrEmpty(item.Benefit))
                    {
                        typeHardcode = 3;
                        qDetailExe.Parameters.AddWithValue("@Description", item.Benefit);
                    }
                    qDetailExe.Parameters.AddWithValue("@DonateTypeRunno", typeHardcode);
                    qDetailExe.Parameters.AddWithValue("@Bill", item.Bill);
                    qDetailExe.Parameters.AddWithValue("@BankChqueNo", item.BankChqueNo);
                    qDetailExe.Parameters.AddWithValue("@Asset", item.Asset);
                    qDetailExe.Parameters.AddWithValue("@Benefit", item.Benefit);
                    qDetailExe.Parameters.AddWithValue("@BankCode", item.BankCode);

                    qDetailExe.ExecuteNonQuery();

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

        [Route("DonateDetailData/Delete/{ID}")]
        [HttpPost]
        public IHttpActionResult DeleteDonateDetailData(string ID)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    string SQLDetailDeleteString = "DELETE FROM donatedetaildata WHERE DetailRunno = @DetailRunno";
                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailDeleteString
                    };
                    if (string.IsNullOrEmpty(ID) && string.IsNullOrEmpty(ID))
                        return Json(new ResultDataModel { success = false, errorMessage = "Key is null!", returnRunno = "" });
                    qDetailExe.Parameters.AddWithValue("@DetailRunno", ID);
                    qDetailExe.ExecuteNonQuery();
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

        [Route("DonateDetailData/ListAll/{DocumentRunno}")]
        [HttpGet]
        public IHttpActionResult GetDetailData(string DocumentRunno)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    List<DonateDetailDataModel> result = new List<DonateDetailDataModel>();
                    string SQLDetailDeleteString = "select * from donatedetaildata WHERE DocumentRunno = @DocumentRunno";
                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailDeleteString
                    };
                    qDetailExe.Parameters.AddWithValue("@DocumentRunno", DocumentRunno);
                    MySqlDataReader dataReader = qDetailExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        DonateDetailDataModel row = new DonateDetailDataModel();
                        row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        row.DetailRunno = int.Parse(dataReader["detailrunno"].ToString());
                        row.Description = dataReader["description"].ToString();
                        row.Amount = double.Parse(dataReader["amount"].ToString());
                        row.Remark = dataReader["remark"].ToString();

                        row.Asset = dataReader["Asset"].ToString();
                        row.BankChqueNo = dataReader["BankChqueNo"].ToString();
                        row.BankCode = dataReader["BankCode"].ToString();
                        row.Asset = dataReader["Asset"].ToString();
                        row.Benefit = dataReader["Benefit"].ToString();
                        row.Bill = dataReader["Bill"].ToString();
                        row.DocRefBook = dataReader["DocRefBook"].ToString();
                        row.DocRefNo = dataReader["DocRefNo"].ToString();
                        row.DocType = dataReader["DocType"].ToString();
                        result.Add(row);
                    }
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
        [Route("DonateDetailData/GetByID/{ID}")]
        [HttpGet]
        public IHttpActionResult GetDetailDatabyID(string ID)
        {
            DBConnector.DBConnector conn = new DBConnector.DBConnector();
            if (conn.OpenConnection())
            {
                try
                {
                    DonateDetailDataModel result = new DonateDetailDataModel();
                    string SQLDetailDeleteString = "select * from donatedetaildata WHERE DetailRunno = @DetailRunno";
                    MySqlCommand qDetailExe = new MySqlCommand
                    {
                        Connection = conn.connection,
                        CommandText = SQLDetailDeleteString
                    };
                    qDetailExe.Parameters.AddWithValue("@DetailRunno", ID);
                    MySqlDataReader dataReader = qDetailExe.ExecuteReader();
                    while (dataReader.Read())
                    {
                        DonateDetailDataModel row = new DonateDetailDataModel();
                        row.DocumentRunno = int.Parse(dataReader["DocumentRunno"].ToString());
                        row.DetailRunno = int.Parse(dataReader["detailrunno"].ToString());
                        row.Description = dataReader["description"].ToString();
                        row.Amount = double.Parse(dataReader["amount"].ToString());
                        row.Remark = dataReader["remark"].ToString();

                        row.Asset = dataReader["Asset"].ToString();
                        row.BankChqueNo = dataReader["BankChqueNo"].ToString();
                        row.BankCode = dataReader["BankCode"].ToString();
                        row.Benefit = dataReader["Benefit"].ToString();
                        row.Asset = dataReader["Asset"].ToString();
                        row.Bill = dataReader["Bill"].ToString();
                        row.DocRefBook = dataReader["DocRefBook"].ToString();
                        row.DocRefNo = dataReader["DocRefNo"].ToString();
                        row.DocType = dataReader["DocType"].ToString();
                        result = row;
                    }
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
    }
}
