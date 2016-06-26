namespace SharePointCustomRules
{
    using System;

    public static class GlobalConstant
    {
        public const string CategoryError = "Error";
        public const int CategoryErrorNo = 100;
        public const string CategoryWarning = "Warning";
        public const int CategoryWarningNo = 0x5f;
        public const string ConcatChar = ".";
        public const int ConsecutiveInstructionIndex = 1;
        public const string DirConcat = @"\";
        public const char FileChar = '@';
        public const string FileCustomRules = "SharePointCustomRules.CustomRules";
        public const char IndexChar = ',';
        public const string LogLevel = "LogLevel";
        public const string LogLevelInfo = "/Configuration/LogLevelInfo/LogLevelSeverity";
        public const string Namespace = "Namespace:";
        public const char NewlineChar = '\n';
        public const string Path = "Path";
        public const int SecondConsecutiveInstructionIndex = 2;
        public const char SplitChar = ':';
        public const string SplitOnMessage = "Message";
        public const string SplitOnType = "Type";
        public const string XMLConfigValues = "ConfigValues.xml";

        public static class CustomClaimsCheckConstans
        {
            public const string ProblemStatementForPropertyUsage = "This rule is applicable only to SharePoint 2010 customizations. {0}";
        }

        public static class DeprecatedAPIConstants
        {
            public const int AdjustedCharCount = 2;
            public const string DeprecatedNamespace = "Microsoft.SharePoint.Portal";
            public const string XMLNodeTypeForDepFilepath = "/Configuration/FileInfo/DepricatedFilePath";
        }

        public static class InlineCodeSupportConstants
        {
            public const string ASPXFileType = "*.aspx";
            public const string ConfigFileType = "web.config";
            public const string ExtractionPath = "ExtractionPath";
            public const string InlineCodeDefn1 = "<script runat=\"server\"";
            public const string InlineCodeDefn2 = "<script language=\"c#\" runat=\"server\">";
            public const string InlineCodeDefn3 = "</script>";
            public const int InstructionCntForInlineCodeSupport = 2;
            public const string LayoutFolderName = "LAYOUT";
            public const string PageParserAllowServerScriptDefn = "AllowServerSideScript=\"true\"";
            public const string PageParserPathExclusionDefn = "<PageParserPath VirtualPath=";
            public const string XMLNodeTypeForExtractionFilepath = "/Configuration/FileInfo/PackageExtractionPath";
        }

        public static class LoggingConstants
        {
            public const string CheckAndGetInstructionsWithinLoopIfExists = "CheckAndGetInstructionsWithinLoopIfExists() - ";
            public const string CustomItemCheck = "SharePointCustomItemCheck:Check() - ";
            public const string DeprecatedAPICheck = "SharePointDeprecatedAPICheck:Check() - ";
            public const string DeprecatedAPICheckFillSPDeprecatedAPIStore = "SharePointDeprecatedAPICheck:FillSPDeprecatedAPIStore() - ";
            public const string ErrorOccured = "Error occured in function : ";
            public const string ForwardAndFillInstructionsTillFirstBrTrue = "ForwardAndFillInstructionsTillFirstBrTrue() - ";
            public const string ForwardAndFillInstructionsTillFirstJump = "ForwardAndFillInstructionsTillFirstJump() - ";
            public const string HardCodedControlTemplateCheck = "SharePointHardCodedControlTemplatesPath:Check() - ";
            public const string InlineCodeSupportCheckCheckIfInlineCodeExists = "SharePointInlineCodeSupportCheck:CheckIfInlineCodeExists() - ";
            public const string InlineCodeSupportCheckCheckIfPageParserExclusionExists = "SharePointInlineCodeSupportCheck:CheckIfPageParserExclusionExists() - ";
            public const string InlineCodeSupportCheckProcessDirectory = "SharePointInlineCodeSupportCheck:ProcessDirectory() - ";
            public const string InlineCodeSupportCheckSearchDirectory = "SharePointInlineCodeSupportCheck:SearchDirectory() - ";
            public const string ItemsCollectionCheck = "SharePointItemsCollectionCheck:Check() - ";
            public const string ListItemUpdateCheck = "SharePointListItemUpdateCheck:Check() - ";
            public const string MonitorScopeCheck = "SharePointMonitorScopeCheck:Check() - ";
            public const string OutofBoxFilesModificationCheck = "SharePointOutofBoxFilesModificationCheck:Check() - ";
            public const string OutofBoxFilesModificationCheckProcessDirectory = "SharePointOutofBoxFilesModificationCheck:ProcessDirectory() - ";
            public const string RewindTillBackJump = "RewindTillBackJump () - ";
            public const string RowLimitExistCheckCheckIfObjectExistAndSetRowLimitVerifyProperty = "SharePointCustomRowLimitExistCheck:CheckIfObjectExistAndSetRowLimitVerifyProperty() - ";
            public const string RowLimitExistCheckCheckModuleNode = "SharePointCustomRowLimitExistCheck:Check(ModuleNode) -  ";
            public const string RowLimitExistCheckFillSPQueryObjectList = "SharePointCustomRowLimitExistCheck:FillSPQueryObjectList() - ";
            public const string RowLimitExistCheckFillSPQueryObjectListForLocals = "SharePointCustomRowLimitExistCheck:FillSPQueryObjectListForLocals() - ";
            public const string RowLimitExistCheckFillSPQueryObjectListForParameters = "SharePointCustomRowLimitExistCheck:FillSPQueryObjectListForParameters() - ";
            public const string RowLimitExistCheckParseMethod = "SharePointCustomRowLimitExistCheck:ParseMethod() - ";
            public const string RowLimitExistCheckTypeNode = "SharePointCustomRowLimitExistCheck:Check(TypeNode) - ";
            public const string RowLimitExistCheckVisitMembers = "SharePointCustomRowLimitExistCheck:VisitMembers() - ";
            public const string RowLimitValueCheck = "SharePointCustomRowLimitValueCheck:Check() - ";
            public const string RowLimitValueCheckVisitAssignmentStatement = "SharePointCustomRowLimitValueCheck:VisitAssignmentStatement() - ";
            public const string TimerjobImplementationCheck = "SharePointTimerjobImplementationCheck:Check() - ";
            public const string TimerjobImplementationCheckSearchForSPFeatureReceiverClass = "SharePointTimerjobImplementationCheck:SearchForSPFeatureReceiverClass() - ";
            public const string TimerjobImplementationCheckVisitMethod = "SharePointTimerjobImplementationCheck:VisitMethod() - ";
            public const string VerboseLoggingCheck = "SharePointVerboseLoggingCheck:Check() - ";
            public const string WebconfigEditCheck = "SharePointWebconfigEditCheck:Check() - ";
            public const string WebconfigEditCheckValidateForConfigManager = "SharePointWebconfigEditCheck:ValidateForConfigManager() - ";
            public const string WebconfigEditCheckValidateForStreamWriter = "SharePointWebconfigEditCheck:ValidateForStreamWriter() - ";
            public const string WebconfigEditCheckValidateForXMLDocument = "SharePointWebconfigEditCheck:ValidateForXMLDocument() - ";
            public const string WebconfigEditCheckValidateForXMLTextWriter = "SharePointWebconfigEditCheck:ValidateForXMLTextWriter() - ";
        }

        public static class LoopDetectorConstants
        {
            public const string InstructionForAddressLoadForLocal = "Ldloc";
            public const string InstructionForCurrentEnumerator = "get_Current";
            public const string InstructionForNextEnumerator = "MoveNext";
            public const string InstructionForPushIntegerOnEvaluationStack = "Ldc_I";
        }

        public static class MonitorScopeConstants
        {
            public const int DisplayLevelOnDemandValueDeveloperDashboard = 1;
            public const int DisplayLevelOnValueDeveloperDashboard = 2;
            public const string Instruction1ForDeveloperDashboard = "SharePoint.Administration.SPDeveloperDashboardSettings.set_DisplayLevel";
            public const string Instruction2ForDeveloperDashboard = "Microsoft.SharePoint.Administration.SPPersistedObject.Update";
            public const string InstructionForMonitorScope = "Microsoft.SharePoint.Utilities.SPMonitoredScope";
            public const int MaxInstructionCntForDevloperDashboard = 2;
            public const string MethodOnInit = "OnInit";
            public const string MethodRender = "Render";
        }

        public static class RowLimitExistCheckConstants
        {
            public const int LowerBoundRowLimit = 1;
            public const int MaxInstructionCntForSPQueryRowLimit = 2;
            public const string RowLimitCheckCmd = "SPQuery.set_RowLimit";
            public const string RowLimitReslnForLocalObject = "RowLimitForLocalObject";
            public const string RowLimitReslnForMemberObject = "RowLimitForMemberObject";
            public const string SPQueryObjectType = "Microsoft.SharePoint.SPQuery";
            public const int UpperBoundRowLimit = 0x7d0;
        }

        public static class SharePointBlobCacheCheck
        {
            public const string InstructionForBlobCacheContainsBlobCache = "BlobCache";
            public const string InstructionForBlobCacheNewSPWebAdmin = "Microsoft.SharePoint.Administration.SPWebConfigModification.set_Path";
            public const string InstructionForBlobCacheNewSPWebConfigModification = "Microsoft.SharePoint.Administration.SPWebConfigModification(";
            public const string OpCodeForBlobCacheLoadString = "Ldstr";
        }

        public static class SharePointFeatureReceiverCheckConstants
        {
            public const string BaseClassNameForFeatureReceiver = "Microsoft.SharePoint.SPFeatureReceiver";
            public const string MethodNameFeatureActivated = "FeatureActivated";
            public const string MethodNameFeatureDeactivating = "FeatureDeactivating";
            public const string MethodNameFeatureInstalled = "FeatureInstalled";
            public const string MethodNameFeatureUninstalling = "FeatureUninstalling";
            public const string MethodNameFeatureUpgrading = "FeatureUpgrading";
            public const string MethodNameStartsWithMicrosoft = "microsoft.";
            public const string MethoNameStartsWithSystem = "system.";
            public const string ProblemStatementForCatchBlocks = "The catch block at line number {0} in method {1} must log to ULS Logs. For ULS logging, please use the Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.";
            public const string ProblemStatementForTryCatchNotPresent = "The method {0} does not implement a top level try catch block";
            public const string ProblemStatmentForNoThrowFound = "The catch block at line number {0} in method {1} must throw the exception back to SharePoint.";
            public const string sp2010LoggingMethodName = "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace";
            public const string SPFeatureRecieverMethod = "SPFeatureReceiverMethod";
            public const string symbolsNotFound = "'[symbols not found to locate the line number]'";
        }

        public static class SharePointGetGenericSetupPath
        {
            public const string InstructionForGenericSetupPathValue = "Microsoft.SharePoint.Utilities.SPUtility.GetGenericSetupPath";
            public const string OpCodeForGenericSetupPath = "Call";
        }

        public static class SharePointHardCodedControlTemplatesPath
        {
            public const string InstructionForControlTemplateValue = "_CONTROLTEMPLATES";
            public const string OpCodeForControlTemplate = "Ldstr";
        }

        public static class SharePointHardCodedlayoutsFolderPath
        {
            public const string InstructionForLayoutValue = "_layouts/";
            public const string OpCodeForLayout = "Ldstr";
        }

        public static class SharePointMonitorScopeWebpartCheck
        {
            public const string BaseClassNameForWebpart = "Microsoft.SharePoint.WebPartPages.WebPart";
            public const string InstructionCallVirtDisposable = "System.IDisposable.Dispose";
            public const string InstructionNewobj = "Microsoft.SharePoint.Utilities.SPMonitoredScope";
            public const string Methodctor = ".ctor";
            public const string Methodgetproperty = "get_";
            public const string MethodOnInit = "OnInit";
            public const string MethodOnPreRender = "OnPreRender";
            public const string MethodRender = "Render";
            public const string Methodsetproperty = "set_";
            public const string WebPartSPMonitoredScopeCheck = "SPMonitoredScope is absent in {0} Webpart";
        }

        public static class SharePointRunWithElevatedPrivilegesCheck
        {
            public const string CriticalServiceName = "SCardSvr,SNMPTRAP";
            public const string FunctionNameForDelegate = "CS$<>9__CachedAnonymousMethodDelegate";
            public const string FunctionTypeForDelegate = "Microsoft.SharePoint.SPSecurity+CodeToRunElevated";
            public const string InstructionValueForDirectoryDelete = "System.IO.Directory.Delete";
            public const string InstructionValueForDirectoryInfoDelete = "System.IO.DirectoryInfo.Delete";
            public const string InstructionValueForFileDelete = "System.IO.File.Delete";
            public const string InstructionValueForFileSystemInfoDelete = "System.IO.FileSystemInfo.Delete";
            public const string InstructionValueForNewServiceController = "System.ServiceProcess.ServiceController(";
            public const string InstructionValueForServicePause = "System.ServiceProcess.ServiceController.Pause";
            public const string InstructionValueForServiceRefresh = "System.ServiceProcess.ServiceController.Refresh";
            public const string InstructionValueForServiceStart = "System.ServiceProcess.ServiceController.Start";
            public const string InstructionValueForServiceStop = "System.ServiceProcess.ServiceController.Stop";
            public const string InstructionValueForUserProfileDeletion = "Microsoft.Office.Server.UserProfiles.UserProfileManager.RemoveUserProfile";
            public const string OpCodeForCall = "Call";
            public const string OpCodeForCallVirtual = "Callvirt";
            public const string OpCodeForLoadString = "Ldstr";
            public const string OpCodeForNewObject = "Newobj";
            public const string RunWithElevatedPrivilegesForFileDeleteCheck = "FileDeleteOperationCheck";
            public const string RunWithElevatedPrivilegesForProfileDeleteCheck = "ProfileDeleteOperationCheck";
            public const string RunWithElevatedPrivilegesForServiceCheck = "ServiceOperationCheck";
        }

        public static class SharePointSeachSQLSyntaxCheck
        {
            public const string InstructionValueForSearchService = "Microsoft.Office.Server.Search.Query.QueryService.Query";
            public const string InstructionValueForSearchServiceSoap = "QueryServiceSoapClient.Query";
            public const string InstructionValueForSQLSyntax = "Microsoft.Office.Server.Search.Query.FullTextSqlQuery(";
            public const string OpCodeForSearchService = "Callvirt";
            public const string OpCodeForSQLSyntax = "Newobj";
        }

        public static class SharePointXMLDatasourceTransformCheck
        {
            public const string InstructionValueForTransformCheck = "System.Web.UI.WebControls.XmlDataSource.set_Transform";
            public const string OpCodeForTransformCheck = "Callvirt";
        }

        public static class SPDiagnosticsImplementationConstants
        {
            public const string BaseClassNameForEventReceiver1 = "Microsoft.SharePoint.SPWebEventReceiver";
            public const string BaseClassNameForEventReceiver2 = "Microsoft.SharePoint.SPListEventReceiver";
            public const string BaseClassNameForEventReceiver3 = "Microsoft.SharePoint.SPItemEventReceiver";
            public const string BaseClassNameForEventReceiver4 = "Microsoft.SharePoint.SPEmailEventReceiver";
            public const string BaseClassNameForFeatureReceiver = "Microsoft.SharePoint.SPFeatureReceiver";
            public const string BaseClassNameForTimerJob = "Microsoft.SharePoint.Administration.SPJobDefinition";
            public const string EventReceiverULSLoggingCheck = "Please add SPDiagnostics WriteTrace call at start and end of Function {0}";
            public const string FeatureReceiverULSLoggingCheck = "Please add SPDiagnostics WriteTrace call at start and end of Function {0}";
            public const string InstructionForADONET1 = "System.Data.Common.DbConnection.Open";
            public const string InstructionForADONET2 = "System.Data.SqlClient";
            public const string InstructionForCalltoSPDiagnosticsServiceBaseWriteTrace = "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace";
            public const string InstructionForSPDiagnosticsServiceget_Areas = "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.get_Areas";
            public const string InstructionForSPDiagnosticsServiceget_Local = "Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local";
            public const string InstructionForWebService = "System.Web.Services.Protocols.SoapHttpClientProtocol.Invoke";
            public const string MethodNameExecute = "Execute";
            public const string MethodNameFeatureActivated = "FeatureActivated";
            public const string MethodNameFeatureDeactivating = "FeatureDeactivating";
            public const string TimerJobULSLoggingCheck = "Please add SPDiagnostics WriteTrace call at start and end of 'Execute' Function in {0}";
            public const string WebServiceULSLoggingCheck = "Please add SPDiagnostics WriteTrace call at start and end of every Web Service call - {0}";
        }

        public static class SPLISTItemConstants
        {
            public const string InstructionForListItemUpdate = "SPItem.Update";
            public const string InstructionForSPListItem = "SPList.get_Items";
            public const string InstructionForSPListItem2 = "SPList.GetItems";
            public const string InstructionForSPListItemById = "SPList.GetItemById";
            public const string InstructionForSPListItemCount = "SPList.get_Count";
        }

        public static class TimerjobImplementationConstants
        {
            public const string BaseClassNameForTimerJob = "Microsoft.SharePoint.Administration.SPJobDefinition";
            public const string BaseClassNameForTimerJobFeature = "Microsoft.SharePoint.SPFeatureReceiver";
            public const string Instruction1ForJobMinuteSchedule = "Microsoft.SharePoint.SPMinuteSchedule";
            public const string Instruction2ForJobMinuteSchedule = "Microsoft.SharePoint.Administration.SPJobDefinition.set_Schedule";
            public const string Instruction3ForJobMinuteSchedule = "Microsoft.SharePoint.Administration.SPPersistedObject.Update";
            public const int InstructionCountForMinuteScheduleJob = 3;
            public const int InstructionCountForTimerJob = 2;
            public const string InstructionForJobDelete = "Microsoft.SharePoint.Administration.SPPersistedObject.Delete";
            public const string InstructionForJobExist = "Microsoft.SharePoint.Administration.SPWebApplication.get_JobDefinitions";
            public const string MethodNameFeatureActivated = "FeatureActivated";
            public const string MethodNameFeatureDeactivating = "FeatureDeactivating";
            public const string TimerJobReslnForJobDeleteCheck = "JobDeleteCheck";
            public const string TimerJobReslnForJobExistsCheck = "JobExistsCheck";
            public const string TimerJobReslnForJobFeatureClassCheck = "JobFeatureReceiverCheck";
            public const string TimerJobReslnForJobMinuteScheduleCheck = "JobMinuteScheduleCheck";
        }

        public static class ULSLoggingCustomRuleConstants
        {
            public const string problemStatementForCatchBlocks = "The catch block at line number {0} in method {1} must log to ULS Logs. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.";
            public const string problemStatementForIndirectUnsupportedMethods = "Method '{0}' calls one or both of the unsupported APIs '{1}' and '{2}' indirectly. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.";
            public const string problemStatementForUnsupportedMethods = "Method '{0}' calls '{1}' at line number {2} which is unsupported. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.";
            public const string sp2007UnsupportedMethodCall1 = "Microsoft.Office.Server.Diagnostics.PortalLog.LogString";
            public const string sp2007UnsupportedMethodCall2 = "Microsoft.Office.Server.Diagnostics.PortalLog.DebugLogString";
            public const string sp2010LoggingMethodName = "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace";
            public const string symbolsNotFound = "'[symbols not found to locate the line number]'";
        }

        public static class VerboseLoggingConstants
        {
            public const string InstructionForTraceSeverity = "Microsoft.SharePoint.Administration.TraceSeverity";
            public const int SeverityValueVerboseForTrace = 100;
        }

        public static class WebConfigEditConstants
        {
            public const string Instruction1ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPWebConfigModification";
            public const string Instruction2ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPWebApplication.get_WebConfigModifications";
            public const string Instruction3ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPWebConfigModification>.Add";
            public const string Instruction4ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPWebService.ApplyWebConfigModifications";
            public const string Instruction5ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPPersistedObject.Update";
            public const string Instruction6ForWebConfigEdit = "Microsoft.SharePoint.Administration.SPWebConfigModification>.Remove";
            public const int InstructionCountForWebConfigEdit = 4;
            public const string InstructionForConfigurationSave = "System.Configuration.Configuration.Save";
            public const string InstructionForCreateText = "System.IO.File.CreateText";
            public const string InstructionForDocumentSave = "System.Xml.XmlDocument.Save";
            public const string InstructionForOpenExeConfiguration = "System.Configuration.ConfigurationManager.OpenExeConfiguration";
            public const string InstructionForWriteLine = "System.IO.TextWriter.WriteLine";
            public const string InstructionForWriteTextWriter = "System.Xml.XmlWriter.WriteElementString";
            public const string InstructionForXMLTextWriter = "System.Xml.XmlTextWriter";
            public const string sPossibleWebConfigNamePart1 = ".exe";
            public const string sPossibleWebConfigNamePart2 = ".config";
            public const string sWebConfigName = "web.config";
        }
    }
}

