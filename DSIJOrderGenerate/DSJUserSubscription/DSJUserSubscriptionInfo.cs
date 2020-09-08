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
using System.Configuration;
using System.Data;

namespace YourCompany.Modules.DSJUserSubscription
{
    /// -----------------------------------------------------------------------------
    ///<summary>
    /// The Info class for the DSJUserSubscription
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public class DSJUserSubscriptionInfo
    {

    #region private Members

        private int _PortalId;
        private int _OrderId;
        private string _SubscriptionNo;
        private int _CreatedBy;
        private DateTime _CreatedDate;
        private int _OrderFamilyId;
        private int _ModifiedBy;
        private DateTime _ModifiedDate;
        private char _SubscriptionType;
        private bool _IsComplimentary;
        private int _SourceEventId;
        private string _ComplimentaryBy;
        private string _ComplimentaryComment;
        private bool _IsActive;
        private int _IsolatedOrderId;
        public int Status { get; set; }
        public string Message { get; set; }
        public int DSIJZionSubscriptionID { get; set; }
        public string SubscriptionID { get; set; }
        public int UserID { get; set; }
        #endregion

        #region Constructors

        // initialization
        public DSJUserSubscriptionInfo()
        {
        }

    #endregion

    #region public Properties

        public int IsolatedOrderId
        {
            get
            {
                return _IsolatedOrderId;
            }
            set
            {
                _IsolatedOrderId = value;
            }
        }

        public int PortalId
        {
            get
            {
                return _PortalId;
            }
            set
            {
                _PortalId = value;
            }
        }
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
            }
        }

        public int OrderId
        {
            get
            {
                return _OrderId;
            }
            set
            {
                _OrderId = value;
            }
        }

        public string SubscriptionNo
        {
            get
            {
                return _SubscriptionNo;
            }
            set
            {
                _SubscriptionNo = value;
            }
        }

        public int CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                _CreatedBy = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
            }
        }

        public int ModifiedBy
        {
            get
            {
                return _ModifiedBy;
            }
            set
            {
                _ModifiedBy = value;
            }
        }

        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
            }
        }

        public int OrderFamilyId
        {
            get
            {
                return _OrderFamilyId;
            }
            set
            {
                _OrderFamilyId = value;
            }
        }

        public int SourceEventId
        {
            get
            {
                return _SourceEventId;
            }
            set
            {
                _SourceEventId = value;
            }
        }

        public char SubscriptionType
        {
            get
            {
               return _SubscriptionType;
            }
            set
            {
                _SubscriptionType = value;
            }
        }

        public bool IsComplimentary
        {
            get
            {
                return _IsComplimentary;
            }
            set
            {
                _IsComplimentary = value;
            }
        }

        public string ComplimentaryBy
        {
            get
            {
                return _ComplimentaryBy;
            }
            set
            {
                _ComplimentaryBy = value;
            }
        }

        public string ComplimentaryComment
        {
            get
            {
                return _ComplimentaryComment;
            }
            set
            {
                _ComplimentaryComment = value;
            }
        }



    #endregion
    
    }
}
