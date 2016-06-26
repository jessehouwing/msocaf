namespace SharePointCustomRules
{
    using System;

    public class SPClaimsTypesToBeChecked
    {
        private string m_sAPIType;
        private string m_sMessage;
        private string m_sNamespace;

        public string APIType
        {
            get
            {
                return this.m_sAPIType;
            }
            set
            {
                this.m_sAPIType = value;
            }
        }

        public string Message
        {
            get
            {
                return this.m_sMessage;
            }
            set
            {
                this.m_sMessage = value;
            }
        }

        public string Namespace
        {
            get
            {
                return this.m_sNamespace;
            }
            set
            {
                this.m_sNamespace = value;
            }
        }
    }
}

