using Encoder;
using System.Net.Sockets;

namespace Parser
{
    /// <summary>
    /// Interface for implementing dynamic content plugins
    /// </summary>
    public interface IDynamicContent
    {
        /// <summary>
        /// Method for handling connection flow
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        Task<string> HandleConnectionFlow(NetworkStream stream, IEncoder encoder);

        /// <summary>
        /// Method for definint content of the page
        /// </summary>
        /// <param name="navigationOptions">Navigation options</param>
        /// <returns>String for content</returns>
        string Content(string navigationOptions);

        /// <summary>
        /// Method for defining footer of the page
        /// </summary>
        /// <param name="navigationOptions">Navigation options</param>
        /// <returns>String for footer</returns>
        string Footer(string navigationOptions);
    }
}
