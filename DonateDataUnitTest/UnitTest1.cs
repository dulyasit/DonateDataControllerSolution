using System;
using System.Collections.Generic;
using DonateDataController.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtilityControllers.Models;
using UtilityLib;

namespace DonateDataUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ทดสอบบันทึกข้อมูลใหม่()
        {
            /*
            List<DonateDetailDataModel> detailList = new List<DonateDetailDataModel>
            {
                new DonateDetailDataModel
                {
                    Description = "บริจาคด้วยเงินสด",
                    Amount = 5000,
                    Remark = "หมายเหตุไม่มี"
                }
            };
            */
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            DonateDataModel itemData = new DonateDataModel
            {
                WriteAt = "บ้าน",
                DocumentDate = DateTime.Now.Date,
                DonateType = "เงินสด",
                DonateObjective = "เพื่อบำรุงพรรค",
                MemberRunno = 1,
                MemberId = "",
                DonatorRunno = 1,
                DonatorId = "",
                DonateAmount = 5000,
                DonateDetail = null
            };
            var result = service.DonateDataAdd(itemData);
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void ทดสอบแก้ไขเอกสาร()
        {
            List<DonateDetailDataModel> detailList = new List<DonateDetailDataModel>
            {
                new DonateDetailDataModel
                {
                    Description = "บริจาคด้วยเงินสด",
                    Amount = 500,
                    Remark = "โอนเงินเข้าธนาคาร"
                },
                new DonateDetailDataModel
                {
                    Description = "เครื่องทำกาแฟ",
                    Amount = 1500,
                    Remark = ""
                }
            };
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            DonateDataModel itemData = new DonateDataModel
            {
                DocumentRunno = 5,
                WriteAt = "บ้าน",
                DocumentDate = DateTime.Now.Date,
                DonateType = "เงินสด",
                DonateObjective = "เพื่อบำรุงพรรค",
                MemberRunno = 1,
                MemberId = "",
                DonatorRunno = 1,
                DonatorId = "",
                DonateAmount = 5000,
                DonateDetail = detailList
            };
            var result = service.DonateDataEdit(itemData);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบลบเอกสาร()
        {
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            // DonateDataModel item = new DonateDataModel();
            // item.DocumentRunno = "2";
            var result = service.DonateDataDelete("6");
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ทดสอบการดึงข้อมูลตามrunno()
        {
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            DonateDataModel item = new DonateDataModel();
            string DocumentRunno = "900";
            var result = service.DonateDataListbyRunno(DocumentRunno);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบดึงเอกสารทั้งหมด()
        {
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.DonateDataList();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบบันทึกเฉพาะDetailของเอกสาร()
        {
            DonateDetailDataModel detailitem = new DonateDetailDataModel
            {
                DocumentRunno = 5,
                Description = "เพิ่มรายการโดยที่",
                Amount = 5000,
                Remark = "เลขระหว่างกลางหาย"
            };
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.AddDonateDetailData(detailitem);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบแก้ไขDetailเอกสาร()
        {
            DonateDetailDataModel detailitem = new DonateDetailDataModel
            {
                DocumentRunno = 5,
                DetailRunno = 2,
                Description = "เครื่องชงกาแฟสำเร็จ",
                Amount = 20000,
                Remark = "สำหรับใช้งานใน Office"
            };
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.EditDonateDetailData(detailitem);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบลบข้อมูลDetail()
        {
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.DeleteDonateDetailData("3");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ดึงข้อมูลDetail()
        {
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.GetDetailData("5");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบเพิ่มข้อมูลผู้บริจาค()
        {
            DonatorController service = new DonatorController();
            DonatorData item = new DonatorData();
            item.DonatorId = "0003";
            item.DonatorPreName = "นาย";
            item.DonatorName = "ทดสอบลบ";
            item.DonatorSurName = "ลบได้เลย";
            item.DonatorCitizenId = "4564567890123";
            item.DonatorRegisterNo = "";
            item.DonatorTaxId = "";
            item.HouseNumber = "98";
            item.Soi = "ซอย";
            item.Road = "";
            item.Moo = "5";
            item.Building = "";
            item.Tambon = "";
            item.Amphur = "";
            item.Province = "";
            item.ZipCode = "10400";
            item.Telephone = "";
            var result = service.AddDonatorData(item);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ทดสอบแก้ไขข้อมูล()
        {
            DonatorController service = new DonatorController();
            DonatorData item = new DonatorData();
            item.DonatorRunno = 2;
            item.DonatorId = "0002";
            item.DonatorPreName = "นาย";
            item.DonatorName = "แก้ไขข้อมูล";
            item.DonatorSurName = "ทดสอบมา";
            item.DonatorCitizenId = "4";
            item.DonatorRegisterNo = "";
            item.DonatorTaxId = "";
            item.HouseNumber = "8";
            item.Soi = "3321";
            item.Road = "";
            item.Moo = "9";
            item.Building = "";
            item.Tambon = "";
            item.Amphur = "";
            item.Province = "";
            item.ZipCode = "99999";
            item.Telephone = "";
            var result = service.EditDonatorData(item);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ทดสอบลบข้อมูลผู้บริจาค()
        {
            DonatorController service = new DonatorController();
            DonatorData item = new DonatorData();
            item.DonatorRunno = 2;
            var result = service.DeleteDonatorData(item.DonatorRunno.ToString());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบGenเอกสาร11()
        {
            GenDocumentController service = new GenDocumentController();
            var result = service.GenDocumentPK11("5");
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ทดสอบGenเอกสาร12()
        {
            GenDocumentController service = new GenDocumentController();
            var result = service.GenDocumentPK12("5");
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ทดสอบดึงเอกสารPK11()
        {
            GenDocumentController service = new GenDocumentController();
            var result = service.GetDocPK11byID("2");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบบันทึกเอกสารPK9()
        {
            PK9DataController service = new PK9DataController();
            ReceiptDataModel item = new ReceiptDataModel();
            UtilLibs utilLibs = new UtilLibs();
            DocumentNumber docNum = utilLibs.GetDocumentNo("pk9");
            item.DocumentBookNumber = docNum.BookNo;
            item.DocumentNumber = docNum.DocumentNo;
            item.DocumentDate = DateTime.Now;
            item.PayerType = "2";
            item.PayerRunno = 1;
            item.AsReceiptTo = "ค่าจัดกิจกรรมระดมทุน";
            item.AsReceiptToRemark = "";
            item.ReceiptAmount = 1750;
            item.ReceiptPayType = "";
            item.ReceiptDate = DateTime.Now;
            item.ReceiptBank = "";
            item.ReceiptBillNumber = "";
            item.ReceiptChqBank = "";
            item.ReceiptChqNumber = "";
            item.ReceiptOther = "";
            var result = service.PK9DataAdd(item);
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void ทดสอบแก้ไขPK9()
        {
            PK9DataController service = new PK9DataController();
            ReceiptDataModel item = new ReceiptDataModel();
            UtilLibs utilLibs = new UtilLibs();
            DocumentNumber docNum = utilLibs.GetDocumentNo("pk9");
            item.DocumentRunno = 4;
            item.DocumentBookNumber = "1";
            item.DocumentNumber = "AC0004";
            item.DocumentDate = DateTime.Now;
            item.PayerType = "1";
            item.PayerRunno = 3;
            item.AsReceiptTo = "ค่าบำรุงพรรคการเมืองรายปี";
            item.AsReceiptToRemark = "";
            item.ReceiptAmount = 6500;
            item.ReceiptPayType = "";
            item.ReceiptDate = DateTime.Now;
            item.ReceiptBank = "";
            item.ReceiptBillNumber = "";
            item.ReceiptChqBank = "";
            item.ReceiptChqNumber = "";
            item.ReceiptOther = "";
            var result = service.PK9DataEdit(item);
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void ทดสอบลบข้อมูล()
        {
            PK9DataController service = new PK9DataController();
            var result = service.PK9DataDelete("6");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบดึงข้อมูลPK9ทั้งหมด()
        {
            PK9DataController service = new PK9DataController();
            var result = service.PK9ListAll();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบดึงข้อมูลPK9ตามเลขที่()
        {
            PK9DataController service = new PK9DataController();
            var result = service.PK9GetByID("7");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบแก้ไขDetailเอกสารพก10()
        {
            DonateDetailDataModel item = new DonateDetailDataModel
            {
                DocumentRunno = 8,
                DetailRunno = 17,
                Description = "เงินสด",
                Amount = 666,
                Remark = "",
                Bill = "",
                BankChqueNo = "",
                Asset = "",
                Benefit = ""
            };
            DonateDataController.Controllers.DonateDataController service = new DonateDataController.Controllers.DonateDataController();
            var result = service.EditDonateDetailData(item);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบดึงข้อมูลTermPK13()
        {
            PK13DataController service = new PK13DataController();
            var result = service.GetPK13Term();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ทดสอบดึงข้อมูลpk13()
        {
            PK13DataController service = new PK13DataController();
            var result = service.GetPK13byMonth(11, 2018);
            Assert.IsNotNull(result);
        }
    }
}
