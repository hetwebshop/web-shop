using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.Data.IUserOfferRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Entities.JobPost;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.Services.UserOfferServices
{
    public class UserJobPostService : IUserJobPostService
    {
        private readonly IUserJobPostRepository userJobPostRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICompanyJobPostRepository companyJobPostRepo;
        private string predictionApiUrl;
        private readonly IBlobStorageService blobStorageService;

        public UserJobPostService(IUserJobPostRepository userJobPostRepository, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IConfiguration configuration, IBlobStorageService blobStorageService)
        {
            this.userJobPostRepository = userJobPostRepository;
            _uow = uow;
            companyJobPostRepo = companyJobPostRepository;
            predictionApiUrl = configuration.GetSection("PredictionApiUrl").Value;
            this.blobStorageService = blobStorageService;
        }

        public async Task<PagedList<UserJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<PagedList<UserJobPostDto>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetUserJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<UserJobPostDto> GetUserJobPostByIdAsync(int id)
        {
            var userJobPost = await userJobPostRepository.GetUserJobPostByIdAsync(id);
            return userJobPost.ToDto();
        }

        public async Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            try
            {
                var entity = userJobPostDto.ToEntity();
                var newItem = await userJobPostRepository.CreateUserJobPostAsync(entity);
                if (newItem == null)
                    throw new Exception("Pretplata koju ste postavili ne postoji");
                var dto = newItem.ToDto();
                var user = await _uow.UserRepository.GetUserByIdAsync(dto.SubmittingUserId);
                dto.CurrentUserCredits = user.Credits;
                return dto;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<UserApplicationDto> CreateUserApplicationAsync(UserApplicationDto userApplication)
        {
            try
            {
                var companyJobPost = await companyJobPostRepo.GetCompanyJobPostByIdAsync(userApplication.CompanyJobPostId);
                if (companyJobPost == null)
                    return null;
                PredictionApiResponse predictionApiResponse = null;
                if(userApplication.CvFilePath != null)
                {
                    using (var client = new HttpClient())
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(companyJobPost.Position), "job_description");

                        var file = await blobStorageService.GetFileAsync(userApplication.CvFileName);
                        var fileContent = new ByteArrayContent(file.FileContent);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                        content.Add(fileContent, "cv", Path.GetFileName(userApplication.CvFilePath));

                        HttpResponseMessage response = await client.PostAsync(predictionApiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseStr = await response.Content.ReadAsStringAsync();
                            var jsonDataStartIndex = responseStr.IndexOf('{');
                            if (jsonDataStartIndex >= 0)
                            {
                                var validJsonStr = responseStr.Substring(jsonDataStartIndex);
                                predictionApiResponse = JsonConvert.DeserializeObject<PredictionApiResponse>(validJsonStr);
                            }
                        }
                    }
                }
                
                var entity = userApplication.ToEntity();
                if(predictionApiResponse != null)
                {
                    entity.AIMatchingDescription = predictionApiResponse.Obrazlozenje;
                    entity.AIMatchingEducationLevel = predictionApiResponse.Opis.PoklapanjeDomeneZnanja;
                    entity.AIMatchingExperience = predictionApiResponse.Opis.PoklapanjeIskustva;
                    entity.AIMatchingSkills = predictionApiResponse.Opis.PoklapanjeVjestina;
                    entity.AIMatchingResult = predictionApiResponse.Rezultat;
                }
                var newItem = await userJobPostRepository.CreateUserApplicationAsync(entity);
                var dto = newItem.ToDto();
                return dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<UserJobPostDto> UpdateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            var newItem = await userJobPostRepository.UpdateUserJobPostAsync(userJobPostDto.ToEntity());
            return newItem.ToDto();
        }

        public async Task<List<JobCategory>> GetAllJobCategoriesAsync()
        {
            var jobCategories = await userJobPostRepository.GetAllJobCategoriesAsync();
            return jobCategories;
        }

        public async Task<List<JobType>> GetAllJobTypesAsync()
        {
            var jobTypes = await userJobPostRepository.GetAllJobTypesAsync();
            return jobTypes;
        }

        public async Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync()
        {
            var adTypes = await userJobPostRepository.GetAllAdvertisementTypesAsync();
            return adTypes;
        }

        public async Task<List<UserJobPostDto>> GetMyAdsAsync(int userId)
        {
            var myAds = await userJobPostRepository.GetMyAdsAsync(userId);
            return myAds.ToDto();
        }

        public async Task<bool> DeleteUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var deleted = await userJobPostRepository.DeleteUserJobPostByIdAsync(jobPostId);
            return deleted;
        }

        public async Task<bool> CloseUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var closed = await userJobPostRepository.CloseUserJobPostByIdAsync(jobPostId);
            return closed;
        }

        public async Task<bool> ReactivateUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var reactivated = await userJobPostRepository.ReactivateUserJobPostByIdAsync(jobPostId);
            return reactivated;
        }

        public async Task<UserJobPostDto> UpdateAdUserBaseInfo(UserAdBaseInfoRequest userAdBaseInfoRequest)
        {
            var updated = await userJobPostRepository.UpdateAdUserBaseInfo(userAdBaseInfoRequest);
            return updated.ToDto();
        }
        public async Task<UserJobPostDto> UpdateAdInfo(UserAdInfoUpdateRequest userAdInfoUpdateRequest)
        {
            var updated = await userJobPostRepository.UpdateAdInfo(userAdInfoUpdateRequest);
            var dto = updated.ToDto();
            return dto;
        }
    }

    public class PredictionApiResponse
    {
        public int Rezultat { get; set; }
        [JsonProperty("opis")]
        public Opis Opis { get; set; }
        [JsonProperty("Obrazloženje")]
        public string Obrazlozenje { get; set; }
    }

    public class Opis
    {
        [JsonProperty("Poklapanje Vještina")]
        public int PoklapanjeVjestina { get; set; }
        [JsonProperty("Poklapanje Iskustva")]
        public int PoklapanjeIskustva { get; set; }
        [JsonProperty("Poklapanje Domene Znanja")]
        public int PoklapanjeDomeneZnanja { get; set; }
    }
}
