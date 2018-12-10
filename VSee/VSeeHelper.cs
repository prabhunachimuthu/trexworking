using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.VSee;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OneDirect.Vsee
{
    public class VSeeHelper
    {
        private static IRequestFactory mFactory { get; set; }

        public VSeeHelper()
        {
            mFactory = new RequestFactory();
        }
        public dynamic GetUserList()
        {
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("user/list?apikey={0}", "fghnoxeqrdecu9xyzw0lydeu86izx7824hzp4jlc3awfhvyigk1vhtiqimqknkuv"); ;
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    secretkey = "gtsf6ygej8bqngnjvffqrrowxh6yk0yoqsvrmxavbtxeh3p2izjouikp6y6olgl3"
                });

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    List<GetUser> userlist = new List<GetUser>();
                    if (list.Count > 0 && list["data"] != null && list["data"].Count > 0)
                    {

                        for (int i = 0; i < list["data"].Count; i++)
                        {
                            string result = GetURI("", "", list["data"][i]);
                            if (!string.IsNullOrEmpty(result))
                            {
                                GetUser user = new GetUser();
                                user.username = list["data"][i];
                                user.uri = result;
                                userlist.Add(user);
                            }
                        }
                    }
                    return userlist;
                }
            }
            catch (Exception ex)
            {
               
            }
            return null;
        }

        public dynamic GetURI(string therapistname, string therapistpassword, string patientname)
        {
            try
            {

                var jsonstring = "{'secretkey':'" + ConfigVars.NewInstance.secretkey + "','uris':[[{'init':{'commands':[{'setUser':{'username':'" + therapistname + "','password':'" + therapistpassword + "'}},{'setAddressBook':{'enabled':false}},{'setFirstTutorial':{'enabled':false}}]}},{'onIdle':{'timeout':120,'commands':[{'exit':{}}]}},{'onEndCall':{'commands':[{'exit':{}}]}},{'showLocalVideo':{}},{'call':{'username':'" + patientname + "'}},{'chat':{'username':'" + patientname + "'}}]]}";
               

                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonstring);

                var request = mFactory.CreateRequest();
                request.Resource = string.Format("uri/create?apikey={0}", ConfigVars.NewInstance.apikey); ;
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", jsonstring, ParameterType.RequestBody);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    if (list.Count > 0 && list["data"] != null && list["data"].Count > 0)
                    {
                        return list["data"][0];
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
               
            }
            return "";
        }

        public dynamic AddUser(AddUser user)
        {
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("user/create?apikey={0}", ConfigVars.NewInstance.apikey);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(user);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    return list;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public dynamic UpdateUser(AddUser user)
        {
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("user/update?apikey={0}", ConfigVars.NewInstance.apikey);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(user);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    return list;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public dynamic DeleteUser(DeleteUser user)
        {
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("user/delete?apikey={0}", ConfigVars.NewInstance.apikey);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(user);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    return list;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public dynamic StateUser(StateUser user)
        {
            try
            {
                user.secretkey = ConfigVars.NewInstance.secretkey;// "gtsf6ygej8bqngnjvffqrrowxh6yk0yoqsvrmxavbtxeh3p2izjouikp6y6olgl3";
                user.start = 1409913002000;
                user.end = 1512412200000;
                user.limit = 5000;
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("stats/calls?apikey={0}", ConfigVars.NewInstance.apikey);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(user);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    return list;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public dynamic GetLink(string command)
        {
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("uri/create?apikey={0}", ConfigVars.NewInstance.apikey);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(command);

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri("https://api.vsee.com/");
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynamic list = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<dynamic>(httpResponse);
                    return list;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
    }
}
