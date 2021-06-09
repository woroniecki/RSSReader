using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Jobs;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.JobServices
{
    public class UpdateBlogsJobService : IUpdateBlogsJobService
    {
        //private IAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IHttpHelperService _httpService;

        //public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public UpdateBlogsJobService(IMapper mapper, IUnitOfWork unitOfWork, IHttpHelperService httpService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpService = httpService;
        }

        public async Task<UpdateBlogsJobResponse> UpdateBlogs()
        {
            List<string> failed_updates = new List<string>();
            List<string> succeeded_updates = new List<string>();
            List<string> no_update = new List<string>();
            int big_step = 100;
            int small_step = 4;
            int skip_amount = 0;

            List<Blog> blogs;
            do
            {
                blogs = await _unitOfWork.BlogRepo.GetListWithPosts(skip_amount, big_step);

                skip_amount += big_step;

                for (int i = 0; i < blogs.Count; i += small_step)
                {
                    List<Task> tasks = new List<Task>(small_step + 1);

                    for (int j = i; j < i + small_step && j < blogs.Count; j++)
                    {
                        Blog blog = blogs[j];
                        tasks.Add(UpdateBlog(blog, failed_updates, succeeded_updates, no_update));
                    }

                    await Task.WhenAll(tasks);

                    await _unitOfWork.Context.SaveChangesAsync();
                }
            }
            while (blogs.Count() > 0);

            var result = new UpdateBlogsJobResponse()
            {
                Failed = failed_updates,
                Succeeded = succeeded_updates,
                NoUpdate = no_update
            };

            return result;
        }

        private async Task UpdateBlog(Blog blog, List<string> failed_updates, List<string> succeeded_updates, List<string> no_update)
        {
            try
            {
                int result = await FeedMethods.UpdateBlogPostsIfRequired(blog, _httpService, _mapper);
                if (result == -1)
                {
                    lock (failed_updates)
                    {
                        failed_updates.Add($"{blog.Url}");
                    }
                }
                else
                {
                    if (result > 0)
                    {
                        lock (succeeded_updates)
                        {
                            succeeded_updates.Add($"{blog.Url} - {result} added");
                        }
                    }
                    else
                    {
                        lock (no_update)
                        {
                            no_update.Add($"{blog.Url}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (failed_updates)
                {
                    failed_updates.Add($"{blog.Url} - {ex.Message}");
                }
            }
        }
    }

    public interface IUpdateBlogsJobService {
        Task<UpdateBlogsJobResponse> UpdateBlogs();
    }
}
