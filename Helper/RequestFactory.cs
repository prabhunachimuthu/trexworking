using RestSharp;

namespace OneDirect.Helper
{
    public class RequestFactory: IRequestFactory
    {
        /// <summary>
        /// Returns new REST client instance.
        /// </summary>
        public IRestClient CreateClient()
        {
            return new RestClient();
        }

        /// <summary>
        /// Returns new REST request instance.
        /// </summary>
        public IRestRequest CreateRequest()
        {
            return new RestRequest();
        }
    }
}
