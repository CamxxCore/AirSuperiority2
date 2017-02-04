using System.Runtime.Serialization;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace AirSuperiority.LiveStats
{
    [System.Diagnostics.DebuggerStepThrough]
    [DataContract(Name = "UserTicket", Namespace = "http://schemas.datacontract.org/2004/07/ASUPService")]
    public partial class UserTicket : object, IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataField;
        private int MagicNumberField;
        private string StatDataHashField;
        private string StatDataSaltKey;
        public int SessionKeyField;

        public ExtensionDataObject ExtensionData
        {
            get { return extensionDataField; }
            set { extensionDataField = value; }
        }

        [DataMember]
        public int MagicNumber
        {
            get { return MagicNumberField; }
            set { MagicNumberField = value; }
        }

        [DataMember]
        public string StatDataHash
        {
            get { return StatDataHashField; }
            set { StatDataHashField = value; }
        }

        [DataMember]
        public string StatDataSalt
        {
            get { return StatDataSaltKey; }
            set { StatDataSaltKey = value; }
        }

        [DataMember]
        public int SessionKey
        {
            get { return SessionKeyField; }
            set { SessionKeyField = value; }
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
    [DataContract(Name = "UserInfo", Namespace = "http://schemas.datacontract.org/2004/07/ASUPService")]
    public partial class UserInfo : object, IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataField;
        private int UIDField;
        private string UsernameField;
        private int CurrentLevelField;
        private int TotalExpField;
        private int TotalKillsField;
        private int TotalDeathsField;

        public ExtensionDataObject ExtensionData
        {
            get { return extensionDataField; }
            set { extensionDataField = value; }
        }

        [DataMember]
        public int UID
        {
            get { return UIDField; }
            set { UIDField = value; }
        }

        [DataMember]
        public string Username
        {
            get { return UsernameField; }
            set { UsernameField = value; }
        }

        [DataMember]
        public int CurrentLevel
        {
            get { return CurrentLevelField; }
            set { CurrentLevelField = value; }
        }

        [DataMember]
        public int TotalExp
        {
            get { return TotalExpField; }
            set { TotalExpField = value; }
        }

        [DataMember]
        public int TotalKills
        {
            get { return TotalKillsField; }
            set { TotalKillsField = value; }
        }

        [DataMember]
        public int TotalDeaths
        {
            get { return TotalDeathsField; }
            set { TotalDeathsField = value; }
        }
    }

    [ServiceContract(ConfigurationName = "IASUPService")]
    public interface IASUPService
    {
        [OperationContract(Action = "http://tempuri.org/IASUPService/GetPlayerStat", ReplyAction = "http://tempuri.org/IASUPService/GetPlayerStatResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int GetPlayerStat(int uid, string statName);

        [OperationContract(Action = "http://tempuri.org/IASUPService/GetPlayerStat", ReplyAction = "http://tempuri.org/IASUPService/GetPlayerStatResponse")]
        Task<int> GetPlayerStatAsync(int uid, string statName);

        [OperationContract(Action = "http://tempuri.org/IASUPService/SetPlayerStat", ReplyAction = "http://tempuri.org/IASUPService/SetPlayerStatResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool SetPlayerStat(int uid, string statName, int value);

        [OperationContract(Action = "http://tempuri.org/IASUPService/SetPlayerStat", ReplyAction = "http://tempuri.org/IASUPService/SetPlayerStatResponse")]
        Task<bool> SetPlayerStatAsync(int uid, string statName, int value);

        [OperationContract(Action = "http://tempuri.org/IASUPService/UpdatePlayerStat", ReplyAction = "http://tempuri.org/IASUPService/UpdatePlayerStatResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool UpdatePlayerStat(int uid, string statName, int value);

        [OperationContract(Action = "http://tempuri.org/IASUPService/UpdatePlayerStat", ReplyAction = "http://tempuri.org/IASUPService/UpdatePlayerStatResponse")]
        Task<bool> UpdatePlayerStatAsync(int uid, string statName, int value);

        [OperationContract(Action = "http://tempuri.org/IASUPService/CreateUser", ReplyAction = "http://tempuri.org/IASUPService/CreateUserResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool CreateUser(int uid, string name);

        [OperationContract(Action = "http://tempuri.org/IASUPService/CreateUser", ReplyAction = "http://tempuri.org/IASUPService/CreateUserResponse")]
        Task<bool> CreateUserAsync(int uid, string name);

        [OperationContract(Action = "http://tempuri.org/IASUPService/UserExists", ReplyAction = "http://tempuri.org/IASUPService/UserExistsResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool UserExists(int uid);

        [OperationContract(Action = "http://tempuri.org/IASUPService/UserExists", ReplyAction = "http://tempuri.org/IASUPService/UserExistsResponse")]
        Task<bool> UserExistsAsync(int uid);

        [OperationContract(Action = "http://tempuri.org/IASUPService/GetUserStatTable", ReplyAction = "http://tempuri.org/IASUPService/GetUserStatTableResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        UserInfo[] GetUserStatTable();

        [OperationContract(Action = "http://tempuri.org/IASUPService/GetUserStatTable", ReplyAction = "http://tempuri.org/IASUPService/GetUserStatTableResponse")]
        Task<UserInfo[]> GetUserStatTableAsync();

        [OperationContract(Action = "http://tempuri.org/IASUPService/TryConnect", ReplyAction = "http://tempuri.org/IASUPService/TryConnectResponse")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool TryConnect();
    }

    public interface IASUPServiceChannel : IASUPService, IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThrough]
    public partial class WCFClientInstance : ClientBase<IASUPService>, IASUPService
    {
        public WCFClientInstance()
        {
        }

        public WCFClientInstance(Binding binding, EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public int GetPlayerStat(int uid, string statName)
        {
            return Channel.GetPlayerStat(uid, statName);
        }

        public Task<int> GetPlayerStatAsync(int uid, string statName)
        {
            return Channel.GetPlayerStatAsync(uid, statName);
        }

        public bool SetPlayerStat(int uid, string statName, int value)
        {
            return Channel.SetPlayerStat(uid, statName, value);
        }

        public Task<bool> SetPlayerStatAsync(int uid, string statName, int value)
        {
            return Channel.SetPlayerStatAsync(uid, statName, value);
        }

        public bool UpdatePlayerStat(int uid, string statName, int value)
        {
            return Channel.UpdatePlayerStat(uid, statName, value);
        }

        public Task<bool> UpdatePlayerStatAsync(int uid, string statName, int value)
        {
            return Channel.UpdatePlayerStatAsync(uid, statName, value);
        }

        public bool CreateUser(int uid, string name)
        {
            return Channel.CreateUser(uid, name);
        }

        public Task<bool> CreateUserAsync(int uid, string name)
        {
            return Channel.CreateUserAsync(uid, name);
        }

        public bool UserExists(int uid)
        {
            return Channel.UserExists(uid);
        }

        public Task<bool> UserExistsAsync(int uid)
        {
            return Channel.UserExistsAsync(uid);
        }

        public UserInfo[] GetUserStatTable()
        {
            return Channel.GetUserStatTable();
        }

        public Task<UserInfo[]> GetUserStatTableAsync()
        {
            return Channel.GetUserStatTableAsync();
        }

        public bool TryConnect()
        {
            return Channel.TryConnect();
        }
    }
}
