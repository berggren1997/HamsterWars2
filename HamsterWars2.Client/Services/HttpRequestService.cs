﻿using HamsterWars2.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.DataTransferObjects;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HamsterWars2.Client.Services
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthStateProvider _authStateProvider;

        public HttpRequestService(HttpClient httpClient, AuthStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        //TODO: Dessa två metoder är i princip exakt samma, så detta borde kunna lösas på ett generiskt sätt för get-metoder
        public async Task<IEnumerable<HamsterDto>> GetHamstersAsync()
        {
            //test för att prova skicka en request till en PROTECTED ENDPOINT, genom att hämta token från localstorage, och skicka den som header i sin request (fungerade)
            //sätter istället denna logik i authentication service när man loggar in, så man inte vid varje önskad request, ska behöva hämta token, och skicka den igen
            //var token = await _authStateProvider.GetToken(); 
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync("api/hamsters");

            if(!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch hamsters from server. Reason: {response.ReasonPhrase}");

            var content = await response.Content.ReadAsStringAsync();

            var hamsters = JsonSerializer.Deserialize<IEnumerable<HamsterDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return hamsters;
        }
        
        public async Task<HamsterDto> GetHamsterByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/hamsters/{id}");

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Failed to fetch hamster from server. Reason: {response.ReasonPhrase}");

            var content = await response.Content.ReadAsStringAsync();

            var hamster = JsonSerializer.Deserialize<HamsterDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return hamster;
        }
        
        public async Task<HamsterDto> GetRandomHamsterAsync()
        {
            var response = await _httpClient.GetAsync("api/hamsters/random");
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"The response from the server failed. Reason: {response.ReasonPhrase}");
            
            var content = await response.Content.ReadAsStringAsync();
            
            var hamster = JsonSerializer.Deserialize<HamsterDto>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); 
            
            return hamster;
        }
        public async Task<IEnumerable<MatchResultsDto>> GetHamsterMatchesAsync()
        {
            var response = await _httpClient.GetAsync("api/hamsters/hamstermatches");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong when trying to fetch the hamsters");

            var content = await response.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<IEnumerable<MatchResultsDto>>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }
        
        public async Task DeleteHamster(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/hamsters/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Something went wrong when trying to delete hamster with id {id}");
        }
        
        public async Task<IEnumerable<HamsterDto>> GetTopFiveHamsters()
        {
            var response = await _httpClient.GetAsync("api/hamsters/topfive");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong when fetching data from the server");

            var content = await response.Content.ReadAsStringAsync();

            var bottomFiveHamsters = JsonSerializer.Deserialize<IEnumerable<HamsterDto>>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return bottomFiveHamsters;
        }
        
        public async Task<IEnumerable<HamsterDto>> GetBottomFiveHamsters()
        {
            var response = await _httpClient.GetAsync("api/hamsters/bottomfive");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong");

            var content = await response.Content.ReadAsStringAsync();

            var topFiveHamsters = JsonSerializer.Deserialize<IEnumerable<HamsterDto>>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return topFiveHamsters;
        }

        public async Task VoteForHamster(MatchCompletedDto hamster)
        {
            if (hamster == null)
                throw new ArgumentNullException(nameof(hamster));

            if (hamster.MatchWon)
            {
                hamster.HamsterCompetitor.Wins++;
            }
            else
            {
                hamster.HamsterCompetitor.Defeats++;
            }
            
            hamster.HamsterCompetitor.TotalGames++;

            await _httpClient.PutAsJsonAsync($"api/hamsters/{hamster.HamsterCompetitor.Id}", hamster.HamsterCompetitor);

        }

        public async Task<bool> RegisterMatchData(MatchDataDto matchData)
        {
            if(matchData == null)
                throw new ArgumentNullException(nameof(matchData));
            
            var response = await _httpClient.PostAsJsonAsync("api/match", matchData);

            return response.StatusCode == HttpStatusCode.OK;
        }
        
        public async Task<IEnumerable<HamsterDto>> GetSpecificHamsterMatchData(int hamsterId)
        {
            var response = await _httpClient.GetAsync($"api/hamsters/matchwinners/{hamsterId}");
            
            if (!response.IsSuccessStatusCode)
                return null;
            
            var content = await response.Content.ReadAsStringAsync();
            
            var hamstersDefeated = JsonSerializer.Deserialize<IEnumerable<HamsterDto>>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return hamstersDefeated;
        }

        public async Task<bool> CreateHamster(CreateHamsterDto newHamster)
        {
            var response = await _httpClient.PostAsJsonAsync("api/hamsters", newHamster);

            return response.StatusCode == HttpStatusCode.Created;
        }

        public async Task DeleteMatchRow(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/match/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Something went wrong when trying to delete matchrow with id {id}");
        }
    }
}
