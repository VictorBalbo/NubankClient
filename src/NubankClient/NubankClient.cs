using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using NubankClient.Models;
using NubankClient.Models.ApiResponses;
using NubankClient.Http;
using NubankClient.QRCode;

namespace NubankClient
{
    public class NubankClient
    {
        private const string StatementGraphQl = "{ \"query\": \"{ viewer { savingsAccount { feed { id __typename title detail postDate ... on TransferInEvent { amount originAccount { name }} ... on TransferOutEvent { amount destinationAccount { name }} ... on BarcodePaymentEvent { amount }}}}}\" }";
        /// <summary>
        /// User Login
        /// </summary>
        private readonly string _login;
        /// <summary>
        /// User password
        /// </summary>
        private readonly string _password;
        private readonly IHttpClient _httpClient;
        private readonly Endpoints _endpoints;
        private readonly QRCodeClient _qrCodeClient;
        /// <summary>
        /// User Authentication Token
        /// </summary>
        private string _authToken;

        public NubankClient(string login, string password)
        {
            _login = login;
            _password = password;
            _httpClient = new HttpClient();
            _endpoints = new Endpoints(_httpClient);
            _qrCodeClient = new QRCodeClient();
        }

        /// <summary>
        /// Login to Nubank API
        /// </summary>
        /// <returns>
        /// Returns 'true' for successful login and 'false' for need QRCode authentication
        /// </returns>
        public async Task<bool> LoginAsync()
        {
            await GetAuthTokenAsync();

            return _endpoints.Events != null;
        }

        /// <summary>
        /// Login to Nubank using QRCode authentication
        /// </summary>
        /// <returns>
        /// Returns 'true' for successful login and 'false' failed login
        /// </returns>
        public async Task<bool> LoginWithQrCodeAsync()
        {
            await GetAuthTokenAsync();

            var body = new
            {
                qr_code_id = _qrCodeClient.Code,
                type = "login-webapp"
            };

            var liftUrl = await _endpoints.GetLiftUrl();
            var response = await _httpClient.PostWithAuthorizationAsync<AuthenticationResponse>(liftUrl, body, _authToken);

            FillAuthToken(response);

            FillAuthenticatedUrls(response);

            return _endpoints.Events != null;
        }

        /// <summary>
        /// Get QRCode string, used for QRCode authentication
        /// </summary>
        /// <returns>
        /// Returns a string representing the QRCode 
        /// </returns>
        public string GetQRCodeAsAscii() => _qrCodeClient.GetQrCodeAsAscii();

        /// <summary>
        /// Get user events on Nubank 
        /// </summary>
        /// <returns>List of events for user</returns>
        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            EnsureAuthenticated();

            var response = await _httpClient.GetWithAuthorizationAsync<GetEventsResponse>(_endpoints.Events, _authToken);
            
            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new Exception(response.Error);
            }
            
            return response.Events;
        }

        /// <summary>
        /// Get user savings from Nubank 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Saving>> GetSavingsAsync()
        {
            EnsureAuthenticated();

            var response = await _httpClient.PostWithAuthorizationAsync<GetSavingsResponse>(_endpoints.GraphQl, StatementGraphQl, _authToken);

            return response.Savings;
        }

        /// <summary>
        /// Get authentication token using user Login and Password
        /// </summary>
        /// <returns></returns>
        private async Task GetAuthTokenAsync()
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                return;
            }

            var body = new
            {
                client_id = "other.conta",
                client_secret = "yQPeLzoHuJzlMMSAjC-LgNUJdUecx8XO",
                grant_type = "password",
                login = _login,
                password = _password
            };

            var loginUrl = await _endpoints.GetLoginUrlAsync();

            var response = await _httpClient.PostAsync<AuthenticationResponse>(loginUrl, body, default);

            FillAuthToken(response);

            FillAuthenticatedUrls(response);
        }

        /// <summary>
        ///  Extract authentication token from authentication request
        /// </summary>
        /// <param name="response">Response to the authentication request</param>
        private void FillAuthToken(AuthenticationResponse response)
        {
            if (!string.IsNullOrEmpty(response.AccessToken))
            {
                _authToken = response.AccessToken;
                return;
            }

            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new AuthenticationException(response.Error);
            }
            throw new AuthenticationException("Unknow error on trying to authenticate to Nubank");
        }

        /// <summary>
        /// Get Authenticated Urls that will be used once the authentication is completed
        /// </summary>
        /// <param name="response"></param>
        private void FillAuthenticatedUrls(AuthenticationResponse response)
        {
            _endpoints.AutenticatedUrls = response.Links;
        }

        /// <summary>
        /// Garantee the user is already authenticated
        /// </summary>
        private void EnsureAuthenticated()
        {
            if (string.IsNullOrEmpty(_authToken))
            {
                throw new Exception("Authentication needed");
            }
        }
    }
}