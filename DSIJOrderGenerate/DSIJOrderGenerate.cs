using DotNetNuke.Entities.Users;
using System;
using YourCompany.Modules.DSJUserSubscription;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;
using System.Collections;
using DotNetNuke.Common.Utilities;

namespace DSIJOrderGenerate
{
    class DSIJOrderGenerate
    {
        static void Main(string[] args)
        {
            DSIJOrderGenerate objDSIJOrderGenerate = new DSIJOrderGenerate();
            objDSIJOrderGenerate.writeprocessentry("Exe start....................");
            objDSIJOrderGenerate.CreateZionSubscriptionOrder();
        }
        void CreateZionSubscriptionOrder()
        {
            try
            {
                DSJUserSubscriptionController objDSJUserSubscriptionController = new DSJUserSubscriptionController();
                DataSet ds = objDSJUserSubscriptionController.ZionSubscriptionOrder("SELECT");
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DSJUserSubscriptionController objDSJUserSubscription = new DSJUserSubscriptionController();
                        DSJUserSubscriptionInfo objDSJUSInfo = new DSJUserSubscriptionInfo();
                        UserInfoNew objUserInfo;
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item["userid"])))
                            {
                                if (Convert.ToInt32(item["orderid"]) != 0)
                                {
                                    writeprocessentry("Order Mail Send Start of Order id " + Convert.ToInt32(item["orderid"]) + "  ........");
                                    objUserInfo = CreateTempUser(Convert.ToInt32(item["userid"])); // objUC.GetUser(0, Convert.ToInt32(item["userid"]));
                                    objDSJUSInfo.OrderId = Convert.ToInt32(item["orderid"]);
                                    objDSJUSInfo.UserID = Convert.ToInt32(item["userid"]);
                                    SendSubscriptionMail(objUserInfo, objDSJUserSubscription, null, objDSJUSInfo);
                                    writeprocessentry("Order Mail Send End of Order id " + Convert.ToInt32(item["orderid"]) + "  ........");
                                }
                                else
                                {
                                    writeprocessentry("userid 0 ........");
                                }
                            }
                            else
                            {
                                writeprocessentry("userid null........");
                            }
                        }
                    }
                    else
                    {
                        writeprocessentry("No table avaible........");
                    }
                }
                else
                {
                    writeprocessentry("No table avaible........");
                }
            }
            catch (Exception ex)
            {
                writeprocessentry(ex.Message + " " + ex.StackTrace);
            }
        }

        private void SendSubscriptionMail(UserInfoNew objUserInfo, DSJUserSubscriptionController objDSJUserSubscription, ArrayList Products, DSJUserSubscriptionInfo objDSJUSInfo)
        {
            try
            {
                writeprocessentry("Start SendSubscriptionMail...............");

                ReportViewer ReportViewer1 = new ReportViewer();
                DataSet Ds = new DataSet();
                DataSet smtp = new DataSet();
                Ds = objDSJUserSubscription.GetEReceiptDetails(objDSJUSInfo.OrderId);
                if (Ds != null)
                {
                    if (Ds.Tables.Count > 0)
                    {
                        if (Ds.Tables[0].Rows.Count > 0)
                        {
                            System.Collections.Generic.List<ReportParameter> paramList = new System.Collections.Generic.List<ReportParameter>();
                            paramList.Add(new ReportParameter("FName", objUserInfo.FirstName));
                            paramList.Add(new ReportParameter("LName", objUserInfo.LastName));
                            paramList.Add(new ReportParameter("Address", objUserInfo.Address == null ? "" : objUserInfo.Address.Trim()));
                            paramList.Add(new ReportParameter("Tel", objUserInfo.Telephone == null ? "" : objUserInfo.Telephone));
                            paramList.Add(new ReportParameter("Email", objUserInfo.Email));
                            paramList.Add(new ReportParameter("City", objUserInfo.City == null ? "" : objUserInfo.City.Trim()));
                            paramList.Add(new ReportParameter("Pin", objUserInfo.PostalCode == null ? "" : objUserInfo.PostalCode));
                            paramList.Add(new ReportParameter("Prefix", objUserInfo.Prefix == null ? "" : objUserInfo.Prefix));
                            String AmountinWords = GetAmountInWord(Ds);
                            paramList.Add(new ReportParameter("WordsAmt", AmountinWords.ToString()));
                            //try
                            //{
                            //    foreach (var item in paramList)
                            //    {
                            //        writeprocessentry("ReportParameter " + item.Name + ": " + item.Values[0]);
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    writeprocessentry("Exception " + ex.Message + " " + ex.StackTrace);
                            //}

                            //ReportViewer1.Reset();
                            //ReportViewer1.ProcessingMode =  ProcessingMode.Local;
                            ReportViewer1.LocalReport.EnableExternalImages = true;
                            ReportViewer1.LocalReport.DataSources.Clear();
                            ReportViewer1.LocalReport.ReportPath = System.Configuration.ConfigurationManager.AppSettings["ReportPath"];
                            ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("EReceipt_dsj_EReceiptReport", Ds.Tables[0]));
                            ReportViewer1.LocalReport.SetParameters(paramList);
                            ReportViewer1.LocalReport.Refresh();
                            writeprocessentry("Report1 created");

                            Warning[] warnings = null;
                            string[] streamids = null;
                            string mimeType = "application/pdf";
                            string encoding = System.Text.Encoding.UTF8.ToString();
                            string extension = "";
                            string format = "PDF";
                            byte[] bytes = ReportViewer1.LocalReport.Render(format, null, out mimeType, out encoding, out extension, out streamids, out warnings);

                            MemoryStream memoryStream = new MemoryStream(bytes);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            ReportViewer1.Visible = false;
                            writeprocessentry("Ereceipt before send email: " + objUserInfo.Email);
                            String PDFFileName = Convert.ToString("ERECEIPT-" + DateTime.Now.ToString("ddMMMyyyy") + ".pdf").ToLower();
                            //File.WriteAllBytes(@"D:\"+ PDFFileName + "", bytes);

                            DataTable dt = objDSJUserSubscription.GetDSJEmailManagement("PaymentReceiptPayU").Tables[0];
                            string Subject = dt.Rows[0]["subject"].ToString();
                            string MessageBody = dt.Rows[0]["body"].ToString();
                            string mailstatus = new EmailSend().SendEmail(objUserInfo.Email, MessageBody, Subject, dt, memoryStream, PDFFileName);
                            DSJUserSubscriptionInfo objDSJUserSubscriptionInfo = objDSJUserSubscription.ZionSubscriptionOrderInsert("UPDATE",
                            objDSJUSInfo.OrderId, mailstatus == "True" ? "Y" : "N", mailstatus, objDSJUSInfo.UserID);
                            if (objDSJUserSubscriptionInfo.Status == 1)
                            {
                                writeprocessentry("ZionSubscriptionOrderInsert update done");
                            }
                            else
                            {
                                writeprocessentry("ZionSubscriptionOrderInsert update not done");
                            }
                            writeprocessentry("Ereceipt after send email: " + objUserInfo.Email);
                            writeprocessentry("End SendSubscriptionMail...............");
                        }
                        else
                        {
                            writeprocessentry("Payment Reciept data not availble");
                        }
                    }
                    else
                    {
                        writeprocessentry("Data set has not table...............");
                    }
                }
                else
                {
                    writeprocessentry("Data set has empty...............");
                }
            }
            catch (Exception ex)
            {
                writeprocessentry("Exception " + ex.Message + " " + ex.StackTrace);
            }
        }
       
        public UserInfoNew CreateTempUser(int UserID)
        {
            UserInfoNew objUserInfo = new UserInfoNew();
            try
            {
                objUserInfo.FirstName = "";
                DSJUserSubscriptionController objDSJUserSubscriptionController = new DSJUserSubscriptionController();
                DataSet ds = objDSJUserSubscriptionController.GetUserInfo(UserID);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        objUserInfo.UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
                        objUserInfo.FirstName = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["FirstName"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["FirstName"]);
                        objUserInfo.LastName = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["LastName"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["LastName"]);
                        objUserInfo.Address = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["Address"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                        objUserInfo.Telephone = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["Telephone"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Telephone"]);
                        objUserInfo.Email = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["Email"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Email"]);
                        objUserInfo.City = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["City"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                        objUserInfo.PostalCode = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["PostalCode"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["PostalCode"]);
                        objUserInfo.Prefix = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["Prefix"])) ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Prefix"]);

                        writeprocessentry("Userinfo data FirstName: " + objUserInfo.FirstName);
                        writeprocessentry("Userinfo data LastName: " + objUserInfo.LastName);
                        writeprocessentry("Userinfo data Address: " + objUserInfo.Address);
                        writeprocessentry("Userinfo data Telephone: " + objUserInfo.Telephone);
                        writeprocessentry("Userinfo data Email: " + objUserInfo.Email);
                        writeprocessentry("Userinfo data City: " + objUserInfo.City);
                        writeprocessentry("Userinfo data PostalCode: " + objUserInfo.PostalCode);
                        writeprocessentry("Userinfo data Prefix: " + objUserInfo.Prefix);

                    }
                }
            }
            catch (Exception ex)
            {
                writeprocessentry("Userinfo data error: " + ex.Message + " " + ex.StackTrace);
            }

            return objUserInfo;
        }
        public String GetAmountInWord(DataSet Ds)
        {
            Double Result = 0;
            if ((Ds.Tables[0].Rows.Count > 0))
            {
                //for (int i = 0; i <= Ds.Tables[0].Rows.Count - 1; i++)
                //{
                Result = Result + Convert.ToDouble(Ds.Tables[0].Rows[0]["Amount"]);
                //}
            }
            return changeCurrencyToWords(Result.ToString());
        }
        #region Amount To Word Convertion
        public String changeNumericToWords(double numb)
        {

            String num = numb.ToString();

            return changeToWords(num, false);

        }

        public String changeCurrencyToWords(String numb)
        {

            return changeToWords(numb, true);

        }

        public String changeNumericToWords(String numb)
        {

            return changeToWords(numb, false);

        }

        public String changeCurrencyToWords(double numb)
        {

            return changeToWords(numb.ToString(), true);

        }

        private String changeToWords(String numb, bool isCurrency)
        {

            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";

            String endStr = (isCurrency) ? ("Only") : ("");

            try
            {

                int decimalPlace = numb.IndexOf(".");

                if (decimalPlace > 0)
                {

                    wholeNo = numb.Substring(0, decimalPlace);

                    points = numb.Substring(decimalPlace + 1);

                    if (Convert.ToInt32(points) > 0)
                    {

                        andStr = (isCurrency) ? ("and") : ("Rupees");// just to separate whole numbers from points/cents

                        endStr = (isCurrency) ? ("Paise " + endStr) : ("");

                        pointStr = translateCents(points);

                    }

                }

                val = String.Format("{0} {1}{2} {3}", translateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);

            }

            catch {; }

            return val;

        }

        private String translateWholeNumber(String number)
        {

            string word = "";

            try
            {

                bool beginsZero = false;//tests for 0XX

                bool isDone = false;//test if already translated

                double dblAmt = (Convert.ToDouble(number));

                //if ((dblAmt > 0) && number.StartsWith("0"))

                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric

                    beginsZero = number.StartsWith("0");

                    int numDigits = number.Length;

                    int pos = 0;//store digit grouping

                    String place = "";//digit grouping name:hundres,thousand,etc...

                    switch (numDigits)
                    {

                        case 1://ones' range

                            word = ones(number);

                            isDone = true;

                            break;

                        case 2://tens' range

                            word = tens(number);

                            isDone = true;

                            break;

                        case 3://hundreds' range

                            pos = (numDigits % 3) + 1;

                            place = " Hundred ";

                            break;

                        case 4://thousands' range

                        case 5:

                        case 6:

                            pos = (numDigits % 4) + 1;

                            place = " Thousand ";

                            break;

                        case 7://millions' range

                        case 8:

                        case 9:

                            pos = (numDigits % 7) + 1;

                            place = " Million ";

                            break;

                        case 10://Billions's range

                            pos = (numDigits % 10) + 1;

                            place = " Billion ";

                            break;

                        //add extra case options for anything above Billion...

                        default:

                            isDone = true;

                            break;

                    }

                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)

                        word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));

                        //check for trailing zeros

                        if (beginsZero) word = " and " + word.Trim();

                    }

                    //ignore digit grouping names

                    if (word.Trim().Equals(place.Trim())) word = "";

                }

            }

            catch {; }

            return word.Trim();

        }

        private String tens(String digit)
        {

            int digt = Convert.ToInt32(digit);

            String name = null;

            switch (digt)
            {

                case 10:

                    name = "Ten";

                    break;

                case 11:

                    name = "Eleven";

                    break;

                case 12:

                    name = "Twelve";

                    break;

                case 13:

                    name = "Thirteen";

                    break;

                case 14:

                    name = "Fourteen";

                    break;

                case 15:

                    name = "Fifteen";

                    break;

                case 16:

                    name = "Sixteen";

                    break;

                case 17:

                    name = "Seventeen";

                    break;

                case 18:

                    name = "Eighteen";

                    break;

                case 19:

                    name = "Nineteen";

                    break;

                case 20:

                    name = "Twenty";

                    break;

                case 30:

                    name = "Thirty";

                    break;

                case 40:

                    name = "Fourty";

                    break;

                case 50:

                    name = "Fifty";

                    break;

                case 60:

                    name = "Sixty";

                    break;

                case 70:

                    name = "Seventy";

                    break;

                case 80:

                    name = "Eighty";

                    break;

                case 90:

                    name = "Ninety";

                    break;

                default:

                    if (digt > 0)
                    {

                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));

                    }

                    break;

            }

            return name;

        }

        private String ones(String digit)
        {

            int digt = Convert.ToInt32(digit);

            String name = "";

            switch (digt)
            {

                case 1:

                    name = "One";

                    break;

                case 2:

                    name = "Two";

                    break;

                case 3:

                    name = "Three";

                    break;

                case 4:

                    name = "Four";

                    break;

                case 5:

                    name = "Five";

                    break;

                case 6:

                    name = "Six";

                    break;

                case 7:

                    name = "Seven";

                    break;

                case 8:

                    name = "Eight";

                    break;

                case 9:

                    name = "Nine";

                    break;

            }

            return name;

        }

        private String translateCents(String cents)
        {

            String cts = "", digit = "", engOne = "";

            for (int i = 0; i < cents.Length; i++)
            {

                digit = cents[i].ToString();

                if (digit.Equals("0"))
                {

                    engOne = "Zero";

                }

                else
                {

                    engOne = ones(digit);

                }

                cts += " " + engOne;

            }

            return cts;

        }
        #endregion
        public void writeprocessentry(string message)
        {
            try
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings["filePath"];
                string filename = String.Format("{0:yyyy-MM-dd}.txt", DateTime.Now);
                string path = Path.Combine(filePath, filename);
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("");
                    }
                }
                message = message + " " + DateTime.Now.ToLongTimeString();
                using (FileStream fs = new FileStream(filePath + filename, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("\n");
                    sw.WriteLine(message);
                }
            }
            catch (Exception En)
            {
            }
        }
    }
}
