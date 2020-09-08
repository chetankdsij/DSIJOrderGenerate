/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2006
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Data;
using System.Data.SqlClient;

using Microsoft.ApplicationBlocks.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;

namespace YourCompany.Modules.DSJUserSubscription
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SqlDataProvider class is a SQL Server implementation of the abstract DataProvider
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SqlDataProvider : DataProvider
    {

        #region Private Members

        private const string ProviderType = "data";
        private const string ModuleQualifier = "YourCompany_";

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructs new SqlDataProvider instance
        /// </summary>
        public SqlDataProvider()
        {
            //Read the configuration specific information for this provider

            //Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString; 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the connection string
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Gets and sets the Provider path
        /// </summary>
        
        #endregion

        #region Private Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the fully qualified name of the stored procedure
        /// </summary>
        /// <param name="name">The name of the stored procedure</param>
        /// <returns>The fully qualified name</returns>
        
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value for the field or DbNull if field has "null" value
        /// </summary>
        /// <param name="Field">The field to evaluate</param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        private Object GetNull(Object Field)
        {
            return Null.GetNull(Field, DBNull.Value);
        }

        #endregion
        public override DataSet GetEReceiptDetails(int Orderid)
        {
            SqlParameter[] ParamList = new SqlParameter[1];

            ParamList[0] = new SqlParameter("@Orderid", SqlDbType.Int, 4);
            ParamList[0].Value = Orderid;

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dsj_EReceiptReport", ParamList);
        }

        public override DataSet GetSmtpServer(int smtpId)
        {
            try
            {
                SqlParameter[] ParamList = new SqlParameter[1];
                ParamList[0] = new SqlParameter("@smtpId", SqlDbType.Int);
                ParamList[0].Value = smtpId;
                return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "SP_GetDSJSmtpServer", ParamList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override DataSet GetDSJEmailManagement(string templateName)
        {
            try
            {
                SqlParameter[] ParamList = new SqlParameter[1];
                ParamList[0] = new SqlParameter("@tagName", SqlDbType.VarChar);
                ParamList[0].Value = templateName;
                return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dsj_GetDSJEmailManagementByTagName", ParamList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override DataSet ZionSubscriptionOrder(string OperationType)
        {
            try
            {
                SqlParameter[] ParamList = new SqlParameter[1];
                ParamList[0] = new SqlParameter("@OperationType", SqlDbType.VarChar);
                ParamList[0].Value = OperationType;
               
             return   SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "USP_ZionSubscriptionOrder", ParamList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override DSJUserSubscriptionInfo ZionSubscriptionOrderInsert(string OperationType,int OrderID,string MailStatus,
            string Message,int ModifiedBy)
        {
            DSJUserSubscriptionInfo objDSJUserSubscriptionInfo = new DSJUserSubscriptionInfo();
            try
            {
                SqlParameter[] ParamList = new SqlParameter[7];
                ParamList[0] = new SqlParameter("@OperationType", SqlDbType.VarChar);
                ParamList[0].Value = OperationType;
                ParamList[1] = new SqlParameter("@OrderID", SqlDbType.Int);
                ParamList[1].Value = OrderID;
                ParamList[2] = new SqlParameter("@MailStatus", SqlDbType.VarChar,100);
                ParamList[2].Value = MailStatus;
                ParamList[3] = new SqlParameter("@Message", SqlDbType.VarChar,100);
                ParamList[3].Value = Message;
                ParamList[4] = new SqlParameter("@ModifiedBy", SqlDbType.Int);
                ParamList[4].Value = ModifiedBy;
                ParamList[5] = new SqlParameter("@Status", SqlDbType.Int, 4);
                ParamList[5].Value = 0;
                ParamList[5].Direction = ParameterDirection.Output;
                ParamList[6] = new SqlParameter("@ErrorMessage", SqlDbType.VarChar);
                ParamList[6].Value = "";
                ParamList[6].Direction = ParameterDirection.Output;
                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_ZionSubscriptionOrder", ParamList);
                objDSJUserSubscriptionInfo.Status = Convert.ToInt32(ParamList[5].Value);
                objDSJUserSubscriptionInfo.Message = Convert.ToString(ParamList[6].Value);
                return objDSJUserSubscriptionInfo;
            }
            catch (Exception ex)
            {
                objDSJUserSubscriptionInfo.Message = ex.Message;
                return objDSJUserSubscriptionInfo;
            }
        }

        public override DataSet GetUserInfo(int UserID)
        {
            try
            {
                SqlParameter[] ParamList = new SqlParameter[1];
                ParamList[0] = new SqlParameter("@UserID", SqlDbType.Int);
                ParamList[0].Value = UserID;

                return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "GetUserInfo", ParamList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}