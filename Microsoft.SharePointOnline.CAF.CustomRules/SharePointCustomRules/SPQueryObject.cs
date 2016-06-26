namespace SharePointCustomRules
{
    using System;
using Microsoft.FxCop.Sdk;

    public class SPQueryObject
    {
        private bool bIsStatic = false;
        private bool bProblemAdded = false;
        private bool bVerifiedForRowLimitProperty = false;
        private string sMethodName = string.Empty;
        private string sNameSpaceName = string.Empty;
        private string sObjectClassName = string.Empty;
        private string sObjectName = string.Empty;
        private string sObjectType = string.Empty;
        private string sObjectValue = string.Empty;

#if ORIGINAL
#else
        public Node ObjectNode
        {
            get;
            set;
        }
#endif

        public string ClassName
        {
            get
            {
                return this.sObjectClassName;
            }
            set
            {
                this.sObjectClassName = value;
            }
        }

        public bool IsProblemAdded
        {
            get
            {
                return this.bProblemAdded;
            }
            set
            {
                this.bProblemAdded = value;
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.bIsStatic;
            }
            set
            {
                this.bIsStatic = value;
            }
        }

        public string MethodName
        {
            get
            {
                return this.sMethodName;
            }
            set
            {
                this.sMethodName = value;
            }
        }

        public string NameSpaceName
        {
            get
            {
                return this.sNameSpaceName;
            }
            set
            {
                this.sNameSpaceName = value;
            }
        }

        public string ObjectName
        {
            get
            {
                return this.sObjectName;
            }
            set
            {
                this.sObjectName = value;
            }
        }

        public string ObjectType
        {
            get
            {
                return this.sObjectType;
            }
            set
            {
                this.sObjectType = value;
            }
        }

        public string ObjectValue
        {
            get
            {
                return this.sObjectValue;
            }
            set
            {
                this.sObjectValue = value;
            }
        }

        public bool VerifyRowLimitProperty
        {
            get
            {
                return this.bVerifiedForRowLimitProperty;
            }
            set
            {
                this.bVerifiedForRowLimitProperty = value;
            }
        }
    }
}
