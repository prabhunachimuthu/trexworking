using RestSharp;

namespace OneDirect.Helper
{
    public interface IRequestFactory
    {
        /// <summary>
        /// Returns new REST client instance.
        /// </summary>
        IRestClient CreateClient();

        /// <summary>
        /// Returns new REST request instance.
        /// </summary>
        IRestRequest CreateRequest();
    }
}
